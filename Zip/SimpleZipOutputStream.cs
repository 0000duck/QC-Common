using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.SharpZipLib.Zip;
using System.IO;
using QuantumConcepts.Common.Extensions;

namespace QuantumConcepts.Common.Zip
{
    public class SimpleZipOutputStream : IDisposable
    {
        public Stream OutputStream { get; private set; }
        public ZipOutputStream ZipStream { get; private set; }

        public SimpleZipOutputStream(string filePath, CompressionLevel level, string password = null)
            : this(File.Create(filePath), level, password)
        {
        }

        public SimpleZipOutputStream(Stream outputStream, CompressionLevel level, string password = null)
        {
            this.OutputStream = outputStream;
            this.ZipStream = new ZipOutputStream(this.OutputStream);
            this.ZipStream.SetLevel((int)level);

            if (!string.IsNullOrEmpty(password))
                this.ZipStream.Password = password;
        }

        public void AddDirectory(string name)
        {
            this.ZipStream.PutNextEntry(new ZipEntry(name));
        }

        public void AddFile(string name, string path)
        {
            using (FileStream stream = File.OpenRead(path))
            {
                AddFile(name, stream);
            }
        }

        public void AddFile(string name, byte[] data)
        {
            using (MemoryStream stream = new MemoryStream(data))
            {
                AddFile(name, stream);
            }
        }

        public void AddFile(string name, Stream stream)
        {
            StartAddFile(name);
            stream.CopyTo(this.ZipStream);
        }

        public void StartAddFile(string name)
        {
            this.ZipStream.PutNextEntry(new ZipEntry(name));
        }

        public void Dispose()
        {
            this.ZipStream.Finish();
            this.ZipStream.Dispose();
            this.OutputStream.Dispose();
        }
    }
}
