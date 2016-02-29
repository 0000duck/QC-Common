using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuantumConcepts.Common.Search
{
    public class SearchResult
    {
        public ISearchProvider Provider { get; private set; }
        public string Title { get; private set; }
        public string Description { get; private set; }
        public string Url { get; private set; }

        public SearchResult(ISearchProvider provider, string title, string description, string url)
        {
            this.Provider = provider;
            this.Title = title;
            this.Description = description;
            this.Url = url;
        }
    }
}
