using System;
using System.IO;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;

namespace LumiSoft.Net.DNS.Client
{
    /// <summary>
    /// Dns cache entry.
    /// </summary>
    [Serializable]
    internal struct DnsCacheEntry
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="answers">Dns answers.</param>
        /// <param name="addTime">Entry add time.</param>
        public DnsCacheEntry(DnsServerResponse answers, DateTime addTime)
        {
            Answers = answers;
            Time = addTime;
        }

        /// <summary>
        /// Gets dns answers.
        /// </summary>
        public DnsServerResponse Answers { get; }

        /// <summary>
        /// Gets entry add time.
        /// </summary>
        public DateTime Time { get; }
    }

    /// <summary>
    /// This class implements dns query cache.
    /// </summary>
    [Obsolete("Use DNS_Client.Cache instead.")]
    public class DnsCache
    {
        private static Hashtable _cache;

        /// <summary>
        /// Default constructor.
        /// </summary>
        static DnsCache()
        {
            _cache = new Hashtable();
        }

        /// <summary>
        /// Gets or sets how long(seconds) to cache dns query.
        /// </summary>
        public static long CacheTime { get; set; } = 10000;

        /// <summary>
        /// Adds dns records to cache. If old entry exists, it is replaced.
        /// </summary>
        /// <param name="qname"></param>
        /// <param name="qtype"></param>
        /// <param name="answers"></param>
        public static void AddToCache(string qname, int qtype, DnsServerResponse answers)
        {
            if (answers == null)
            {
                return;
            }

            try
            {
                lock (_cache)
                {
                    // Remove old cache entry, if any.
                    if (_cache.Contains(qname + qtype))
                    {
                        _cache.Remove(qname + qtype);
                    }
                    _cache.Add(qname + qtype, new DnsCacheEntry(answers, DateTime.Now));
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// Clears DNS cache.
        /// </summary>
        public static void ClearCache()
        {
            lock (_cache)
            {
                _cache.Clear();
            }
        }

        /// <summary>
        /// DeSerializes stored cache.
        /// </summary>
        /// <param name="cacheData">This value must be DnsCache.SerializeCache() method value.</param>
        public static void DeSerializeCache(byte[] cacheData)
        {
            lock (_cache)
            {
                var retVal = new MemoryStream(cacheData);

                var b = new BinaryFormatter();
                _cache = (Hashtable)b.Deserialize(retVal);
            }
        }

        /// <summary>
        /// Tries to get dns records from cache, if any.
        /// </summary>
        /// <param name="qname"></param>
        /// <param name="qtype"></param>
        /// <returns>Returns null if not in cache.</returns>
        public static DnsServerResponse GetFromCache(string qname, int qtype)
        {
            try
            {
                if (_cache.Contains(qname + qtype))
                {
                    var entry = (DnsCacheEntry)_cache[qname + qtype];

                    // If cache object isn't expired
                    if (entry.Time.AddSeconds(CacheTime) > DateTime.Now)
                    {
                        return entry.Answers;
                    }
                }
            }
            catch
            {
            }

            return null;
        }

        /// <summary>
        /// Serializes current cache.
        /// </summary>
        /// <returns>Return serialized cache.</returns>
        public static byte[] SerializeCache()
        {
            lock (_cache)
            {
                var retVal = new MemoryStream();

                var b = new BinaryFormatter();
                b.Serialize(retVal, _cache);

                return retVal.ToArray();
            }
        }
    }
}
