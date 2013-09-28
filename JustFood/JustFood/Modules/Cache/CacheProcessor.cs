
using System;
using System.Globalization;
using System.Web;
using System.Web.Caching;
using System.IO;

namespace JustFood.Modules.Cache {
    /// <summary>
    /// Default Sliding 2 Hours
    /// Default Expiration 5 Hours
    /// </summary>
    public class CacheProcessor {
        readonly string CacheName = "";
        DateTime defaultExpiration;
        string appData = AppDomain.CurrentDomain.GetData("DataDirectory").ToString();
        TimeSpan defaultSliding;
        string defaultDependencyFileLocation = null;
        /// <summary>
        /// Will be maintained by each db table as single file single text in a 
        /// specific folder.
        /// </summary>
        CacheDependency defaultCacheDependency;

        /*
         * Cache Insert Vs. Add
         * Insert will overrite existing one.
         * Add will fail if already exist one.
         * */

        #region Constructors
        /// <summary>
        /// Default expiration on +5 hours
        /// </summary>
        /// <param name="context"></param>
        public CacheProcessor() {
            SetDefaults();
        }

        public CacheProcessor(string cacheName) {
            this.CacheName = cacheName;
            SetDefaults();

        }
        public CacheProcessor(DateTime expiration) {
            SetDefaults();
            //override after defaults.
            defaultExpiration = expiration;
        }

        /// <summary>
        /// Instantiate CacheProssor
        /// </summary>
        /// <param name="context"></param>
        /// <param name="expiration"></param>
        /// <param name="sliding">If data is not accessed for certain time , then it will be removed from cache.</param>
        public CacheProcessor(DateTime expiration, TimeSpan sliding) {
            SetDefaults();
            //override after defaults.
            defaultExpiration = expiration;
            defaultSliding = sliding;
        }

        /// <summary>
        /// Instantiate CacheProssor
        /// </summary>
        /// <param name="context"></param>
        /// <param name="cacheName"></param>
        /// <param name="expiration"></param>
        /// <param name="sliding">Change Default Sliding: If data is not accessed for certain time , then it will be removed from cache.</param>
        public CacheProcessor(string cacheName, DateTime expiration, TimeSpan sliding) {
            this.CacheName = cacheName;
            SetDefaults();
            //override after defaults.
            defaultExpiration = expiration;
            defaultSliding = sliding;
        }

        /// <summary>
        /// Instantiate CacheProssor
        /// </summary>
        /// <param name="context"></param>
        /// <param name="cacheName"></param>
        /// <param name="expiration"></param>
        public CacheProcessor(string cacheName, DateTime expiration) {
            this.CacheName = cacheName;
            SetDefaults();
            //override after defaults.
            defaultExpiration = expiration;
        }
        #endregion

        void SetDefaults() {
            defaultDependencyFileLocation = appData + @"\DatabaseTables\";
            defaultSliding = new TimeSpan(2, 0, 0);
            defaultExpiration = DateTime.Now.AddHours(5);
        }

        #region Sets

        /// <summary>
        /// Save cache. No Expiration and no sliding.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        public void Set(string key, object data) {
            Set(key, data, null, null, tableName: null, priority: CacheItemPriority.Default);
        }

        /// <summary>
        /// Save object as cache.
        /// </summary>
        /// <param name="key">Key object to look for.</param>
        /// <param name="data">Save any type of data.</param>
        /// <param name="tableName">Name of the table to create dependencies in file (AppData\DatabaseTables\table.table). Change the file manually if table is updated.</param>
        public void Set(string key, object data, string tableName) {
            Set(key, data, defaultExpiration, defaultSliding, tableName: tableName, priority: CacheItemPriority.Default);
        }

        /// <summary>
        /// Save object as cache.
        /// </summary>
        /// <param name="key">Key object to look for.</param>
        /// <param name="data">Save any type of data.</param>
        /// <param name="expires"></param>
        public void Set(string key, object data, DateTime expires) {
            Set(key, data, expires, null, tableName: null, priority: CacheItemPriority.Default);
        }

        /// <summary>
        /// Save object as cache.
        /// </summary>
        /// <param name="key">Key object to look for.</param>
        /// <param name="data">Save any type of data.</param>
        /// <param name="expires"></param>
        /// <param name="sliding">If data is not accessed for certain time then it will be deleted from the cache memory.</param>
        public void Set(string key, object data, TimeSpan sliding) {
            Set(key, data, null, sliding, tableName: null, priority: CacheItemPriority.Default);
        }

        /// <summary>
        /// Save object as cache.
        /// </summary>
        /// <param name="key">Key object to look for.</param>
        /// <param name="data">Save any type of data.</param>
        /// <param name="expires"></param>
        /// <param name="sliding">If data is not accessed for certain time then it will be deleted from the cache memory.</param>
        /// <param name="tableName">Name of the table for dependency.</param>
        public void Set(string key, object data, TimeSpan sliding, string tableName) {
            Set(key, data, null, sliding, tableName: tableName, priority: CacheItemPriority.Default);
        }

