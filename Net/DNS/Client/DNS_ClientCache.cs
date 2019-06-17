using System;
using System.Collections.Generic;

namespace LumiSoft.Net.DNS.Client
{
    /// <summary>
    /// This class implements DNS client cache.
    /// </summary>
    public class DNS_ClientCache
    {
        private Dictionary<string, CacheEntry> _cache;
        private TimerEx _timerTimeout;

        /// <summary>
        /// Default constructor.
        /// </summary>
        internal DNS_ClientCache()
        {
            _cache = new Dictionary<string, CacheEntry>();

            _timerTimeout = new TimerEx(60000);
            _timerTimeout.Elapsed += new System.Timers.ElapsedEventHandler(TimerTimeout_Elapsed);
            _timerTimeout.Start();
        }

        /// <summary>
        /// Gets number of DNS queries cached.
        /// </summary>
        public int Count => _cache.Count;

        /// <summary>
        /// Gets or sets maximum number of seconds to cache positive DNS responses.
        /// </summary>
        public int MaxCacheTtl { get; set; } = 86400;

        /// <summary>
        /// Gets or sets maximum number of seconds to cache negative DNS responses.
        /// </summary>
        public int MaxNegativeCacheTtl { get; set; } = 900;

        /// <summary>
        /// Adds dns records to cache. If old entry exists, it is replaced.
        /// </summary>
        /// <param name="qname">Query name.</param>
        /// <param name="qtype">Query type.</param>
        /// <param name="response">DNS server response.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>qname</b> or <b>response</b> is null reference.</exception>
        /// <exception cref="ArgumentException">Is raised when any of the arguments has invalid value.</exception>
        public void AddToCache(string qname, int qtype, DnsServerResponse response)
        {
            if (qname == null)
            {
                throw new ArgumentNullException("qname");
            }
            if (qname == string.Empty)
            {
                throw new ArgumentException("Argument 'qname' value must be specified.", "qname");
            }
            if (response == null)
            {
                throw new ArgumentNullException("response");
            }

            lock (_cache)
            {
                // Remove old cache entry, if any.
                if (_cache.ContainsKey(qname + qtype))
                {
                    _cache.Remove(qname + qtype);
                }

                if (response.ResponseCode == DNS_RCode.NO_ERROR)
                {
                    int ttl = MaxCacheTtl;
                    // Search smallest DNS record TTL and use it.
                    foreach (DNS_rr rr in response.AllAnswers)
                    {
                        if (rr.TTL < ttl)
                        {
                            ttl = rr.TTL;
                        }
                    }

                    _cache.Add(qname + qtype, new CacheEntry(response, DateTime.Now.AddSeconds(ttl)));
                }
                else
                {
                    _cache.Add(qname + qtype, new CacheEntry(response, DateTime.Now.AddSeconds(MaxNegativeCacheTtl)));
                }
            }
        }

        /// <summary>
        /// Clears DNS cache.
        /// </summary>
        public void ClearCache()
        {
            lock (_cache)
            {
                _cache.Clear();
            }
        }

        /// <summary>
        /// Gets DNS server cached response or null if no cached result.
        /// </summary>
        /// <param name="qname">Query name.</param>
        /// <param name="qtype">Query type.</param>
        /// <returns>Returns DNS server cached response or null if no cached result.</returns>
        /// <exception cref="ArgumentNullException">Is raised when <b>qname</b> is null reference.</exception>
        /// <exception cref="ArgumentException">Is raised when any of the arguments has invalid value.</exception>
        public DnsServerResponse GetFromCache(string qname, int qtype)
        {
            if (qname == null)
            {
                throw new ArgumentNullException("qname");
            }
            if (qname == string.Empty)
            {
                throw new ArgumentException("Argument 'qname' value must be specified.", "qname");
            }

            CacheEntry entry = null;
            if (_cache.TryGetValue(qname + qtype, out entry))
            {
                // Cache entry has expired.
                if (DateTime.Now > entry.Expires)
                {
                    return null;
                }

                return entry.Response;
            }

            return null;
        }

        /// <summary>
        /// Cleans up any resources being used.
        /// </summary>
        internal void Dispose()
        {
            _cache = null;

            _timerTimeout.Dispose();
            _timerTimeout = null;
        }

        /// <summary>
        /// Is called when cache expired entries check timer triggers.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">Event data.</param>
        private void TimerTimeout_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            lock (_cache)
            {
                // Copy entries to new array.
                var values = new List<KeyValuePair<string, CacheEntry>>();
                foreach (KeyValuePair<string, CacheEntry> entry in _cache)
                {
                    values.Add(entry);
                }

                // Remove expired cache entries.
                foreach (KeyValuePair<string, CacheEntry> entry in values)
                {
                    if (DateTime.Now > entry.Value.Expires)
                    {
                        _cache.Remove(entry.Key);
                    }
                }
            }
        }
        /// <summary>
        /// This class represents DNS cache entry.
        /// </summary>
        private class CacheEntry
        {
            /// <summary>
            /// Default constructor.
            /// </summary>
            /// <param name="response">DNS server response.</param>
            /// <param name="expires">Time when cache entry expires.</param>
            /// <exception cref="ArgumentNullException">Is raised when <b>response</b> is null reference.</exception>
            public CacheEntry(DnsServerResponse response, DateTime expires)
            {
                Response = response ?? throw new ArgumentNullException("response");
                Expires = expires;
            }

            /// <summary>
            /// Gets DNS server response.
            /// </summary>
            public DnsServerResponse Response { get; }

            /// <summary>
            /// Gets time when cache entry expires.
            /// </summary>
            public DateTime Expires { get; }
        }
    }
}
