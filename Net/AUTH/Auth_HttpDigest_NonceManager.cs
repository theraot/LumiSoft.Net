using System;
using System.Collections.Generic;
using System.Timers;

namespace LumiSoft.Net.AUTH
{
    /// <summary>
    /// HTTP digest authentication nonce manager.
    /// </summary>
    public class Auth_HttpDigest_NonceManager : IDisposable
    {
        private int _expireTime = 30;

        private List<NonceEntry> _nonces;
        private Timer _timer;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Auth_HttpDigest_NonceManager()
        {
            _nonces = new List<NonceEntry>();

            _timer = new Timer(15000);
            _timer.Elapsed += new ElapsedEventHandler(Timer_Elapsed);
            _timer.Enabled = true;
        }

        /// <summary>
        /// Gets or sets nonce expire time in seconds.
        /// </summary>
        public int ExpireTime
        {
            get => _expireTime;

            set
            {
                if (value < 5)
                {
                    throw new ArgumentException("Property ExpireTime value must be >= 5 !");
                }

                _expireTime = value;
            }
        }

        /// <summary>
        /// Creates new nonce and adds it to active nonces collection.
        /// </summary>
        /// <returns>Returns new created nonce.</returns>
        public string CreateNonce()
        {
            var nonce = Guid.NewGuid().ToString().Replace("-", "");
            _nonces.Add(new NonceEntry(nonce));

            return nonce;
        }

        /// <summary>
        /// Cleans up nay resource being used.
        /// </summary>
        public void Dispose()
        {
            if (_nonces == null)
            {
                _nonces.Clear();
                _nonces = null;
            }

            if (_timer != null)
            {
                _timer.Dispose();
                _timer = null;
            }
        }

        /// <summary>
        /// Checks if specified nonce exists in active nonces collection.
        /// </summary>
        /// <param name="nonce">Nonce to check.</param>
        /// <returns>Returns true if nonce exists in active nonces collection, otherwise returns false.</returns>
        public bool NonceExists(string nonce)
        {
            lock (_nonces)
            {
                foreach (NonceEntry e in _nonces)
                {
                    if (e.Nonce == nonce)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Removes specified nonce from active nonces collection.
        /// </summary>
        /// <param name="nonce">Nonce to remove.</param>
        public void RemoveNonce(string nonce)
        {
            lock (_nonces)
            {
                for (int i = 0; i < _nonces.Count; i++)
                {
                    if (_nonces[i].Nonce == nonce)
                    {
                        _nonces.RemoveAt(i);
                        i--;
                    }
                }
            }
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            RemoveExpiredNonces();
        }

        /// <summary>
        /// Removes not used nonces what has expired.
        /// </summary>
        private void RemoveExpiredNonces()
        {
            lock (_nonces)
            {
                for (int i = 0; i < _nonces.Count; i++)
                {
                    // Nonce expired, remove it.
                    if (_nonces[i].CreateTime.AddSeconds(_expireTime) > DateTime.Now)
                    {
                        _nonces.RemoveAt(i);
                        i--;
                    }
                }
            }
        }
        /// <summary>
        /// This class represents nonce entry in active nonces collection.
        /// </summary>
        private class NonceEntry
        {
            /// <summary>
            /// Default constructor.
            /// </summary>
            /// <param name="nonce"></param>
            public NonceEntry(string nonce)
            {
                Nonce = nonce;
                CreateTime = DateTime.Now;
            }

            /// <summary>
            /// Gets nonce value.
            /// </summary>
            public string Nonce { get; } = "";

            /// <summary>
            /// Gets time when this nonce entry was created.
            /// </summary>
            public DateTime CreateTime { get; }
        }
    }
}
