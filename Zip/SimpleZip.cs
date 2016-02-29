using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using QuantumConcepts.Common.Extensions;

namespace QuantumConcepts.Common.Zip
{
    public class SimpleZip
    {
        public static void Unzip(Stream inputStream, string basePath, string password)
        {
            Directory.CreateDirectory(basePath);

            using (ZipInputStream zipStream = OpenZip(inputStream, password))
            {
                ZipEntry currentEntry = null;

                do
                {
                    currentEntry = zipStream.GetNextEntry();

                    if (currentEntry != null)
                    {
                        if (currentEntry.IsFile)
                        {
                            string filePath = (basePath + currentEntry.Name);

                            using (FileStream outputStream = File.Create(filePath))
                            {
                                ExtractFile(zipStream, outputStream);
                            }
                        }
                        else
                            Directory.CreateDirectory(basePath + currentEntry.Name);
                    }
                } while (currentEntry != null);
            }
        }

        public static bool UnzipFile(Stream inputStream, Stream outputStream, string relativeFilePath, string password)
        {
            using (ZipInputStream zipStream = OpenZip(inputStream, password))
            {
                ZipEntry currentEntry = null;

                do
                {
                    currentEntry = zipStream.GetNextEntry();

                    if (currentEntry != null && currentEntry.IsFile && currentEntry.Name.Equals(relativeFilePath))
                    {
                        ExtractFile(zipStream, outputStream);

                        return true;
                    }
                } while (currentEntry != null);
            }

            return false;
        }

        public static ZipInputStream OpenZip(Stream inputStream, string password = null)
        {
            ZipInputStream zipStream = new ZipInputStream(inputStream);

            if (!string.IsNullOrEmpty(password))
                zipStream.Password = password;

            return zipStream;
        }

        private static void ExtractFile(ZipInputStream inputStream, Stream outputStream)
        {
            inputStream.CopyTo(outputStream);
        }
    }
}
