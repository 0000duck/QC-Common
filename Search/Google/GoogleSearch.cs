using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Policy;
using System.Xml;
using System.Net;
using QuantumConcepts.Common.Utils;
using System.Reflection;
using QuantumConcepts.Common.Exceptions;
using System.Web;
using System.Xml.Serialization;
using System.IO;

namespace QuantumConcepts.Common.Search.Google
{
    public class GoogleSearch
    {
        private static List<MappedProperty> MAPPED_PROPERTIES = new List<MappedProperty>();
        private const string BASE_URL = "http://www.google.com/search?";
        private const string CLIENT = "google-csbe";

        private bool _simplifiedAndTraditionalChineseSearch;
        private ParameterValueGroup _countries = new ParameterValueGroup();
        private string _context;
        private bool _filter = true;
        private string _countryBoost;
        private string _hostLanguage = "en";
        private string _querystringCharacterEncoding = "latin1";
        private string _languageRestriction;
        private int _pageNumber = 1;
        private int _pageSize = 10;
        private string _xmlCharacterEnoding = "latin1";
        private OutputFormat _outputFormat = OutputFormat.XmlNoDtd;
        private string _query;
        private SafeSearchMode _safeSearchMode = SafeSearchMode.Off;
        private bool _idnEncodeUrls = false;
        private List<SearchResult> _results;

        public static string BaseUrl
        {
            get { return GoogleSearch.BASE_URL; }
        }

        public string Client
        {
            get { return GoogleSearch.CLIENT; }
        }

        public bool SimplifiedAndTraditionalChineseSearch
        {
            get { return _simplifiedAndTraditionalChineseSearch; }
            set { _simplifiedAndTraditionalChineseSearch = value; }
        }

        public ParameterValueGroup Countries
        {
            get { return _countries; }
            set { _countries = value; }
        }

        public string Context
        {
            get { return _context; }
            set { _context = value; }
        }

        public bool Filter
        {
            get { return _filter; }
            set { _filter = value; }
        }

        public string CountryBoost
        {
            get { return _countryBoost; }
            set { _countryBoost = value; }
        }

        public string HostLanguage
        {
            get { return _hostLanguage; }
            set { _hostLanguage = value; }
        }

        public string QuerystringCharacterEncoding
        {
            get { return _querystringCharacterEncoding; }
            set { _querystringCharacterEncoding = value; }
        }

        public string LanguageRestriction
        {
            get { return _languageRestriction; }
            set { _languageRestriction = value; }
        }

        public int PageNumber
        {
            get { return _pageNumber; }
            set { _pageNumber = value; }
        }

        public int PageSize
        {
            get { return _pageSize; }
            set { _pageSize = value; }
        }

        public string XmlCharacterEnoding
        {
            get { return _xmlCharacterEnoding; }
            set { _xmlCharacterEnoding = value; }
        }

        public OutputFormat OutputFormat
        {
            get { return _outputFormat; }
            set { _outputFormat = value; }
        }

        public string Query
        {
            get { return _query; }
            set { _query = value; }
        }

        public SafeSearchMode SafeSearchMode
        {
            get { return _safeSearchMode; }
            set { _safeSearchMode = value; }
        }

        public bool IdnEncodeUrls
        {
            get { return _idnEncodeUrls; }
            set { _idnEncodeUrls = value; }
        }

        public List<SearchResult> Results
        {
            get { return _results; }
            set { _results = value; }
        }

