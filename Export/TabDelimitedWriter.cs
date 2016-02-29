using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace QuantumConcepts.Common.Export
{
    /// <summary>Exposes methods to write to tab-delimited text file.</summary>
    public class TabDelimitedWriter : TextWriter
    {
        /// <summary>The character which is used to seperate fields.</summary>
        public override char Separator { get { return '\t'; } }

        /// <summary>The characters which are used to seperate lines.</summary>
        public override string NewLine { get { return "\r\n"; } }

        /// <summary>Creates a new TabDelimitedWriter instance.</summary>
        /// <param name="stream">The Stream to write to.</param>
        public TabDelimitedWriter(Stream stream) : base(stream, TextWriter.DefaultEncoding) { }

        /// <summary>Creates a new TabDelimitedWriter instance.</summary>
        /// <param name="stream">The Stream to write to.</param>
        /// <param name="encoding">The Encoding to use.</param>
        public TabDelimitedWriter(Stream stream, Encoding encoding) : base(stream, encoding) { }
    }
}