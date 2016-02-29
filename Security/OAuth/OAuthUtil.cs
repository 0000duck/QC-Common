using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using QuantumConcepts.Common.Extensions;
using QuantumConcepts.Common.Utils;
using System.Net;
using System.Threading;

namespace QuantumConcepts.Common.Security.OAuth
{
    public partial class OAuthUtil
    {
        public const string OAuthVersion = "1.0";
        public const string OAuthParameter_ConsumerKey = "oauth_consumer_key";
        public const string OAuthParameter_CallBack = "oauth_callback";
        public const string OAuthParameter_ApplicationName = "application_name";
        public const string OAuthParameter_Version = "oauth_version";
        public const string OAuthParameter_SignatureMethod = "oauth_signature_method";
        public const string OAuthParameter_Signature = "oauth_signature";
        public const string OAuthParameter_Timestamp = "oauth_timestamp";
        public const string OAuthParameter_Nonce = "oauth_nonce";
        public const string OAuthParameter_Token = "oauth_token";
        public const string OAuthParameter_TokenSecret = "oauth_token_secret";
        
        private static readonly string[] OAuthParameters =
        {
            OAuthParameter_ConsumerKey,
            OAuthParameter_CallBack,
            OAuthParameter_Version,
            OAuthParameter_SignatureMethod,
            OAuthParameter_Signature,
            OAuthParameter_Timestamp,
            OAuthParameter_Nonce,
            OAuthParameter_Token,
            OAuthParameter_TokenSecret
        };
        private static Random NonceRandom = new Random();
        

        /// <summary>Helper function to compute a hash value.</summary>
        /// <param name="hashAlgorithm">The hashing algoirhtm used. If that algorithm needs some initialization, like HMAC and its derivatives, they should be initialized prior to passing it to this function.</param>
        /// <param name="data">The data to hash.</param>
        /// <returns>a Base64 string of the hash value.</returns>
        private static string ComputeHash(HashAlgorithm hashAlgorithm, string data)
        {
            if (hashAlgorithm == null)
                throw new ArgumentNullException("hashAlgorithm");

            if (string.IsNullOrEmpty(data))
                throw new ArgumentNullException("data");

            byte[] dataBuffer = Encoding.UTF8.GetBytes(data);
            byte[] hashBytes = hashAlgorithm.ComputeHash(dataBuffer);

            return Convert.ToBase64String(hashBytes);
        }

        /// <summary>Internal function to cut out all non oauth query string parameters.</summary>
        /// <param name="parameterString">The query string part of the Url.</param>
        /// <returns>A list of QueryParameter each containing the parameter name and value.</returns>
        private static List<OAuthParameter> GetQueryParameters(string parameterString)
        {
            List<OAuthParameter> parameters = new List<OAuthParameter>();

            if (parameterString.StartsWith("?"))
                parameterString = parameterString.Remove(0, 1);

            if (!string.IsNullOrEmpty(parameterString))
            {
                string[] splitParameters = parameterString.Split('&');

                foreach (string parameter in splitParameters)
                {
                    if (!string.IsNullOrEmpty(parameter) && !OAuthUtil.OAuthParameters.Contains(parameter))
                    {
                        if (parameter.IndexOf('=') > -1)
                        {
                            string[] keyAndValue = parameter.Split('=');

                            parameters.Add(new OAuthParameter(keyAndValue[0], keyAndValue[1]));
                        }
                        else
                            parameters.Add(new OAuthParameter(parameter, string.Empty));
                    }
                }
            }

            return parameters;
        }

        /// <summary>.Normalizes the request parameters according to the spec.</summary>
        /// <param name="parameters">The list of parameters already sorted.</param>
        /// <returns>a string representing the normalized parameters.</returns>
        protected static string NormalizeRequestParameters(IList<OAuthParameter> parameters)
        {
            return string.Join("&", parameters.Select(p => "{0}={1}".FormatString(p.Key, p.Value)).ToArray());
        }

        /// <summary>Generate the signature base that is used to produce the signature.</summary>
        /// <param name="url">The full url that needs to be signed including its non OAuth url parameters.</param>
        /// <param name="consumerKey">The consumer key.</param>        
        /// <param name="token">The token, if available. If not available pass null or an empty string.</param>
        /// <param name="tokenSecret">The token secret, if available. If not available pass null or an empty string.</param>
        /// <param name="httpMethod">The http method used. Must be a valid HTTP method verb (POST,GET,PUT, etc).</param>
        /// <param name="signatureType">The signature type. To use the default values use <see cref="OAuthBase.SignatureTypes">OAuthBase.SignatureTypes</see>.</param>
        /// <returns>The signature base.</returns>
        public static string GenerateSignatureBase(Uri url, string consumerKey, string token, string tokenSecret, string httpMethod, string timeStamp, string nonce, string signatureType, out string normalizedUrl, out string normalizedRequestParameters)
        {
            List<OAuthParameter> parameters = null;
            StringBuilder signatureBase = null;

            if (token == null)
                token = string.Empty;

            if (tokenSecret == null)
                tokenSecret = string.Empty;

            if (string.IsNullOrEmpty(consumerKey))
                throw new ArgumentNullException("consumerKey");

            if (string.IsNullOrEmpty(httpMethod))
                throw new ArgumentNullException("httpMethod");

            if (string.IsNullOrEmpty(signatureType))
                throw new ArgumentNullException("signatureType");

            normalizedUrl = null;
            normalizedRequestParameters = null;

            parameters = GetQueryParameters(url.Query);
            parameters.Add(new OAuthParameter(OAuthParameter_Version, OAuthVersion));
            parameters.Add(new OAuthParameter(OAuthParameter_Nonce, nonce));
            parameters.Add(new OAuthParameter(OAuthParameter_Timestamp, timeStamp));
            parameters.Add(new OAuthParameter(OAuthParameter_SignatureMethod, signatureType));
            parameters.Add(new OAuthParameter(OAuthParameter_ConsumerKey, consumerKey));

            if (!string.IsNullOrEmpty(token))
                parameters.Add(new OAuthParameter(OAuthParameter_Token, token));

            parameters.Sort();

            normalizedUrl = "{0}://{1}".FormatString(url.Scheme, url.Host);

            if (!((url.Scheme == "http" && url.Port == 80) || (url.Scheme == "https" && url.Port == 443)))
                normalizedUrl += ":" + url.Port;

            normalizedUrl += url.AbsolutePath;
            normalizedRequestParameters = NormalizeRequestParameters(parameters);

            signatureBase = new StringBuilder();
            signatureBase.AppendFormat("{0}&", httpMethod.ToUpper());
            signatureBase.AppendFormat("{0}&", UrlUtil.UrlEncode(normalizedUrl));
            signatureBase.AppendFormat("{0}", UrlUtil.UrlEncode(normalizedRequestParameters));

            return signatureBase.ToString();
        }

