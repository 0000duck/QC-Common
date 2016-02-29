using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.IO;
using log4net;
using QuantumConcepts.Common.Extensions;

namespace QuantumConcepts.Common.Cache
{
    /// <summary>This class keeps track of all cache objects.</summary>
    public class CacheManager
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(CacheManager));

        public static CacheManager Instance { get; private set; }

        protected List<ICache> Caches { get; set; }

        static CacheManager()
        {
            CacheManager.Instance = new CacheManager();
        }

        protected CacheManager()
        {
            this.Caches = new List<ICache>();
        }

        /// <summary>Registers the specified cache with the CacheManager.</summary>
        /// <param name="cache">The cache to register.</param>
        public void Register(ICache cache)
        {
            lock (this.Caches)
            {
                if (!this.Caches.Any(o => o.GetType() == cache.GetType()))
                {
                    //Touch the cache.
                    cache.Touch();

                    //Refresh the cache.
                    cache.Refresh();

                    //Add it to the list of caches.
                    this.Caches.Add(cache);
                }
            }
        }

        /// <summary>Registers all caches within all DLLs and EXEs which reside at the same physical location as this assembly.</summary>
        public void RegisterAllCaches(string path)
        {
            List<string> files = new List<string>();

            path = (path ?? Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));

            files.AddRange(Directory.GetFiles(path, "*.dll"));
            files.AddRange(Directory.GetFiles(path, "*.exe"));

            foreach (string file in files)
            {
                Assembly assembly = null;

                try
                {
                    assembly = Assembly.LoadFrom(file);
                }
                catch (Exception ex)
                {
                    CacheManager.Logger.Error("While loading batch processors, could not load assembly \"{0}\".".FormatString(file), ex);
                }

                RegisterAllCaches(assembly);
            }
        }

        /// <summary>Registers all caches within the provided.</summary>
        public void RegisterAllCaches(Assembly assembly)
        {
            Type baseType = typeof(ICache);

            if (assembly != null)
            {
                foreach (Type type in assembly.GetTypes().Where(t => t != baseType && baseType.IsAssignableFrom(t)))
                {
                    lock (this.Caches)
                    {
                        if (!this.Caches.Any(o => o.GetType() == type))
                        {
                            ICache instance = null;

                            try
                            {
                                instance = (ICache)Activator.CreateInstance(type);
                            }
                            catch (Exception ex)
                            {
                                CacheManager.Logger.Error("While loading batch processors, could not instantiate type \"{0}\".".FormatString(type.FullName), ex);
                            }

                            if (instance != null)
                                Register(instance);
                        }
                    }
                }
            }
        }

        /// <summary>Refreshes all of the caches.</summary>
        public void RefreshAll()
        {
            foreach (ICache c in this.Caches)
            {
                lock (typeof(CacheManager))
                {
                    c.Refresh();
                }
            }
        }
    }
}
