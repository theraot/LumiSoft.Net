using System;
using System.Text;
using System.Net;
using System.Threading;

namespace LumiSoft.Net.DNS.Client
{
    /// <summary>
    /// This class represents DNS client transaction.
    /// </summary>
    public class DNS_ClientTransaction
    {
        private readonly object m_pLock = new object();
        private Dns_Client m_pOwner;
        private TimerEx m_pTimeoutTimer;
        private int m_ResponseCount;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="owner">Owner DNS client.</param>
        /// <param name="id">Transaction ID.</param>
        /// <param name="qtype">QTYPE value.</param>
        /// <param name="qname">QNAME value.</param>
        /// <param name="timeout">Timeout in milliseconds.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>owner</b> or <b>qname</b> is null reference.</exception>
        internal DNS_ClientTransaction(Dns_Client owner, int id, DNS_QType qtype, string qname, int timeout)
        {
            m_pOwner = owner ?? throw new ArgumentNullException("owner");
            ID = id;
            QName = qname ?? throw new ArgumentNullException("qname");
            QType = qtype;

            CreateTime = DateTime.Now;
            m_pTimeoutTimer = new TimerEx(timeout);
            m_pTimeoutTimer.Elapsed += new System.Timers.ElapsedEventHandler(m_pTimeoutTimer_Elapsed);
        }

        /// <summary>
        /// This event is raised when DNS transaction state has changed.
        /// </summary>
        public event EventHandler<EventArgs<DNS_ClientTransaction>> StateChanged;

        /// <summary>
        /// This event is raised when DNS transaction times out.
        /// </summary>
        public event EventHandler Timeout;

        /// <summary>
        /// Gets transaction create time.
        /// </summary>
        public DateTime CreateTime { get; }

        /// <summary>
        /// Gets DNS transaction ID.
        /// </summary>
        public int ID { get; } = 1;

        /// <summary>
        /// Gets QNAME value.
        /// </summary>
        public string QName { get; } = "";

        /// <summary>
        /// Gets QTYPE value.
        /// </summary>
        public DNS_QType QType { get; } = 0;

        /// <summary>
        /// Gets DNS server response. Value null means no response received yet.
        /// </summary>
        public DnsServerResponse Response { get; private set; }

        /// <summary>
        /// Get DNS transaction state.
        /// </summary>
        public DNS_ClientTransactionState State { get; private set; } = DNS_ClientTransactionState.WaitingForStart;

        /// <summary>
        /// Cleans up any resource being used.
        /// </summary>
        public void Dispose()
        {
            lock (m_pLock)
            {
                if (State == DNS_ClientTransactionState.Disposed)
                {
                    return;
                }

                SetState(DNS_ClientTransactionState.Disposed);

                m_pTimeoutTimer.Dispose();
                m_pTimeoutTimer = null;

                m_pOwner = null;

                Response = null;

                StateChanged = null;
                Timeout = null;
            }
        }

        /// <summary>
        /// Starts DNS transaction processing.
        /// </summary>
        /// <exception cref="InvalidOperationException">Is raised when this method is called in invalid transaction state.</exception>
        public void Start()
        {
            if (State != DNS_ClientTransactionState.WaitingForStart)
            {
                throw new InvalidOperationException("DNS_ClientTransaction.Start may be called only in 'WaitingForStart' transaction state.");
            }

            SetState(DNS_ClientTransactionState.Active);

            // Move processing to thread pool.
            ThreadPool.QueueUserWorkItem(delegate (object state)
            {
                try
                {
                    // Use DNS cache if allowed.
                    if (Dns_Client.UseDnsCache)
                    {
                        var response = m_pOwner.Cache.GetFromCache(QName, (int)QType);
                        if (response != null)
                        {
                            Response = response;

                            SetState(DNS_ClientTransactionState.Completed);
                            Dispose();

                            return;
                        }
                    }

                    var buffer = new byte[1400];
                    int count = CreateQuery(buffer, ID, QName, QType, 1);

                    // Send parallel query to DNS server(s).
                    foreach (string server in Dns_Client.DnsServers)
                    {
                        if (Net_Utils.IsIPAddress(server))
                        {
                            var ip = IPAddress.Parse(server);
                            m_pOwner.Send(ip, buffer, count);
                        }
                    }

                    m_pTimeoutTimer.Start();
                }
                catch
                {
                    Dispose();
                }
            });
        }

        /// <summary>
        /// Processes DNS server response through this transaction.
        /// </summary>
        /// <param name="response">DNS server response.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>response</b> is null reference.</exception>
        internal void ProcessResponse(DnsServerResponse response)
        {
            if (response == null)
            {
                throw new ArgumentNullException("response");
            }

            try
            {
                lock (m_pLock)
                {
                    if (State != DNS_ClientTransactionState.Active)
                    {
                        return;
                    }
                    m_ResponseCount++;

                    // Late arriving response or retransmitted response, just skip it.
                    if (Response != null)
                    {
                        return;
                    }
                    // If server refused to complete query and we more active queries to other servers, skip that response.
                    if (response.ResponseCode == DNS_RCode.REFUSED && m_ResponseCount < Dns_Client.DnsServers.Length)
                    {
                        return;
                    }

                    Response = response;

                    SetState(DNS_ClientTransactionState.Completed);
                }
            }
            finally
            {
                if (State == DNS_ClientTransactionState.Completed)
                {
                    Dispose();
                }
            }
        }

