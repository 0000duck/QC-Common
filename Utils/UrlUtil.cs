using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using QuantumConcepts.Common.Extensions;

#if (!WINDOWS_PHONE)
using System.Web;
using System.Web.UI;
#endif

namespace QuantumConcepts.Common.Utils
{
    public partial class UrlUtil
    {
        private static readonly char[] ReservedCharacters = { '!', '*', '\'', '(', ')', ';', ':', '@', '&', '=', '+', '$', ',', '/', '?', '%', '#', '[', ']', ' ' };
        private static readonly char[] NonReservedCharacters = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '-', '_', '.', '~' };

        /// <summary>Replaces all characters which are spaces with "-" and all other characters which are not letters, numbers, or hyphens with nothing.</summary>
        /// <param name="text">The text to format.</param>
        /// <returns>A friendly URL-safe version of text.</returns>
        public static string FormatForUrl(string text)
        {
            string formattedText = text;

            formattedText = Regex.Replace(formattedText, @"\s", "-");
            formattedText = Regex.Replace(formattedText, @"[^a-z0-9\-]", "", RegexOptions.IgnoreCase);

            return formattedText;
        }

        /// <summary>This is a different Url Encode implementation since the default .NET one outputs the percent encoding in lower case. While this is not a problem with the percent encoding spec, it is used in upper case throughout OAuth.</summary>
        /// <param name="value">The value to Url encode.</param>
        /// <returns>Returns a Url encoded string'</returns>
        public static string UrlEncode(string value)
        {
            if (string.IsNullOrEmpty(value))
                return null;

            StringBuilder encoded = new StringBuilder();

            foreach (char c in value)
            {
                if (UrlUtil.ReservedCharacters.Contains(c))
                    encoded.Append("%{0:X2}".FormatString((int)c));
                else if (!UrlUtil.NonReservedCharacters.Contains(c))
                    throw new ApplicationException("Character '{0}' is not valid.".FormatString(c));
                else
                    encoded.Append(c);
            }

            return encoded.ToString();
        }
#if(!WINDOWS_PHONE)
        public static void RedirectToPage(string url)
        {
            HttpContext.Current.Response.Redirect(url);
        }

        public static string BuildInfoPopupUrl(ResourceUrl url, Control control)
        {
            return "JavaScript:window.open('" + url.GetAbsoluteUrl(control) + "', 'Info', 'width=600,height=400,toolbar=0,location=0,menubar=0,directories=0,resizable=1,scrollbars=1');";
        }

        public static bool RedirectToReferer()
        {
            HttpResponse response = HttpContext.Current.Response;
            string url = GetReferer();

            if (!string.IsNullOrEmpty(url))
            {
                response.Redirect(url);

                return true;
            }

            return false;
        }

        public static string GetReferer()
        {
            HttpRequest request = HttpContext.Current.Request;

            if (request.UrlReferrer != null)
                return request.UrlReferrer.ToString();

            return null;
        }

        public static string GetRefererOrDefault(string defaultUrl)
        {
            return (GetReferer() ?? defaultUrl);
        }

        public static bool RedirectToSource()
        {
            HttpRequest request = HttpContext.Current.Request;
            HttpResponse response = HttpContext.Current.Response;
            HttpServerUtility server = HttpContext.Current.Server;
            string url = request.QueryString["Source"];

            if (!string.IsNullOrEmpty(url))
            {
                response.Redirect(server.UrlDecode(url));

                return true;
            }

            return false;
        }

        public static void RedirectToSource(string fallbackUrl)
        {
            if (!RedirectToSource())
                HttpContext.Current.Response.Redirect(fallbackUrl);
        }

        public static bool RedirectToSourceOrReferer()
        {
            if (!RedirectToSource())
                return RedirectToReferer();

            return true;
        }

        public static void RedirectToHttps()
        {
            HttpRequest request = HttpContext.Current.Request;
            HttpResponse response = HttpContext.Current.Response;

            if (request.IsSecureConnection)
                return;

            response.Redirect(request.Url.ToString().Replace("http://", "https://"));
        }

        public static void RedirectToHttp()
        {
            HttpRequest request = HttpContext.Current.Request;
            HttpResponse response = HttpContext.Current.Response;

            if (!request.IsSecureConnection)
                return;

            response.Redirect(request.Url.ToString().Replace("https://", "http://"));
        }

        public static void RedirectToCurrent(params string[] excludeParameters)
        {
            HttpContext context = HttpContext.Current;

            if (excludeParameters != null && excludeParameters.Length > 0)
            {
                StringBuilder url = new StringBuilder(context.Request.Path);
                bool first = true;

                if (context.Request.QueryString != null && context.Request.QueryString.Count > 0)
                {
                    string[] keys = context.Request.QueryString.AllKeys.Where(k => !excludeParameters.Contains(k)).ToArray();

                    if (keys != null && keys.Length > 0)
                    {
                        foreach (string key in keys)
                        {
                            if (first)
                                url.Append("?");
                            else
                                url.Append("&");

                            url.AppendFormat("{0}={1}", key, context.Request.QueryString[key]);
                            first = false;
                        }
                    }
                }

                RedirectToPage(url.ToString());
            }
            else
                RedirectToPage(context.Request.Url.ToString());
        }

        public static string GetAbsoluteUrl(string relativePath)
        {
            return GetAbsoluteUrl(relativePath, null);
        }

        public static string GetAbsoluteUrl(string relativePath, Control control)
        {
            HttpRequest request = HttpContext.Current.Request;
            StringBuilder urlString = new StringBuilder();
            Uri uri = null;

            if (!string.IsNullOrEmpty(relativePath))
            {
                if (control == null)
                {
                    urlString.Append(request.ApplicationPath);

                    if (urlString[urlString.Length - 1] != '/')
                        urlString.Append("/");

                    if (relativePath.StartsWith("~"))
                        urlString.Append(relativePath.Substring(2));
                    else if (relativePath.StartsWith("/"))
                        urlString.Append(relativePath.Substring(1));
                    else
                        urlString.Append(relativePath);
                }
                else
                {
                    urlString.Append("/");
                    urlString.Append(control.ResolveUrl(relativePath));
                }
            }

            uri = new Uri(request.Url.Scheme + "://" + request.Url.Host + (request.Url.Port == 80 || request.Url.Port == 443 ? "" : ":" + request.Url.Port.ToString()) + Regex.Replace(urlString.ToString(), "/+", "/"));

            return uri.AbsoluteUri;
        }
#endif
    }
}