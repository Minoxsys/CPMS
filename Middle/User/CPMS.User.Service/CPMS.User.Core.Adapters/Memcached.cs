using System;
using System.Configuration;
using CPMS.User.Manager;
using Enyim.Caching.Memcached;

namespace CPMS.User.Core.Adapters
{
    public class Memcached<T> : ICache<T>
    {
        public void Set(string key, T obj)
        {
            int accessTokenExpirationMinutes;

            if (!int.TryParse(ConfigurationManager.AppSettings[Constants.AccessTokenExpirationMinutesKey],
                out accessTokenExpirationMinutes))
            {
                accessTokenExpirationMinutes = 30;
            }

            MemcachedClient.Instance.Store(StoreMode.Set, key, obj, TimeSpan.FromMinutes(accessTokenExpirationMinutes));
        }

        public T Get(string key)
        {
            return MemcachedClient.Instance.Get<T>(key);
        }
    }
}