        /// <summary>
        /// Creates binary query.
        /// </summary>
        /// <param name="buffer">Buffer where to store query.</param>
        /// <param name="ID">Query ID.</param>
        /// <param name="qname">Query text.</param>
        /// <param name="qtype">Query type.</param>
        /// <param name="qclass">Query class.</param>
        /// <returns>Returns number of bytes stored to <b>buffer</b>.</returns>
        private int CreateQuery(byte[] buffer, int ID, string qname, DNS_QType qtype, int qclass)
        {
            //---- Create header --------------------------------------------//
            // Header is first 12 bytes of query

            /* 4.1.1. Header section format
                                          1  1  1  1  1  1
            0  1  2  3  4  5  6  7  8  9  0  1  2  3  4  5
            +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
            |                      ID                       |
            +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
            |QR|   Opcode  |AA|TC|RD|RA|   Z    |   RCODE   |
            +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
            |                    QDCOUNT                    |
            +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
            |                    ANCOUNT                    |
            +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
            |                    NSCOUNT                    |
            +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
            |                    ARCOUNT                    |
            +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
            
            QR  A one bit field that specifies whether this message is a
                query (0), or a response (1).
                
            OPCODE          A four bit field that specifies kind of query in this
                message.  This value is set by the originator of a query
                and copied into the response.  The values are:

                0               a standard query (QUERY)

                1               an inverse query (IQUERY)

                2               a server status request (STATUS)
                
            */

            //--------- Header part -----------------------------------//
            buffer[0] = (byte)(ID >> 8); buffer[1] = (byte)(ID & 0xFF);
            buffer[2] = (byte)1; buffer[3] = (byte)0;
            buffer[4] = (byte)0; buffer[5] = (byte)1;
            buffer[6] = (byte)0; buffer[7] = (byte)0;
            buffer[8] = (byte)0; buffer[9] = (byte)0;
            buffer[10] = (byte)0; buffer[11] = (byte)0;
            //---------------------------------------------------------//

            //---- End of header --------------------------------------------//

            //----Create query ------------------------------------//

            /* Rfc 1035 4.1.2. Question section format
                                              1  1  1  1  1  1
            0  1  2  3  4  5  6  7  8  9  0  1  2  3  4  5
            +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
            |                                               |
            /                     QNAME                     /
            /                                               /
            +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
            |                     QTYPE                     |
            +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
            |                     QCLASS                    |
            +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
            
            QNAME
                a domain name represented as a sequence of labels, where
                each label consists of a length octet followed by that
                number of octets.  The domain name terminates with the
                zero length octet for the null label of the root.  Note
                that this field may be an odd number of octets; no
                padding is used.
            */

            // Convert unicode domain name. For more info see RFC 5890.
            var ldn = new System.Globalization.IdnMapping();
            qname = ldn.GetAscii(qname);

            var labels = qname.Split(new[] { '.' });
            int position = 12;

            // Copy all domain parts(labels) to query
            // eg. lumisoft.ee = 2 labels, lumisoft and ee.
            // format = label.length + label(bytes)
            foreach (string label in labels)
            {
                // convert label string to byte array
                var b = Encoding.ASCII.GetBytes(label);

                // add label lenght to query
                buffer[position++] = (byte)(b.Length);
                b.CopyTo(buffer, position);

                // Move position by label length
                position += b.Length;
            }

            // Terminate domain (see note above)
            buffer[position++] = (byte)0;

            // Set QTYPE
            buffer[position++] = (byte)0;
            buffer[position++] = (byte)qtype;

            // Set QCLASS
            buffer[position++] = (byte)0;
            buffer[position++] = (byte)qclass;
            //-------------------------------------------------------//

            return position;
        }

        /// <summary>
        /// Is called when DNS transaction timeout timer triggers.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">Event data.</param>
        private void m_pTimeoutTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                OnTimeout();
            }
            catch
            {
                // We don't care about errors here.
            }
            finally
            {
                Dispose();
            }
        }

        /// <summary>
        /// Raises <b>StateChanged</b> event.
        /// </summary>
        private void OnStateChanged()
        {
            if (StateChanged != null)
            {
                StateChanged(this, new EventArgs<DNS_ClientTransaction>(this));
            }
        }

        /// <summary>
        /// Raises <b>Timeout</b> event.
        /// </summary>
        private void OnTimeout()
        {
            if (Timeout != null)
            {
                Timeout(this, new EventArgs());
            }
        }

        /// <summary>
        /// Sets transaction state.
        /// </summary>
        /// <param name="state">New transaction state.</param>
        private void SetState(DNS_ClientTransactionState state)
        {
            State = state;

            OnStateChanged();
        }
    }
}
