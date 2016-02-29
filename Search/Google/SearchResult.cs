using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace QuantumConcepts.Common.Search.Google
{
    [XmlRoot("R")]
    public class SearchResult
    {
        private int _number;
        private string _mime;
        private string _url;
        private string _urlEscaped;
        private string _title;
        private int _rank;
        private string _crawlDate;
        private string _excerpt;
        private string _language;
        private SearchResultOptions _options;
        private string _additionalSearchResultsHost;

        [XmlAttribute("N")]
        public int Number
        {
            get { return _number; }
            set { _number = value; }
        }

        [XmlAttribute("MIME")]
        public string Mime
        {
            get { return _mime; }
            set { _mime = value; }
        }

        [XmlElement("U")]
        public string Url
        {
            get { return _url; }
            set { _url = value; }
        }

        [XmlElement("UE")]
        public string UrlEscaped
        {
            get { return _urlEscaped; }
            set { _urlEscaped = value; }
        }

        [XmlElement("T")]
        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        [XmlElement("RK")]
        public int Rank
        {
            get { return _rank; }
            set { _rank = value; }
        }

        [XmlElement("CRAWLDATE")]
        public string CrawlDate
        {
            get { return _crawlDate; }
            set { _crawlDate = value; }
        }

        [XmlElement("S")]
        public string Excerpt
        {
            get { return _excerpt; }
            set { _excerpt = value; }
        }

        [XmlElement("LANG")]
        public string Language
        {
            get { return _language; }
            set { _language = value; }
        }

        [XmlElement("HAS")]
        public SearchResultOptions Options
        {
            get { return _options; }
            set { _options = value; }
        }

        [XmlElement("HN")]
        public string AdditionalSearchResultsHost
        {
            get { return _additionalSearchResultsHost; }
            set { _additionalSearchResultsHost = value; }
        }
    }
}
