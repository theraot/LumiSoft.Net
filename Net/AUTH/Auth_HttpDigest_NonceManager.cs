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
        private int m_ExpireTime = 30;

        private List<NonceEntry> m_pNonces;
        private Timer m_pTimer;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Auth_HttpDigest_NonceManager()
        {
            m_pNonces = new List<NonceEntry>();

            m_pTimer = new Timer(15000);
            m_pTimer.Elapsed += new ElapsedEventHandler(m_pTimer_Elapsed);
            m_pTimer.Enabled = true;
        }

        /// <summary>
        /// Gets or sets nonce expire time in seconds.
        /// </summary>
        public int ExpireTime
        {
            get { return m_ExpireTime; }

            set
            {
                if (value < 5)
                {
                    throw new ArgumentException("Property ExpireTime value must be >= 5 !");
                }

                m_ExpireTime = value;
            }
        }

        /// <summary>
        /// Creates new nonce and adds it to active nonces collection.
        /// </summary>
        /// <returns>Returns new created nonce.</returns>
        public string CreateNonce()
        {
            var nonce = Guid.NewGuid().ToString().Replace("-", "");
            m_pNonces.Add(new NonceEntry(nonce));

            return nonce;
        }

        /// <summary>
        /// Cleans up nay resource being used.
        /// </summary>
        public void Dispose()
        {
            if (m_pNonces == null)
            {
                m_pNonces.Clear();
                m_pNonces = null;
            }

            if (m_pTimer != null)
            {
                m_pTimer.Dispose();
                m_pTimer = null;
            }
        }

        /// <summary>
        /// Checks if specified nonce exists in active nonces collection.
        /// </summary>
        /// <param name="nonce">Nonce to check.</param>
        /// <returns>Returns true if nonce exists in active nonces collection, otherwise returns false.</returns>
        public bool NonceExists(string nonce)
        {
            lock (m_pNonces)
            {
                foreach (NonceEntry e in m_pNonces)
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
            lock (m_pNonces)
            {
                for (int i = 0; i < m_pNonces.Count; i++)
                {
                    if (m_pNonces[i].Nonce == nonce)
                    {
                        m_pNonces.RemoveAt(i);
                        i--;
                    }
                }
            }
        }

        private void m_pTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            RemoveExpiredNonces();
        }

        /// <summary>
        /// Removes not used nonces what has expired.
        /// </summary>
        private void RemoveExpiredNonces()
        {
            lock (m_pNonces)
            {
                for (int i = 0; i < m_pNonces.Count; i++)
                {
                    // Nonce expired, remove it.
                    if (m_pNonces[i].CreateTime.AddSeconds(m_ExpireTime) > DateTime.Now)
                    {
                        m_pNonces.RemoveAt(i);
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
