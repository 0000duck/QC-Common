using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace QuantumConcepts.Common.Search.Google
{
    [XmlRoot("GSP")]
    public class SearchResponse
    {
        private string _version;
        private decimal _time;
        private SearchResults _results;

        [XmlAttribute("VER")]
        public string Version
        {
            get { return _version; }
            set { _version = value; }
        }

        [XmlElement("TM")]
        public decimal Time
        {
            get { return _time; }
            set { _time = value; }
        }

        [XmlElement("RES")]
        public SearchResults Results
        {
            get { return _results; }
            set { _results = value; }
        }

        [XmlIgnore]
        public bool IsEmpty { get { return (_results == null || _results.IsEmpty); } }

        public void Merge(SearchResponse searchResponse)
        {
            _time += searchResponse.Time;
            _results.Merge(searchResponse.Results);
        }
    }
}