        /// <summary>
        /// Save object as cache.
        /// </summary>
        /// <param name="key">Key object to look for.</param>
        /// <param name="data">Save any type of data.</param>
        /// <param name="expires"></param>
        /// <param name="tableName">Name of the table to create dependencies in file (AppData\DatabaseTables\table.table). Change the file manually if table is updated.</param>
        public void Set(string key, object data, DateTime expires, string tableName) {
            Set(key, data, expires, null, tableName, CacheItemPriority.Default);
        }

        /// <summary>
        /// Save object as cache.
        /// </summary>
        /// <param name="key">Key object to look for.</param>
        /// <param name="data">Save any type of data.</param>
        /// <param name="expires"></param>
        /// <param name="sliding">If data is not accessed for certain time then it will be deleted from the cache memory.</param>
        /// <param name="tableName">Name of the table to create dependencies in file (AppData\DatabaseTables\table.table). Change the file manually if table is updated.</param>
        /// <param name="priority"></param>
        public void Set(string key, object data, DateTime? expires, TimeSpan? sliding, string tableName, CacheItemPriority priority) {
            var cache = HttpContext.Current.Cache;


            defaultCacheDependency = tableName != null
                                         ? new CacheDependency(defaultDependencyFileLocation + tableName + ".table")
                                         : null;
            var expiration = System.Web.Caching.Cache.NoAbsoluteExpiration;
            var cacheSliding = System.Web.Caching.Cache.NoSlidingExpiration;

            if (expires != null) {
                expiration = (DateTime)expires;
            }
            if (sliding != null) {
                cacheSliding = (TimeSpan)sliding;
            }

            if (data != null && key != null) {
                cache.Insert(key, data, defaultCacheDependency, expiration, slidingExpiration: cacheSliding, priority: priority, onRemoveCallback: null);
            }
        }

        /// <summary>
        /// Save object as cache.
        /// </summary>
        /// <param name="key">Key object to look for.</param>
        /// <param name="data">Save any type of data.</param>
        /// <param name="expires">If put expire then don't put sliding</param>
        /// <param name="sliding">If data is not accessed for certain time then it will be deleted from the cache memory.</param>
        /// <param name="cacheDependency">New dependency cache.</param>
        /// <param name="priority"></param>
        public void Set(string key, object data, DateTime? expires, TimeSpan? sliding, CacheDependency cacheDependency, CacheItemPriority priority) {
            var cache = HttpContext.Current.Cache;

            var expiration = System.Web.Caching.Cache.NoAbsoluteExpiration;
            var cacheSliding = System.Web.Caching.Cache.NoSlidingExpiration;

            if (expires != null) {
                expiration = (DateTime)expires;
            }
            if (sliding != null) {
                cacheSliding = (TimeSpan)sliding;
            }
            if (data != null && key != null) {
                cache.Insert(key, data, cacheDependency, expiration, cacheSliding, priority, null);
            }
        }

        /// <summary>
        /// Save object as cache.
        /// </summary>
        /// <param name="key">Key object to look for.</param>
        /// <param name="data">Save any type of data.</param>
        /// <param name="expires">If put expire then don't put sliding</param>
        /// <param name="sliding">If data is not accessed for certain time then it will be deleted from the cache memory.</param>
        /// <param name="cacheDependency">New dependency cache.</param>
        /// <param name="priority"></param>
        /// <param name="onRemoveMethod">on remove method name</param>
        public void Set(string key, object data, DateTime? expires, TimeSpan? sliding, CacheDependency cacheDependency, CacheItemPriority priority, CacheItemRemovedCallback onRemoveMethod) {
            var cache = HttpContext.Current.Cache;
            var expiration = System.Web.Caching.Cache.NoAbsoluteExpiration;
            var cacheSliding = System.Web.Caching.Cache.NoSlidingExpiration;

            if (expires != null) {
                expiration = (DateTime)expires;
            }
            if (sliding != null) {
                cacheSliding = (TimeSpan)sliding;
            }
            if (data != null && key != null) {
                cache.Insert(key, data, cacheDependency, expiration, cacheSliding, priority, onRemoveMethod);
            }
        }




        #endregion

        #region Retrieve Cache Value

        public dynamic Get(string name) {
            if (HttpContext.Current.Cache != null && HttpContext.Current.Cache[name] != null) {
                return HttpContext.Current.Cache[name];
            }
            return null;
        }

        #endregion

        #region Notify File
        public void Notify(string table) {
            string path = defaultDependencyFileLocation + table + ".table";
            File.WriteAllText(path,DateTime.UtcNow.ToString(CultureInfo.InvariantCulture));
        }

        #endregion

        #region Remove Cache

        public void Remove(string name) {
            var cache = HttpContext.Current.Cache;
            if (cache[name] != null) {
                cache.Remove(name);
            }
        }

        #endregion
    }
}