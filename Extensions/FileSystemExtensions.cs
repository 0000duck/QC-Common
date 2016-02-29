using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace QuantumConcepts.Common.Extensions
{
    public static class FileSystemExtensions
    {
        public static void Copy(this DirectoryInfo source, string targetPath, bool overwrite)
        {
            source.Copy(new DirectoryInfo(targetPath), overwrite);
        }

        public static void Copy(this DirectoryInfo source, DirectoryInfo target, bool overwrite)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            if (target == null)
                throw new ArgumentNullException("target");

            if (!source.Exists)
                throw new DirectoryNotFoundException(source.FullName);

            if (!target.Exists)
                target.Create();

            foreach (FileInfo file in source.GetFiles())
                if (overwrite || !file.Exists)
                    file.CopyTo(Path.Combine(target.FullName, file.Name), overwrite);

            foreach (DirectoryInfo directory in source.GetDirectories())
                directory.Copy(new DirectoryInfo(Path.Combine(target.FullName, directory.Name)), overwrite);
        }
    }
}
