using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using QuantumConcepts.Common.Extensions;

namespace QuantumConcepts.Common.Export
{
    /// <summary>This class defines the basics to export data from an <see cref="IExportDataSource"/>.</summary>
    public abstract class ExportBase : IDisposable
    {
        /// <summary>Handler for the <see cref="ExportProgress"/> event.</summary>
        /// <param name="sender">The <see cref="ExportBase"/> that is raising the event.</param>
        /// <param name="percentComplete">The (optional) completion percentage of the export.</param>
        public delegate void ExportProgressEventHandler(object sender, decimal? percentComplete);

        /// <summary>This event is raised as the export progresses.</summary>
        public event ExportProgressEventHandler ExportProgress;

        /// <summary>This event is raised when the export completes.</summary>
        public event EventHandler ExportComplete;

        /// <summary>The default Encoding to use if an Encoding is not specified.</summary>
        public static readonly Encoding DefaultEncoding = Encoding.UTF8;

        /// <summary>Indicates whether or not the document has been started.</summary>
        protected bool IsDocumentStarted { get; set; }

        /// <summary>Indicates whether or not the page has been started.</summary>
        protected bool IsPageStarted { get; set; }

        /// <summary>The Encoding to use when writing to the stream.</summary>
        public Encoding Encoding { get; protected set; }

        /// <summary>The Stream to write to.</summary>
        public Stream Stream { get; protected set; }

        /// <summary>Indicates the percent of completion of the export..</summary>
        public decimal? PercentComplete { get; private set; }

        /// <summary>Indicates whether or not the import is complete.</summary>
        public bool IsComplete { get; private set; }

        /// <summary>The DateTime when the export was first created.</summary>
        public DateTime Started { get; private set; }

        /// <summary>Calculates how long the export has been running.</summary>
        public TimeSpan ElapsedTime { get { return (DateTime.Now - this.Started); } }

        /// <summary>
        ///  The name of the current section
        /// </summary>
        public string SectionName { get; private set; }

        public TimeSpan? EstimatedTimeRemaining
        {
            get
            {
                if (!this.PercentComplete.HasValue)
                    return null;
                else
                {
                    decimal remaining = (100 - this.PercentComplete.Value);
                    decimal perSecond = (this.PercentComplete.Value / (decimal)this.ElapsedTime.TotalSeconds);
                    decimal secondsRemaining = (remaining / perSecond);

                    return new TimeSpan(0, 0, (int)System.Math.Ceiling(secondsRemaining));
                }
            }
        }

        /// <summary>Creates a new TextWriter instance.</summary>
        /// <param name="stream">The Stream to write to.</param>
        public ExportBase(Stream stream)
            : this(stream, ExportBase.DefaultEncoding)
        { }

        /// <summary>Creates a new TextWriter instance.</summary>
        /// <param name="stream">The Stream to write to.</param>
        /// <param name="encoding">The Encoding to use.</param>
        public ExportBase(Stream stream, Encoding encoding)
        {
            this.Stream = stream;
            this.Encoding = encoding;
            this.Started = DateTime.Now;
        }

        /// <summary>When overridden in a derived class, writes the beginning information for the document.</summary>
        public virtual void BeginDocument()
        {
            if (this.IsDocumentStarted)
                throw new ApplicationException("The document has already been started.");

            this.IsDocumentStarted = true;
        }

        /// <summary>
        /// Starts a new section of the document. A section is a logical grouping of pages. For example, an Excel spreadsheet
        /// that has a a "Metadata" tab and 14 "Data" tabs would have 2 sections: one containing the tab with the metadata and one
        /// for the 14 tabs containing the data.
        /// </summary>
        /// <param name="sectionName"></param>
        public virtual void BeginSection(string sectionName)
        {
            if (!this.IsDocumentStarted)
                throw new ApplicationException("The document has not been started.");

            this.SectionName = sectionName;
        }

        public virtual void EndSection()
        {
            if (!this.IsDocumentStarted)
                throw new ApplicationException("The document has not been started.");
        }

