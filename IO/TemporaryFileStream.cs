using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using QuantumConcepts.Common.Extensions;

namespace QuantumConcepts.Common.IO
{
    public class TemporaryFileStream : FileStream, IDisposable
    {
        public string FilePath { get; private set; }

        public TemporaryFileStream()
            : this(Path.GetTempFileName())
        {
        }

        public TemporaryFileStream(string filePath)
            : base(filePath, FileMode.OpenOrCreate)
        {
            if (filePath.IsNullOrEmpty())
                throw new ArgumentNullException("filePath");

            this.FilePath = filePath;
        }

        public TemporaryFileStream Recreate()
        {
            base.Dispose();

            return new TemporaryFileStream(this.FilePath);
        }

        public new void Dispose()
        {
            base.Dispose();

            if (File.Exists(this.FilePath))
            {
                try
                {
                    File.Delete(this.FilePath);
                }
                catch { }
            }
        }
    }
}
