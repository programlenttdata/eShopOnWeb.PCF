using System.IO;

using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;

using Microsoft.Extensions.Caching.Distributed;

namespace Catalog.API.Extensions
{
    public static class RedisCacheExtensions
    {
        private static IDistributedCache distributedCache;
        private static IDistributedCache DistributedCache
        {
            get { return distributedCache; }
            set
            {
                if (distributedCache == null)
                {
                    distributedCache = value;
                }
            }
        }

        public static async Task<T> TryGetAsync<T>(this IDistributedCache cache, string key)
        {
            DistributedCache = cache;
            var value = await DistributedCache.GetAsync(key);

            if (value != null)
            {
                return FromByteArray<T>(value);
            }
            return default(T);
        }

        public static async Task TrySetAsync<T>(this IDistributedCache cache, string key, T value)
        {
            if (value == null)
                return;

            DistributedCache = cache;
            T temp = await DistributedCache.TryGetAsync<T>(key);

            if (temp == null || temp.Equals(default(T)))
            {
                var array = ToByteArray<T>(value);
                await DistributedCache.SetAsync(key, array);
            }
        }

        public static async Task TryResetAsync<T>(this IDistributedCache cache, string key, T value)
        {
            DistributedCache = cache;
            T temp = await DistributedCache.TryGetAsync<T>(key);

            if (temp != null && !temp.Equals(default(T)))
            {
                var array = ToByteArray<T>(value);
                await DistributedCache.RemoveAsync(key);
                await DistributedCache.TrySetAsync(key, array);
            }
        }

        private static byte[] ToByteArray<T>(T obj)
        {
            if (obj == null)
                return null;

            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        private static T FromByteArray<T>(byte[] data)
        {
            if (data == null)
                return default(T);

            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream(data))
            {
                return (T)bf.Deserialize(ms);
            }
        }
    }
}
