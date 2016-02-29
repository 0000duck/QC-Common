using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuantumConcepts.Common.Search
{
    public interface ISearchProvider
    {
        string Name { get; }
        string ResultsHeaderText { get; }
        IEnumerable<SearchResult> GetSearchResults(string keywords);
    }
}
