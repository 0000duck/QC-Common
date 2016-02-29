using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml;
using QuantumConcepts.Common.Extensions;
using System.Xml;

namespace QuantumConcepts.Common.Export
{
    /// <summary>Provides a simple way to write to an Excel document.</summary>
    public class ExcelWriter : ExportBase
    {
        private uint NextSheetID = 1;

        private uint SectionSubsheet = 1;

        private OpenXmlWriter Writer { get; set; }
        private SpreadsheetDocument Document { get; set; }

        /// <summary>When non-null, a new sheet will be appended to the document when the number indicated has been reached.</summary>
        public uint? MaximumRowsPerSheet { get; set; }

        /// <summary>Indicates the number of rows which have been written to the current sheet.</summary>
        public uint RowsInCurrentSheet { get; private set; }

        /// <summary>An array containing the headers which are written at the top of each sheet.</summary>
        public string[] Headers { get; private set; }

        /// <summary>Creates a new ExcelWriter instance.</summary>
        /// <param name="stream">The Stream to write to.</param>
        public ExcelWriter(Stream stream, uint? maximumRowsPerSheet = null)
            : base(stream)
        {
            this.MaximumRowsPerSheet = maximumRowsPerSheet;
        }

        /// <summary>Creates a new ExcelWriter instance.</summary>
        /// <param name="stream">The Stream to write to.</param>
        /// <param name="encoding">This parameter is not used.</param>
        public ExcelWriter(Stream stream, Encoding encoding, uint? maximumRowsPerSheet = null)
            : base(stream, encoding)
        {
            this.MaximumRowsPerSheet = maximumRowsPerSheet;
        }

        public override void BeginSection(string sectionName)
        {
            base.BeginSection(sectionName);

            this.SectionSubsheet = 1;
        }

        /// <summary>Writes the beginning of the Workbook.</summary>
        public override void BeginDocument()
        {
            base.BeginDocument();

            this.Document = SpreadsheetDocument.Create(this.Stream, SpreadsheetDocumentType.Workbook);
            this.Document.AddWorkbookPart();
            this.Document.WorkbookPart.Workbook = new Workbook();
            this.Document.WorkbookPart.Workbook.Append(new Sheets());
        }

        /// <summary>Writes the beginning of a Worksheet.</summary>
        /// <param name="headers">The column headers to write as the first row.</param>
        public override void BeginPage(string pageName, params string[] headers)
        {
            base.BeginPage(pageName, headers);

            //Create a new Worksheet and add a new Sheet to the Sheets collection.
            WorksheetPart worksheetPart = this.Document.WorkbookPart.AddNewPart<WorksheetPart>();

            this.Document.WorkbookPart.Workbook.Sheets.Append(new Sheet()
            {
                Id = this.Document.WorkbookPart.GetIdOfPart(worksheetPart),
                SheetId = this.NextSheetID,
                Name = pageName
            });
            this.Writer = OpenXmlWriter.Create(worksheetPart);
            this.Writer.WriteStartElement(new Worksheet());
            this.Writer.WriteStartElement(new SheetData());

            //Reset the RowsInCurrentSheet and increment the NextSheetID.
            this.NextSheetID++;
            this.RowsInCurrentSheet = 0;
            this.Headers = headers;

            //Write the column headers, if any.
            if (!headers.IsNullOrEmpty())
                WriteLine(headers);
        }

        /// <summary>Writes the values to the document.</summary>
        public override void WriteLine(params string[] values)
        {
            base.WriteLine(values);

            if (this.MaximumRowsPerSheet == this.RowsInCurrentSheet)
            {
                this.SectionSubsheet++;
                this.EndPage();
                this.BeginPage(this.SectionName + " " + this.SectionSubsheet.ToString(), this.Headers);
            }

            //Create a new row and set its RowIndex.
            this.Writer.WriteStartElement(new Row());

            if (!values.IsNullOrEmpty())
            {
                //For each value, create a new cell and point it to a location in the SharedStringTable.
                foreach (string value in values)
                {
                    this.Writer.WriteElement(new Cell()
                    {
                        CellValue = new CellValue(value),
                        DataType = CellValues.String
                    });
                }
            }

            this.Writer.WriteEndElement(); //End the row element.
            this.RowsInCurrentSheet++;
        }

        /// <summary>Closes the SheetData and Worksheet.</summary>
        public override void EndPage()
        {
            base.EndPage();

            this.Writer.WriteEndElement();  //SheetData
            this.Writer.WriteEndElement();  //Worksheet
            this.Writer.Close();
            this.Writer.Dispose();
        }

        /// <summary>Closes the Sheets and Workbook.</summary>
        public override void EndDocument()
        {
            base.EndDocument();

            this.Document.Close();
            this.Document.Dispose();
            this.Dispose();
        }

        /// <summary>Converts a column number (5) to its respective alpha column name (E).</summary>
        /// <remarks>
        ///     Given a 1-based column number, this method will recursively call itself in order to
        ///     determine the full name of the column.
        ///     <example>
        ///         Column number 26 would return the column name "Z."
        ///     </example>
        ///     <example>
        ///         Column number 27 would return the column name "AA."
        ///     </example>
        /// </remarks>
        /// <param name="columnNumber">The 1-based cell number.</param>
        /// <returns>A string containing the cell name.</returns>
        private string GetColumnNameFromNumber(int columnNumber)
        {
            const int charCount = 26;   //The number of letters in the alphabet.
            const int charIndexA = 65;  //The ASCII value for the letter A.

            //Start from the end of the column name - get the remainder.
            int charIndex = (columnNumber > charCount ? (columnNumber % charCount) : columnNumber);
            string column = null;

            //If charIndex is 0, that means "Z."
            if (charIndex == 0)
                charIndex = charCount;

            //Move the charIndex to the alpha portion of ASCII and cast to a char.
            column = ((char)(charIndex + (charIndexA - 1))).ToString();

            //Subtract the charIndex we just used and then divide by the number of alpha letters.
            //This will move the charIndex left "one precision."
            charIndex = ((columnNumber - charIndex) / charCount);

            //If we haven't reached the end (charIndex is still positive) then call this method
            //again and prefix the result.
            if (charIndex > 0)
                column = (GetColumnNameFromNumber(charIndex) + column);

            return column;
        }

        /// <summary>Performs the base Dispose and then deleted the temporary file that was used to store the document.</summary>
        public override void Dispose()
        {
            base.Dispose();

            if (this.Writer != null)
            {
                try
                {
                    this.Writer.Close();
                    this.Writer.Dispose();
                }
                catch { }
            }
        }

        /// <summary>Disposes of resources.</summary>
        ~ExcelWriter()
        {
            Dispose();
        }
    }
}