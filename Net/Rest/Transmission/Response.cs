using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Net;
using System.Xml;
using QuantumConcepts.Common.Extensions;
using QuantumConcepts.Common.Net.Rest.DataObjects;
using System.Xml.Linq;

namespace QuantumConcepts.Common.Net.Rest.Transmission
{
    public class Response<T>
        where T : IRestDataObject
    {
        public string RawResponse { get; private set; }
        public bool Success { get; private set; }
        public T Data { get; private set; }

        private Response(string rawResponse, bool success, T data)
        {
            this.RawResponse = rawResponse;
            this.Success = success;
            this.Data = data;
        }

        public static Response<T> Create(HttpWebResponse httpResponse)
        {
            string rawResponse = null;
            bool success = false;
            T data = default(T);

            using (Stream stream = httpResponse.GetResponseStream())
            {
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    rawResponse = reader.ReadToEnd();
                }
            }

            switch (httpResponse.StatusCode)
            {
                case HttpStatusCode.Continue:
                case HttpStatusCode.OK:
                case HttpStatusCode.Created:
                case HttpStatusCode.Accepted:
                case HttpStatusCode.NoContent:
                case HttpStatusCode.ResetContent:
                case HttpStatusCode.PartialContent:
                case HttpStatusCode.Found:
                case HttpStatusCode.NotModified:
                {
                    data = DeserializeData(rawResponse);
                    success = true;
                    break;
                }
            }

            return new Response<T>(rawResponse, success, data);
        }

        public static T FromHttpWebResponse(HttpWebResponse httpResponse)
        {
            string rawResponse = null;

            using (Stream stream = httpResponse.GetResponseStream())
            {
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    rawResponse = reader.ReadToEnd();
                }
            }

            return DeserializeData(rawResponse);
        }

        public static T DeserializeData(string rawResponse)
        {
            using (StringReader stringReader = new StringReader(rawResponse))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));

                return (T)serializer.Deserialize(stringReader);
            }
        }
    }
}
