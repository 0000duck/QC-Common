using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuantumConcepts.Common.Extensions;
using System.Net;
using System.IO;

namespace QuantumConcepts.Common.Testing
{
    public static class RandomImage
    {
        private const string BaseUri = "http://lorempixel.com/";

        public enum Category
        {
            Abstract, Animals, Business, Cats, City, Food, Nightlife, Fashion, People, Nature, Sports, Technics, Transport
        }

        public static Uri GetUri(int width, int height, bool gray = false, Category? category = null, int? number = null, string text = null)
        {
            StringBuilder uri = new StringBuilder(RandomImage.BaseUri);

            if (!category.HasValue)
                category = (Category)Enum.GetValues(typeof(Category)).Random();

            if (gray)
                uri.Append("g/");

            uri.AppendFormat("{0}/{1}/", width, height);

            if (category.HasValue)
            {
                uri.AppendFormat("{0}/", category.ToString().ToLower());

                if (number.HasValue)
                    uri.AppendFormat("{0}/", number.Value);

                if (!text.IsNullOrEmpty())
                    uri.Append(text);
            }

            return new Uri(uri.ToString(), UriKind.Absolute);
        }

        public static byte[] GetData(int width, int height, bool gray = false, Category? category = null, int? number = null, string text = null)
        {
            Uri uri = GetUri(width, height, gray, category, number, text);

            using (WebClient client = new WebClient())
            {
                return client.DownloadData(uri);
            }
        }

        public static void SaveTo(string path, int width, int height, bool gray = false, Category? category = null, int? number = null, string text = null)
        {
            Uri uri = GetUri(width, height, gray, category, number, text);
            string directory = Path.GetDirectoryName(path);

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            using (WebClient client = new WebClient())
            {
                client.DownloadFile(uri, path);
            }
        }
    }
}
