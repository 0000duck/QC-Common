using System.IO;

namespace QuantumConcepts.Common.Net.Rest.Transmission
{
    public partial class ApiMimeParameter
	{
        public static ApiMimeParameter Create(string name, string mimeType, Stream stream)
        {
            using (var memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);

                return new ApiMimeParameter(name, mimeType, memoryStream.ToArray());
            }
        }
	}
}