        /// <summary>Generate the signature value based on the given signature base and hash algorithm.</summary>
        /// <param name="signatureBase">The signature based as produced by the GenerateSignatureBase method or by any other means.</param>
        /// <param name="hash">The hash algorithm used to perform the hashing. If the hashing algorithm requires initialization or a key it should be set prior to calling this method.</param>
        /// <returns>A base64 string of the hash value.</returns>
        public static string GenerateSignatureUsingHash(string signatureBase, HashAlgorithm hash)
        {
            return ComputeHash(hash, signatureBase);
        }

        /// <summary>Generates a signature using the HMAC-SHA1 algorithm.</summary>		
        /// <param name="url">The full url that needs to be signed including its non OAuth url parameters.</param>
        /// <param name="consumerKey">The consumer key.</param>
        /// <param name="consumerSecret">The consumer seceret.</param>
        /// <param name="token">The token, if available. If not available pass null or an empty string.</param>
        /// <param name="tokenSecret">The token secret, if available. If not available pass null or an empty string.</param>
        /// <param name="httpMethod">The http method used. Must be a valid HTTP method verb (POST,GET,PUT, etc).</param>
        /// <returns>A base64 string of the hash value.</returns>
        public static string GenerateSignature(Uri url, string consumerKey, string consumerSecret, string token, string tokenSecret, string httpMethod, string timeStamp, string nonce, out string normalizedUrl, out string normalizedRequestParameters)
        {
            return GenerateSignature(url, consumerKey, consumerSecret, token, tokenSecret, httpMethod, timeStamp, nonce, OAuthSignatureAlgorithm.HMACSHA1, out normalizedUrl, out normalizedRequestParameters);
        }

        /// <summary>Generates a signature using the specified signatureType.</summary>		
        /// <param name="url">The full url that needs to be signed including its non OAuth url parameters.</param>
        /// <param name="consumerKey">The consumer key.</param>
        /// <param name="consumerSecret">The consumer seceret.</param>
        /// <param name="token">The token, if available. If not available pass null or an empty string.</param>
        /// <param name="tokenSecret">The token secret, if available. If not available pass null or an empty string.</param>
        /// <param name="httpMethod">The http method used. Must be a valid HTTP method verb (POST,GET,PUT, etc).</param>
        /// <param name="algorithm">The type of signature to use.</param>
        /// <returns>A base64 string of the hash value.</returns>
        public static string GenerateSignature(Uri url, string consumerKey, string consumerSecret, string token, string tokenSecret, string httpMethod, string timeStamp, string nonce, OAuthSignatureAlgorithm algorithm, out string normalizedUrl, out string normalizedRequestParameters)
        {
            normalizedUrl = null;
            normalizedRequestParameters = null;

            switch (algorithm)
            {
                case OAuthSignatureAlgorithm.PlainText:
                {
                    return UrlUtil.UrlEncode(string.Format("{0}&{1}", consumerSecret, tokenSecret));
                }
                case OAuthSignatureAlgorithm.HMACSHA1:
                {
                    string signatureBase = GenerateSignatureBase(url, consumerKey, token, tokenSecret, httpMethod, timeStamp, nonce, algorithm.GetDescription(), out normalizedUrl, out normalizedRequestParameters);
                    HMACSHA1 hmacSha1 = new HMACSHA1();

                    hmacSha1.Key = Encoding.UTF8.GetBytes(string.Format("{0}&{1}", UrlUtil.UrlEncode(consumerSecret), string.IsNullOrEmpty(tokenSecret) ? "" : UrlUtil.UrlEncode(tokenSecret)));

                    return GenerateSignatureUsingHash(signatureBase, hmacSha1);
                }
            }

            throw new ArgumentException("Unknown or unsupported signature algorithm.", "signatureType");
        }

        /// <summary>Generate the timestamp for the signature.</summary>
        /// <returns>A string representation of the current number of seconds since 1/1/1970.</returns>
        public static string GenerateTimeStamp()
        {
            return Convert.ToInt64((DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalSeconds).ToString();
        }

        /// <summary>Generate a random nonce.</summary>
        /// <returns>A random nonce.</returns>
        public static string GenerateNonce()
        {
            return OAuthUtil.NonceRandom.Next(123400, 9999999).ToString();
        }
    }
}
