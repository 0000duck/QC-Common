using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace QuantumConcepts.Common.Net.Rest.Transmission
{
    public partial class ApiMimeParameter : IApiParameter
    {
        public string Name { get; private set; }
        public string ContentType { get; private set; }
        public byte[] Data { get; private set; }
        public bool IsValid { get { return (!string.IsNullOrEmpty(this.Name) && !string.IsNullOrEmpty(this.ContentType) && this.Data != null && this.Data.Length > 0); } }

        private ApiMimeParameter(string name, string contentType, byte[] data)
        {
            this.Name = name;
            this.ContentType = contentType;
            this.Data = data;
        }

        public static ApiMimeParameter Create(string name, string contentType, string filePath)
        {
            //Compatability for .net CF.
            using (FileStream stream = File.OpenRead(filePath))
            {
                int offset = 0;
                int count = (int)stream.Length;
                byte[] buffer = new byte[stream.Length];

                while (count > 0)
                {
                    int bytesRead = stream.Read(buffer, offset, count);

                    offset += bytesRead;
                    count -= bytesRead;
                }

                return new ApiMimeParameter(name, contentType, buffer);
            }
        }

        public void Write(StringBuilder data, string boundary)
        {
            //Header
            data.AppendFormat("--{0}", boundary);
            data.AppendLine();
            data.AppendFormat("Content-Disposition: file; name=\"{0}\"; filename=\"{1}\"", this.Name, this.Name);
            data.AppendLine();
            data.AppendFormat("Content-Type: {0}", this.ContentType);
            data.AppendLine();
            data.AppendLine();

            //Data
            if (this.Data != null && this.Data.Length > 0)
                data.AppendLine(Encoding.UTF8.GetString(this.Data, 0, this.Data.Length));

            //Footer
            data.AppendFormat("--{0}--", boundary);
            data.AppendLine();
        }
    }
}
