using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuantumConcepts.Common.Search
{
    public enum SortDirection
    {
        Ascending, Descending
    }

    public class SortOption<T>
    {
        public string Title { get; private set; }
        public Func<T, object> SortFunction { get; private set; }
        public SortDirection SortDirection { get; private set; }

        public SortOption(string title, Func<T, object> sortFunction, SortDirection sortDirection)
        {
            this.Title = title;
            this.SortFunction = sortFunction;
            this.SortDirection = sortDirection;
        }
    }
}
