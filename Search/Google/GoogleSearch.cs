using QuantumConcepts.Common.Utils;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace QuantumConcepts.Common.Search.Google
{
    public class GoogleSearch
    {
        private static List<MappedProperty> MAPPED_PROPERTIES = new List<MappedProperty>();
        private const string BASE_URL = "http://www.google.com/search?";
        private const string CLIENT = "google-csbe";

        public static string BaseUrl
        {
            get { return GoogleSearch.BASE_URL; }
        }

        public string Client
        {
            get { return GoogleSearch.CLIENT; }
        }

        public bool SimplifiedAndTraditionalChineseSearch { get; set; }

        public ParameterValueGroup Countries { get; set; } = new ParameterValueGroup();

        public string Context { get; set; }

        public bool Filter { get; set; } = true;

        public string CountryBoost { get; set; }

        public string HostLanguage { get; set; } = "en";

        public string QuerystringCharacterEncoding { get; set; } = "latin1";

        public string LanguageRestriction { get; set; }

        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10;

        public string XmlCharacterEnoding { get; set; } = "latin1";

        public OutputFormat OutputFormat { get; set; } = OutputFormat.XmlNoDtd;

        public string Query { get; set; }

        public SafeSearchMode SafeSearchMode { get; set; } = SafeSearchMode.Off;

        public bool IdnEncodeUrls { get; set; } = false;

        public List<SearchResult> Results { get; set; }

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
            Query = query;
        }

        public SearchResponse GetResponse()
        {
            WebClient webClient = new WebClient();
            XmlSerializer serializer = serializer = new XmlSerializer(typeof(SearchResponse));
            SearchResponse searchResponse = null;
            int realPageNumber = PageNumber;
            int realPageSize = PageSize;
            int startIndex = ((PageNumber - 1) * PageSize);
            int endIndex = ((PageSize * PageNumber) - 1);

            for (int i = startIndex; i <= endIndex; i += 10)
            {
                string url = null;
                byte[] data = null;
                string results = null;
                XmlDocument document = null;
                XmlNodeReader reader = null;
                SearchResponse thisSearchResponse = null;

                PageNumber = i;
                PageSize = Math.Min(10, endIndex - startIndex);

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

            PageNumber = realPageNumber;
            PageSize = realPageSize;

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