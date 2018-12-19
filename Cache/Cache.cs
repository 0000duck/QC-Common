using System;
using System.Collections.Generic;
using System.Linq;

namespace QuantumConcepts.Common.Cache
{
    /// <summary>Enables caching functionality for the given type (T).</summary>
    /// <typeparam name="T">The type of object that will be cached.</typeparam>
    /// <typeparam name="K">The type of the object's identifier.</typeparam>
    public abstract class Cache<T, K> : ICache
    {
        private List<T> _cache = null;

        /// <summary>This delegate is used when a cache is refreshed.</summary>
        /// <param name="sender">The cache that was refreshed.</param>
        /// <param name="e">Not currently used.</param>
        public delegate void CacheRefreshedEventhandler(object sender, EventArgs e);

        /// <summary>This event is fired when a cache is refreshed.</summary>
        public event CacheRefreshedEventhandler CacheRefreshed;

        /// <summary>This is the main entry point for access the underlying cache. All requests should go through this lazy-loading property.</summary>
        protected List<T> CachedItems
        {
            get
            {
                //Only need to fetch the cache in a single thread.
                lock (typeof(Cache<T, K>))
                {
                    if (_cache == null)
                    {
                        DoRefresh();
                        OnCacheRefreshed();
                    }
                }

                return _cache;
            }
            set { _cache = value; }
        }

        /// <summary>Gets a list of all items in the cache.</summary>
        public List<T> All { get { return this.CachedItems; } }

        /// <summary>Gets the item at the specified index in the cache.</summary>
        /// <param name="index">The index of the item to retrieve.</param>
        /// <returns>An item from the cache.</returns>
        public T this[int index]
        {
            get
            {
                if (this.CachedItems != null && index >= 0 && index < this.CachedItems.Count)
                    return this.CachedItems[index];

                throw new ArgumentOutOfRangeException("index");
            }
        }

        /// <summary>Performs no operation but will cause the static initializer to fire.</summary>
        public void Touch() { }

        /// <summary>This method extracts the key from the specified item.</summary>
        /// <param name="item">The item from which to extract the key.</param>
        /// <returns>An object of type T.</returns>
        protected abstract K GetKey(T item);

        /// <summary>Clears and reloads the cache.</summary>
        public void Refresh()
        {
            //Destroy the cache - and don't actually refresh it yet, wait for the next call that tries to access the cache.
            this.CachedItems = null;
        }

        /// <summary>This method is responsible for reloading the cache.</summary>
        protected abstract void DoRefresh();

        /// <summary>Provides the ability for a cache to refresh other related caches.</summary>
        /// <param name="typesToIgnore">Should contain a list of cache types that have already been refreshed.</param>
        public virtual void RefreshRelatedCaches(List<Type> typesToIgnore) { }

        /// <summary>Finds an item with the specified key.</summary>
        /// <param name="key">The key of the item to be found.</param>
        /// <returns>An object of type T.</returns>
        public virtual T Find(K key)
        {
            return this.CachedItems.SingleOrDefault(o => object.Equals(key, GetKey(o)));
        }

        /// <summary>This is called when the cache is refreshed. If necessary, the CacheRefreshed event will be raised.</summary>
        protected void OnCacheRefreshed()
        {
            if (CacheRefreshed != null)
                CacheRefreshed(this, EventArgs.Empty);
        }
    }
}