        static GoogleSearch()
        {
            Type type = typeof(GoogleSearch);

            MAPPED_PROPERTIES.Add(new MappedProperty("client", type.GetProperty("Client"), true));
            MAPPED_PROPERTIES.Add(new MappedProperty("c2coff", type.GetProperty("SimplifiedAndTraditionalChineseSearch")));
            MAPPED_PROPERTIES.Add(new MappedProperty("cr", type.GetProperty("Countries")));
            MAPPED_PROPERTIES.Add(new MappedProperty("cx", type.GetProperty("Context"), true));
            MAPPED_PROPERTIES.Add(new MappedProperty("filter", type.GetProperty("Filter")));
            MAPPED_PROPERTIES.Add(new MappedProperty("gl", type.GetProperty("CountryBoost")));
            MAPPED_PROPERTIES.Add(new MappedProperty("hl", type.GetProperty("HostLanguage")));
            MAPPED_PROPERTIES.Add(new MappedProperty("ie", type.GetProperty("QuerystringCharacterEncoding")));
            MAPPED_PROPERTIES.Add(new MappedProperty("lr", type.GetProperty("LanguageRestriction")));
            MAPPED_PROPERTIES.Add(new MappedProperty("start", type.GetProperty("PageNumber")));
            MAPPED_PROPERTIES.Add(new MappedProperty("num", type.GetProperty("PageSize")));
            MAPPED_PROPERTIES.Add(new MappedProperty("oe", type.GetProperty("XmlCharacterEnoding")));
            MAPPED_PROPERTIES.Add(new MappedProperty("output", type.GetProperty("OutputFormat"), true));
            MAPPED_PROPERTIES.Add(new MappedProperty("q", type.GetProperty("Query")));
            MAPPED_PROPERTIES.Add(new MappedProperty("safe", type.GetProperty("SafeSearchMode")));
            MAPPED_PROPERTIES.Add(new MappedProperty("ud", type.GetProperty("IdnEncodeUrls")));
        }

        public GoogleSearch() { }

        public GoogleSearch(string query)
        {
            _query = query;
        }

        public SearchResponse GetResponse()
        {
            WebClient webClient = new WebClient();
            XmlSerializer serializer = serializer = new XmlSerializer(typeof(SearchResponse));
            SearchResponse searchResponse = null;
            int realPageNumber = _pageNumber;
            int realPageSize = _pageSize;
            int startIndex = ((_pageNumber - 1) * _pageSize);
            int endIndex = ((_pageSize * _pageNumber) - 1);

            for (int i = startIndex; i <= endIndex; i += 10)
            {
                string url = null;
                byte[] data = null;
                string results = null;
                XmlDocument document = null;
                XmlNodeReader reader = null;
                SearchResponse thisSearchResponse = null;

                _pageNumber = i;
                _pageSize = Math.Min(10, endIndex - startIndex);

                url = BuildUrl();
                data = webClient.DownloadData(url.ToString());
                results = ASCIIEncoding.ASCII.GetString(data);
                document = new XmlDocument();
                reader = new XmlNodeReader(document);

                results = results.Substring(results.IndexOf("\n") + 1);
                document.LoadXml(results);

                thisSearchResponse = (SearchResponse)serializer.Deserialize(reader);

                if (thisSearchResponse == null || thisSearchResponse.Results == null)
                    break;

                //If this is the first response then set it, otherwise merge it into the existing response
                if (searchResponse == null)
                    searchResponse = thisSearchResponse;
                else
                    searchResponse.Merge(thisSearchResponse);

                if (searchResponse.Results.NumberOfResults == searchResponse.Results.Results.Length)
                    break;
            }

            _pageNumber = realPageNumber;
            _pageSize = realPageSize;

            if (searchResponse == null)
                return new SearchResponse();
            
            
            return searchResponse;
        }

        private string BuildUrl()
        {
            StringBuilder url = new StringBuilder();

            url.Append(BASE_URL);

            foreach (MappedProperty thisProperty in MAPPED_PROPERTIES)
            {
                object valueObject = thisProperty.Info.GetValue(this, null);

                if (valueObject == null && thisProperty.Required)
                    throw new ApplicationException("Google search parameter " + thisProperty.Name + " (" + thisProperty.Info.Name + ") is required but was not supplied.");
                else if (valueObject != null)
                {
                    string value = null;

                    if (valueObject is bool)
                        value = (((bool)valueObject) ? "1" : "0");
                    else if (valueObject.GetType().IsEnum)
                        value = EnumUtil.GetEnumDescription(valueObject);
                    else
                        value = valueObject.ToString();

                    url.Append(thisProperty.Name + "=" + value + "&");
                }
            }

            return url.ToString().Substring(0, url.Length - 1);
        }
    }
}