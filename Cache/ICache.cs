using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuantumConcepts.Common.Cache
{
    /// <summary>This interface marks an object as a cache object.</summary>
    public interface ICache
    {
        /// <summary>Performs no operation but will cause the static initializer to fire.</summary>
        void Touch();

        /// <summary>Refreshes the cache.</summary>
        void Refresh();

        /// <summary>Provides the ability for a cache to refresh other related caches.</summary>
        /// <param name="typesToIgnore">Should contain a list of cache types that have already been refreshed.</param>
        void RefreshRelatedCaches(List<Type> typesToIgnore);
    }
}
