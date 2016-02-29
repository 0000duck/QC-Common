using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing.Imaging;
using System.Drawing;

namespace QuantumConcepts.Common.Net.Rest.Transmission
{
    public partial class ApiMimeParameter
	{
        public static ApiMimeParameter Create(string name, Image image)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                image.Save(stream, ImageFormat.Png);

                return new ApiMimeParameter(name, "image/png", stream.ToArray());
            }
        }
	}
}
