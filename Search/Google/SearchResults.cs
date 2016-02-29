using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace QuantumConcepts.Common.Search.Google
{
    [XmlRoot("RES")]
    public class SearchResults
    {
        private int _firstResultIndex;
        private int _lastResultIndex;
        private int _numberOfResults;
        private bool _exactNumberOfResults;
        private bool _filtered;
        private SearchResult[] _results;

        [XmlAttribute("SN")]
        public int FirstResultIndex
        {
            get { return _firstResultIndex; }
            set { _firstResultIndex = value; }
        }

        [XmlAttribute("EN")]
        public int LastResultIndex
        {
            get { return _lastResultIndex; }
            set { _lastResultIndex = value; }
        }

        [XmlElement("M")]
        public int NumberOfResults
        {
            get { return _numberOfResults; }
            set { _numberOfResults = value; }
        }

        //[XmlElement("XT")]
        public bool ExactNumberOfResults
        {
            get { return _exactNumberOfResults; }
            set { _exactNumberOfResults = value; }
        }

        //[XmlElement("FI")]
        public bool Filtered
        {
            get { return _filtered; }
            set { _filtered = value; }
        }

        [XmlElement("R")]
        public SearchResult[] Results
        {
            get { return _results; }
            set { _results = value; }
        }

        public bool IsEmpty
        {
            get { return (_results == null || _results.Length == 0); }
        }

        public void Merge(SearchResults searchResults)
        {
            _lastResultIndex = searchResults.LastResultIndex;

            if (_results == null)
                _results = searchResults.Results;
            else
            {
                List<SearchResult> allResults = new List<SearchResult>(_results);

                allResults.AddRange(searchResults.Results);

                _results = allResults.ToArray();
            }
        }
    }
}