        /// <summary>When overridden in a derived class, begins a page in the document.</summary>
        /// <param name="headers">The header values to write. NULL is allowed.</param>
        public virtual void BeginPage(string pageName, params string[] headers)
        {
            if (!this.IsDocumentStarted)
                throw new ApplicationException("The document has not been started.");

            if (this.IsPageStarted)
                throw new ApplicationException("The page has already been started.");

            this.IsPageStarted = true;
        }

        /// <summary>When overridden in a derived class, writes the values to the stream.</summary>
        /// <param name="values">The values to write.</param>
        public virtual void WriteLine(params string[] values)
        {
            if (!this.IsDocumentStarted)
                throw new ApplicationException("The document has not been started.");

            if (!this.IsPageStarted)
                throw new ApplicationException("The page has not been started.");
        }

        /// <summary>When overridden in a derived class, ends the current page in the document.</summary>
        public virtual void EndPage()
        {
            if (!this.IsDocumentStarted)
                throw new ApplicationException("The document has not been started.");

            if (!this.IsPageStarted)
                throw new ApplicationException("The page has not been started.");

            this.IsPageStarted = false;
        }

        /// <summary>When overridden in a derived class, ends the ending information for the document.</summary>
        public virtual void EndDocument()
        {
            if (!this.IsDocumentStarted)
                throw new ApplicationException("The document has not been started.");
            
            this.IsDocumentStarted = false;
        }

        /// <summary>Writes the raw value to the stream.</summary>
        protected virtual void Write(object value)
        {
            if (value == null)
                return;

            using (MemoryStream memoryStream = new MemoryStream(this.Encoding.GetBytes(value.ToString())))
            {
                memoryStream.CopyTo(this.Stream);
            }
        }

        /// <summary>
        /// Export the data in this export. This method is needed for legacy purposes.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataSource"></param>
        public virtual void ExportData<T>(IExportDataSource<T> dataSource)
        {
            ExportData<T>(new List<IExportDataSource<T>>() { dataSource });
        }


        /// <summary>
        /// Export the data in this export.
        /// </summary>
        /// <typeparam name="T">The type of the data source</typeparam>
        /// <param name="dataSources">A list of data sources. Each data source represents a logical page in the exported file.</param>
        public virtual void ExportData<T>(IList<IExportDataSource<T>> dataSources)
        {
            if (dataSources == null)
                throw new ArgumentNullException("dataSource");

            this.OnExportProgress(0);

            //Start the document, page and write the headers.
            this.BeginDocument();

            // Now, for each data source that we were passed in, make a new page and display the data
            foreach (IExportDataSource<T> dataSource in dataSources)
            {
                this.BeginSection(dataSource.Name);
                
                // Store the section name
                this.BeginPage(dataSource.Name, dataSource.Fields.Select(efd => efd.Name).ToArray());

                //Write each row.
                while (dataSource.MoveNext())
                {
                    string[] values = dataSource.Fields.Select(efd => Convert.ToString(efd.Selector(dataSource.Current))).ToArray();

                    this.WriteLine(values);
                }

                this.EndPage();
                this.EndSection();
            }

            this.EndDocument();

            //Indicate the export is complete.
            this.OnExportComplete();

            //Close the stream.
            //this.Stream.Close();
            //this.Stream.Dispose();
        }

        /// <summary>Reports the progress of the export.</summary>
        protected void OnExportProgress(decimal? percentComplete)
        {
            this.PercentComplete = percentComplete;

            if (this.ExportProgress != null)
                this.ExportProgress(this, percentComplete);
        }

        /// <summary>Reports that the export is complete.</summary>
        protected void OnExportComplete()
        {
            this.IsComplete = true;

            if (this.ExportComplete != null)
                this.ExportComplete(this, EventArgs.Empty);
        }

        /// <summary>Cleans up any resources.</summary>
        public virtual void Dispose()
        {
        }

        /// <summary>Cleans up any resources.</summary>
        ~ExportBase()
        {
            this.Dispose();
        }
    }
}
