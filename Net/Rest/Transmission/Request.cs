using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Xml;
using QuantumConcepts.Common.Extensions;
using QuantumConcepts.Common.Net.Rest.DataObjects;
using QuantumConcepts.Common.Utils;

namespace QuantumConcepts.Common.Net.Rest.Transmission
{
    public enum HttpMethod { GET, POST, PUT, DELETE }

    public class Request
    {
        public static int TimeoutMilliseconds = 60000;

        public static void GetResponse<T>(Action<Response<T>> callback, Action<Exception> onError, IRestApi client, string url, HttpMethod method, params IApiParameter[] parameters) where T : IRestDataObject
        {
            Request.GetHttpWebResponse(delegate(HttpWebResponse httpResponse)
            {
                Response<T> result = Response<T>.Create(httpResponse);

                if (result.Success)
                {
                    if (callback != null)
                        callback(result);
                }
                else
                    onError(new ApplicationException(httpResponse.StatusDescription));
            }, onError, client, url, method, parameters);
        }

        public static void GetStringResponse(Action<string> callback, Action<Exception> onError, IRestApi client, string url, HttpMethod method, params IApiParameter[] parameters)
        {
            Request.GetHttpWebResponse(delegate(HttpWebResponse httpResponse)
            {
                using (Stream stream = httpResponse.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        if (callback != null)
                            callback(reader.ReadToEnd());
                    }
                }

            }, onError, client, url, method, parameters);
        }

        public static void GetHttpWebResponse(Action<HttpWebResponse> callback, Action<Exception> onError, IRestApi client, string url, HttpMethod method, params IApiParameter[] parameters)
        {
            List<ApiParameter> simpleParameters = parameters.OfType<ApiParameter>().ToListOrEmpty();
            List<ApiMimeParameter> mimeParameters = parameters.OfType<ApiMimeParameter>().ToListOrEmpty();
            string fullUrl = null;
            HttpWebRequest httpRequest = null;
            StringBuilder postData = null;

            //Allow the Client to authenticate, add parameters, etc., and return the Full URL to use.
            try
            {
                fullUrl = client.BeforeSendRequest(url, method, simpleParameters, mimeParameters);
            }
            catch (Exception ex) { onError(ex); return; }

            //If the URL was not set by the this.Client, build it here.
            if (string.IsNullOrEmpty(fullUrl))
            {
                fullUrl = url;

                //Build the parameter string, if necessary.
                if (simpleParameters.Count > 0)
                {
                    string parameterString = string.Join("&", simpleParameters.Where(p => p.IsValid).Select(p => p.ToString()).ToArray());

                    if (!string.IsNullOrEmpty(parameterString))
                        fullUrl += "?" + parameterString;
                }
            }

            //Set the request options.
            httpRequest = (HttpWebRequest)WebRequest.Create(fullUrl);
            httpRequest.UserAgent = client.ApplicationName;
            httpRequest.Method = method.ToString();

            //Add any attachments.
            if (mimeParameters.Count > 0 && mimeParameters.Any(p => p.IsValid))
            {
                string boundary = DateTimeUtil.Now.Ticks.ToString();

                postData = new StringBuilder();

                foreach (ApiMimeParameter parameter in mimeParameters.Where(p => p.IsValid))
                    parameter.Write(postData, boundary);

                //Set additional request options.
                httpRequest.Method = HttpMethod.POST.ToString();
                httpRequest.ContentType = string.Format("multipart/form-data; boundary={0}", boundary);

#if (!WINDOWS_PHONE)
                httpRequest.ContentLength = postData.Length;
#endif

                //Write the data to the request stream (async). Which will then get the response (async).
                httpRequest.BeginGetRequestStream(GetRequestStreamCallback, new RequestState(httpRequest, postData.ToString(), callback, onError));
            }
            else
                httpRequest.BeginGetResponse(GetResponseCallback, new RequestState(httpRequest, null, callback, onError));
        }

        private static void GetRequestStreamCallback(IAsyncResult result)
        {
            RequestState state = (RequestState)result.AsyncState;

            if (state.Data != null)
            {
                using (Stream stream = state.Request.EndGetRequestStream(result))
                {
                    byte[] dataBytes = Encoding.UTF8.GetBytes(state.Data);

                    stream.Write(dataBytes, 0, dataBytes.Length);
                    stream.Flush();
                }
            }

            //Send the request and get the response (async).
            state.Request.BeginGetResponse(GetResponseCallback, state);
        }

        private static void GetResponseCallback(IAsyncResult result)
        {
            RequestState state = (RequestState)result.AsyncState;
            HttpWebResponse httpResponse = null;

            try
            {
                using (System.Threading.Timer timeoutTimer = new System.Threading.Timer(new System.Threading.TimerCallback(o => state.Request.Abort())))
                {
                    httpResponse = (HttpWebResponse)state.Request.EndGetResponse(result);
                }
            }
            catch (WebException ex)
            {
                StringBuilder error = new StringBuilder();

                error.AppendLine("URI: {0}".FormatString(state.Request.RequestUri));
                error.AppendLine("Method: {0}".FormatString(state.Request.Method));

                //If there was a web exception, we'll still try to parse the response.
                if (ex.Response != null)
                {
                    using (StreamReader reader = new StreamReader(ex.Response.GetResponseStream()))
                    {
                        error.AppendLine(reader.ReadToEnd());
                    }
                }

                state.OnError(new ApplicationException(error.ToString(), ex));
                return;
            }

            if (state.Callback != null)
                state.Callback(httpResponse);
        }

        private class RequestState
        {
            public HttpWebRequest Request { get; private set; }
            public string Data { get; private set; }
            public Action<HttpWebResponse> Callback { get; private set; }
            public Action<Exception> OnError { get; private set; }

            public RequestState(HttpWebRequest request, string data, Action<HttpWebResponse> callback, Action<Exception> onError)
            {
                this.Request = request;
                this.Data = data;
                this.Callback = callback;
                this.OnError = onError;
            }
        }
    }
}