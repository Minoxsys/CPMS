namespace CPMS.Authorization.WebAPI
{
    public class MemcachedClient
    {
        private static Enyim.Caching.MemcachedClient memcachedClient;

        public static Enyim.Caching.MemcachedClient Instance
        {
            get
            {
                return memcachedClient ?? (memcachedClient = new Enyim.Caching.MemcachedClient());
            }
        }
    }
}
