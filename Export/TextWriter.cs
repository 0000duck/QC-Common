using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using QuantumConcepts.Common.Extensions;

namespace QuantumConcepts.Common.Export
{
    /// <summary>Exposes methods to write to a text file.</summary>
    public abstract class TextWriter : ExportBase
    {
        /// <summary>The character which is used to separate fields.</summary>
        public abstract char Separator { get; }

        /// <summary>The characters which are used to separate lines.</summary>
        public abstract string NewLine { get; }

        /// <summary>The character to use when encoding fields (default: double quote).</summary>
        public char TextQualifier { get; set; }

        /// <summary>Indicates whether or not every field should be surrounded by quotes.</summary>
        public bool EncodeAllFields { get; set; }

        /// <summary>Creates a new TextWriter instance.</summary>
        /// <param name="stream">The Stream to write to.</param>
        public TextWriter(Stream stream) : this(stream, ExportBase.DefaultEncoding) { }

        /// <summary>Creates a new TextWriter instance.</summary>
        /// <param name="stream">The Stream to write to.</param>
        /// <param name="encoding">The Encoding to use.</param>
        public TextWriter(Stream stream, Encoding encoding)
            : base(stream, encoding)
        {
            this.TextQualifier = '"';
        }

        /// <summary>For this type of export, nothing is performed.</summary>
        public override void BeginDocument() { base.BeginDocument(); }

        /// <summary>The first time this is called, the header values (if any) are written to the document.</summary>
        /// <param name="headers">The column headers to write as the first row.</param>
        public override void BeginPage(string pageName, params string[] headers)
        {
            base.BeginPage(pageName, headers);
            WriteLine(headers);
        }

        /// <summary>Writes the values to the document.</summary>
        public override void WriteLine(params string[] values)
        {
            if (values == null)
                return;

            base.WriteLine(values);

            Write(string.Join(this.Separator.ToString(), values.Select(o => EncodeField(o)).ToArray()));
            Write(this.NewLine);
        }

        /// <summary>For this type of export, nothing is performed.</summary>
        public override void EndPage() { base.EndPage(); }

        /// <summary>For this type of export, nothing is performed.</summary>
        public override void EndDocument() { base.EndDocument(); }

        /// <summary>Encodes the value if it contains the Separator char or NewLine string.</summary>
        protected virtual string EncodeField(string value)
        {
            if (string.IsNullOrEmpty(value))
                return null;

            if (this.EncodeAllFields || value.Contains(this.TextQualifier) || value.Contains(this.Separator) || value.Contains(this.NewLine))
                return "{0}{1}{0}".FormatString(this.TextQualifier, value.Replace(this.TextQualifier.ToString(), "{0}{0}".FormatString(this.TextQualifier)));

            return value;
        }

        /// <summary>Cleans up any resources.</summary>
        ~TextWriter()
        {
            base.Dispose();
        }
    }
}