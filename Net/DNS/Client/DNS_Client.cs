using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Text;
using System.Net.NetworkInformation;
using System.Threading;
using LumiSoft.Net.UDP;

namespace LumiSoft.Net.DNS.Client
{
    /// <summary>
    /// DNS client.
    /// </summary>
    /// <example>
    /// <code>
    /// // Optionally set dns servers, by default DNS client uses defaultt NIC DNS servers.
    /// Dns_Client.DnsServers = new string[]{"194.126.115.18"};
    ///
    /// Dns_Client dns = Dns_Client.Static;
    ///
    /// // Get MX records.
    /// DnsServerResponse resp = dns.Query("lumisoft.ee",QTYPE.MX);
    /// if(resp.ConnectionOk &amp;&amp; resp.ResponseCode == RCODE.NO_ERROR){
    ///  MX_Record[] mxRecords = resp.GetMXRecords();
    ///
    ///  // Do your stuff
    /// }
    /// else{
    ///  // Handle error there, for more exact error info see RCODE
    /// }
    ///
    /// </code>
    /// </example>
    public class Dns_Client : IDisposable
    {
        private static IPAddress[] _dnsServers;
        private static Dns_Client _dnsClient;

        //
        private bool _isDisposed;
        private Socket _ipv4Socket;
        private Socket _ipv6Socket;
        private Random _random;
        private List<UDP_DataReceiver> _receivers;
        private Dictionary<int, DNS_ClientTransaction> _transactions;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Dns_Client()
        {
            _transactions = new Dictionary<int, DNS_ClientTransaction>();

            _ipv4Socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            _ipv4Socket.Bind(new IPEndPoint(IPAddress.Any, 0));

            if (Socket.OSSupportsIPv6)
            {
                _ipv6Socket = new Socket(AddressFamily.InterNetworkV6, SocketType.Dgram, ProtocolType.Udp);
                _ipv6Socket.Bind(new IPEndPoint(IPAddress.IPv6Any, 0));
            }

            _receivers = new List<UDP_DataReceiver>();
            _random = new Random();
            Cache = new DNS_ClientCache();

            // Create UDP data receivers.
            for (int i = 0; i < 5; i++)
            {
                var ipv4Receiver = new UDP_DataReceiver(_ipv4Socket);
                ipv4Receiver.PacketReceived += delegate (object s1, UDP_e_PacketReceived e1)
                {
                    ProcessUdpPacket(e1);
                };
                _receivers.Add(ipv4Receiver);
                ipv4Receiver.Start();

                if (_ipv6Socket != null)
                {
                    var ipv6Receiver = new UDP_DataReceiver(_ipv6Socket);
                    ipv6Receiver.PacketReceived += delegate (object s1, UDP_e_PacketReceived e1)
                    {
                        ProcessUdpPacket(e1);
                    };
                    _receivers.Add(ipv6Receiver);
                    ipv6Receiver.Start();
                }
            }
        }

        /// <summary>
        /// Static constructor.
        /// </summary>
        static Dns_Client()
        {
            // Try to get default NIC dns servers.
            try
            {
                var dnsServers = new List<IPAddress>();
                foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
                {
                    if (nic.OperationalStatus == OperationalStatus.Up)
                    {
                        foreach (IPAddress ip in nic.GetIPProperties().DnsAddresses)
                        {
                            if (ip.AddressFamily == AddressFamily.InterNetwork)
                            {
                                if (!dnsServers.Contains(ip))
                                {
                                    dnsServers.Add(ip);
                                }
                            }
                        }

                        break;
                    }
                }

                _dnsServers = dnsServers.ToArray();
            }
            catch
            {
            }
        }

        /// <summary>
        /// Gets or sets dns servers.
        /// </summary>
        /// <exception cref="ArgumentNullException">Is raised when null value is passed.</exception>
        public static string[] DnsServers
        {
            get
            {
                var retVal = new string[_dnsServers.Length];
                for (int i = 0; i < _dnsServers.Length; i++)
                {
                    retVal[i] = _dnsServers[i].ToString();
                }

                return retVal;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException();
                }

                var retVal = new IPAddress[value.Length];
                for (int i = 0; i < value.Length; i++)
                {
                    retVal[i] = IPAddress.Parse(value[i]);
                }

                _dnsServers = retVal;
            }
        }

        /// <summary>
        /// Gets static DNS client.
        /// </summary>
        public static Dns_Client Static
        {
            get
            {
                if (_dnsClient == null)
                {
                    _dnsClient = new Dns_Client();
                }

                return _dnsClient;
            }
        }

        /// <summary>
        /// Gets or sets if to use dns caching.
        /// </summary>
        public static bool UseDnsCache { get; set; } = true;

        /// <summary>
        /// Gets DNS cache.
        /// </summary>
        public DNS_ClientCache Cache { get; private set; }

        //--- OBSOLETE --------------------

        /// <summary>
        /// Resolves host names to IP addresses.
        /// </summary>
        /// <param name="hosts">Host names to resolve.</param>
        /// <returns>Returns specified hosts IP addresses.</returns>
        /// <exception cref="ArgumentNullException">Is raised when <b>hosts</b> is null.</exception>
        [Obsolete("Use Dns_Client.GetHostAddresses instead.")]
        public static IPAddress[] Resolve(string[] hosts)
        {
            if (hosts == null)
            {
                throw new ArgumentNullException("hosts");
            }

            var retVal = new List<IPAddress>();
            foreach (string host in hosts)
            {
                var addresses = Resolve(host);
                foreach (IPAddress ip in addresses)
                {
                    if (!retVal.Contains(ip))
                    {
                        retVal.Add(ip);
                    }
                }
            }

            return retVal.ToArray();
        }

        /// <summary>
        /// Resolves host name to IP addresses.
        /// </summary>
        /// <param name="host">Host name or IP address.</param>
        /// <returns>Return specified host IP addresses.</returns>
        /// <exception cref="ArgumentNullException">Is raised when <b>host</b> is null.</exception>
        [Obsolete("Use Dns_Client.GetHostAddresses instead.")]
        public static IPAddress[] Resolve(string host)
        {
            if (host == null)
            {
                throw new ArgumentNullException("host");
            }

            // If hostName_IP is IP
            try
            {
                return new[] { IPAddress.Parse(host) };
            }
            catch
            {
            }

            // This is probably NetBios name
            if (host.IndexOf(".") == -1)
            {
                return Dns.GetHostEntry(host).AddressList;
            }

            // hostName_IP must be host name, try to resolve it's IP
            using (Dns_Client dns = new Dns_Client())
            {
                var resp = dns.Query(host, DNS_QType.A);
                if (resp.ResponseCode == DNS_RCode.NO_ERROR)
                {
                    var records = resp.GetARecords();
                    var retVal = new IPAddress[records.Length];
                    for (int i = 0; i < records.Length; i++)
                    {
                        retVal[i] = records[i].IP;
                    }

                    return retVal;
                }

                throw new Exception(resp.ResponseCode.ToString());
            }
        }

        /// <summary>
        /// Creates new DNS client transaction.
        /// </summary>
        /// <param name="queryType">Query type.</param>
        /// <param name="queryText">Query text. It depends on queryType.</param>
        /// <param name="timeout">Transaction timeout in milliseconds. DNS default value is 2000, value 0 means no timeout - this is not suggested.</param>
        /// <returns>Returns DNS client transaction.</returns>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and and this method is accessed.</exception>
        /// <exception cref="ArgumentNullException">Is raised when <b>queryText</b> is null reference.</exception>
        /// <exception cref="ArgumentException">Is raised when any of the arguments has invalid value.</exception>
        /// <remarks>Creates asynchronous(non-blocking) DNS transaction. Call <see cref="DNS_ClientTransaction.Start"/> to start transaction.
        /// It is allowd to create multiple conccurent transactions.</remarks>
        public DNS_ClientTransaction CreateTransaction(DNS_QType queryType, string queryText, int timeout)
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
            if (queryText == null)
            {
                throw new ArgumentNullException("queryText");
            }
            if (queryText == string.Empty)
            {
                throw new ArgumentException("Argument 'queryText' value may not be \"\".", "queryText");
            }
            if (queryType == DNS_QType.PTR)
            {
                IPAddress ip = null;
                if (!IPAddress.TryParse(queryText, out ip))
                {
                    throw new ArgumentException("Argument 'queryText' value must be IP address if queryType == DNS_QType.PTR.", "queryText");
                }
            }

            if (queryType == DNS_QType.PTR)
            {
                var ip = queryText;

                // See if IP is ok.
                var ipA = IPAddress.Parse(ip);
                queryText = "";

                // IPv6
                if (ipA.AddressFamily == AddressFamily.InterNetworkV6)
                {
                    // 4321:0:1:2:3:4:567:89ab
                    // would be
                    // b.a.9.8.7.6.5.0.4.0.0.0.3.0.0.0.2.0.0.0.1.0.0.0.0.0.0.0.1.2.3.4.IP6.ARPA

                    var ipChars = ip.Replace(":", "").ToCharArray();
                    for (int i = ipChars.Length - 1; i > -1; i--)
                    {
                        queryText += ipChars[i] + ".";
                    }
                    queryText += "IP6.ARPA";
                }
                // IPv4
                else
                {
                    // 213.35.221.186
                    // would be
                    // 186.221.35.213.in-addr.arpa

                    var ipParts = ip.Split('.');
                    //--- Reverse IP ----------
                    for (int i = 3; i > -1; i--)
                    {
                        queryText += ipParts[i] + ".";
                    }
                    queryText += "in-addr.arpa";
                }
            }

            // Create transaction ID.
            int transactionID = 0;
            lock (_transactions)
            {
                while (true)
                {
                    transactionID = _random.Next(0xFFFF);

                    // We got not used transaction ID.
                    if (!_transactions.ContainsKey(transactionID))
                    {
                        break;
                    }
                }
            }

            var retVal = new DNS_ClientTransaction(this, transactionID, queryType, queryText, timeout);
            retVal.StateChanged += delegate (object s1, EventArgs<DNS_ClientTransaction> e1)
            {
                if (retVal.State == DNS_ClientTransactionState.Disposed)
                {
                    lock (_transactions)
                    {
                        _transactions.Remove(e1.Value.ID);
                    }
                }
            };
            lock (_transactions)
            {
                _transactions.Add(retVal.ID, retVal);
            }

            return retVal;
        }

        /// <summary>
        /// Cleans up any resources being used.
        /// </summary>
        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }
            _isDisposed = true;

            if (_receivers != null)
            {
                foreach (UDP_DataReceiver receiver in _receivers)
                {
                    receiver.Dispose();
                }
                _receivers = null;
            }

            _ipv4Socket.Close();
            _ipv4Socket = null;

            if (_ipv6Socket != null)
            {
                _ipv6Socket.Close();
                _ipv6Socket = null;
            }

            _transactions = null;

            _random = null;

            Cache.Dispose();
            Cache = null;
        }

        /// <summary>
        /// Gets email hosts.
        /// </summary>
        /// <param name="domain">Email domain. For example: 'domain.com'.</param>
        /// <returns>Returns email hosts in priority order.</returns>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and and this method is accessed.</exception>
        /// <exception cref="ArgumentNullException">Is raised when <b>domain</b> is null reference.</exception>
        /// <exception cref="ArgumentException">Is raised when any of the arguments has invalid value.</exception>
        /// <exception cref="DNS_ClientException">Is raised when DNS server returns error.</exception>
        /// <exception cref="IOException">Is raised when IO reletaed error happens.</exception>
        public HostEntry[] GetEmailHosts(string domain)
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
            if (domain == null)
            {
                throw new ArgumentNullException("domain");
            }
            if (domain == string.Empty)
            {
                throw new ArgumentException("Argument 'domain' value must be specified.", "domain");
            }

            var wait = new ManualResetEvent(false);
            using (GetEmailHostsAsyncOP op = new GetEmailHostsAsyncOP(domain))
            {
                op.CompletedAsync += delegate (object s1, EventArgs<GetEmailHostsAsyncOP> e1)
                {
                    wait.Set();
                };
                if (!GetEmailHostsAsync(op))
                {
                    wait.Set();
                }
                wait.WaitOne();
                wait.Close();

                if (op.Error != null)
                {
                    throw op.Error;
                }

                return op.Hosts;
            }
        }

        /// <summary>
        /// Starts getting email hosts.
        /// </summary>
        /// <param name="op">Asynchronous operation.</param>
        /// <returns>Returns true if aynchronous operation is pending (The <see cref="GetEmailHostsAsyncOP.CompletedAsync"/> event is raised upon completion of the operation).
        /// Returns false if operation completed synchronously.</returns>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and and this method is accessed.</exception>
        /// <exception cref="ArgumentNullException">Is raised when <b>op</b> is null reference.</exception>
        public bool GetEmailHostsAsync(GetEmailHostsAsyncOP op)
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
            if (op == null)
            {
                throw new ArgumentNullException("op");
            }
            if (op.State != AsyncOP_State.WaitingForStart)
            {
                throw new ArgumentException("Invalid argument 'op' state, 'op' must be in 'AsyncOP_State.WaitingForStart' state.", "op");
            }

            return op.Start(this);
        }

        /// <summary>
        /// Gets host IPv4 and IPv6 addresses.
        /// </summary>
        /// <param name="hostNameOrIP">Host name or IP address.</param>
        /// <returns>Returns host IPv4 and IPv6 addresses.</returns>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and and this method is accessed.</exception>
        /// <exception cref="ArgumentNullException">Is raised when <b>hostNameOrIP</b> is null reference.</exception>
        /// <exception cref="DNS_ClientException">Is raised when DNS server returns error.</exception>
        /// <exception cref="IOException">Is raised when IO reletaed error happens.</exception>
        public IPAddress[] GetHostAddresses(string hostNameOrIP)
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
            if (hostNameOrIP == null)
            {
                throw new ArgumentNullException("hostNameOrIP");
            }

            var wait = new ManualResetEvent(false);
            using (GetHostAddressesAsyncOP op = new GetHostAddressesAsyncOP(hostNameOrIP))
            {
                op.CompletedAsync += delegate (object s1, EventArgs<GetHostAddressesAsyncOP> e1)
                {
                    wait.Set();
                };
                if (!GetHostAddressesAsync(op))
                {
                    wait.Set();
                }
                wait.WaitOne();
                wait.Close();

                if (op.Error != null)
                {
                    throw op.Error;
                }

                return op.Addresses;
            }
        }

        /// <summary>
        /// Starts resolving host IPv4 and IPv6 addresses.
        /// </summary>
        /// <param name="op">Asynchronous operation.</param>
        /// <returns>Returns true if aynchronous operation is pending (The <see cref="GetHostAddressesAsyncOP.CompletedAsync"/> event is raised upon completion of the operation).
        /// Returns false if operation completed synchronously.</returns>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and and this method is accessed.</exception>
        /// <exception cref="ArgumentNullException">Is raised when <b>op</b> is null reference.</exception>
        public bool GetHostAddressesAsync(GetHostAddressesAsyncOP op)
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
            if (op == null)
            {
                throw new ArgumentNullException("op");
            }
            if (op.State != AsyncOP_State.WaitingForStart)
            {
                throw new ArgumentException("Invalid argument 'op' state, 'op' must be in 'AsyncOP_State.WaitingForStart' state.", "op");
            }

            return op.Start(this);
        }

        /// <summary>
        /// Resolving multiple host IPv4 and IPv6 addresses.
        /// </summary>
        /// <param name="hostNames">Host names to resolve.</param>
        /// <returns>Returns host entries.</returns>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and and this method is accessed.</exception>
        /// <exception cref="ArgumentNullException">Is raised when <b>hostNames</b> is null reference.</exception>
        /// <exception cref="DNS_ClientException">Is raised when DNS server returns error.</exception>
        /// <exception cref="IOException">Is raised when IO reletaed error happens.</exception>
        public HostEntry[] GetHostsAddresses(string[] hostNames)
        {
            return GetHostsAddresses(hostNames, false);
        }

        /// <summary>
        /// Resolving multiple host IPv4 and IPv6 addresses.
        /// </summary>
        /// <param name="hostNames">Host names to resolve.</param>
        /// <param name="resolveAny">If true, as long as one host name is resolved, no error returned.</param>
        /// <returns>Returns host entries.</returns>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and and this method is accessed.</exception>
        /// <exception cref="ArgumentNullException">Is raised when <b>hostNames</b> is null reference.</exception>
        /// <exception cref="DNS_ClientException">Is raised when DNS server returns error.</exception>
        /// <exception cref="IOException">Is raised when IO reletaed error happens.</exception>
        public HostEntry[] GetHostsAddresses(string[] hostNames, bool resolveAny)
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
            if (hostNames == null)
            {
                throw new ArgumentNullException("hostNames");
            }

            var wait = new ManualResetEvent(false);
            using (GetHostsAddressesAsyncOP op = new GetHostsAddressesAsyncOP(hostNames, resolveAny))
            {
                op.CompletedAsync += delegate (object s1, EventArgs<GetHostsAddressesAsyncOP> e1)
                {
                    wait.Set();
                };
                if (!GetHostsAddressesAsync(op))
                {
                    wait.Set();
                }
                wait.WaitOne();
                wait.Close();

                if (op.Error != null)
                {
                    throw op.Error;
                }

                return op.HostEntries;
            }
        }

        /// <summary>
        /// Starts resolving multiple host IPv4 and IPv6 addresses.
        /// </summary>
        /// <param name="op">Asynchronous operation.</param>
        /// <returns>Returns true if aynchronous operation is pending (The <see cref="GetHostsAddressesAsyncOP.CompletedAsync"/> event is raised upon completion of the operation).
        /// Returns false if operation completed synchronously.</returns>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and and this method is accessed.</exception>
        /// <exception cref="ArgumentNullException">Is raised when <b>op</b> is null reference.</exception>
        public bool GetHostsAddressesAsync(GetHostsAddressesAsyncOP op)
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
            if (op == null)
            {
                throw new ArgumentNullException("op");
            }
            if (op.State != AsyncOP_State.WaitingForStart)
            {
                throw new ArgumentException("Invalid argument 'op' state, 'op' must be in 'AsyncOP_State.WaitingForStart' state.", "op");
            }

            return op.Start(this);
        }

        /// <summary>
        /// Queries server with specified query.
        /// </summary>
        /// <param name="queryText">Query text. It depends on queryType.</param>
        /// <param name="queryType">Query type.</param>
        /// <returns>Returns DSN server response.</returns>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and and this method is accessed.</exception>
        /// <exception cref="ArgumentNullException">Is raised when <b>queryText</b> is null.</exception>
        public DnsServerResponse Query(string queryText, DNS_QType queryType)
        {
            return Query(queryText, queryType, 2000);
        }

        /// <summary>
        /// Queries server with specified query.
        /// </summary>
        /// <param name="queryText">Query text. It depends on queryType.</param>
        /// <param name="queryType">Query type.</param>
        /// <param name="timeout">Query timeout in milli seconds.</param>
        /// <returns>Returns DSN server response.</returns>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and and this method is accessed.</exception>
        /// <exception cref="ArgumentNullException">Is raised when <b>queryText</b> is null.</exception>
        public DnsServerResponse Query(string queryText, DNS_QType queryType, int timeout)
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
            if (queryText == null)
            {
                throw new ArgumentNullException("queryText");
            }

            DnsServerResponse retVal = null;
            var wait = new ManualResetEvent(false);

            var transaction = CreateTransaction(queryType, queryText, timeout);
            transaction.Timeout += delegate (object s, EventArgs e)
            {
                wait.Set();
            };
            transaction.StateChanged += delegate (object s1, EventArgs<DNS_ClientTransaction> e1)
            {
                if (transaction.State == DNS_ClientTransactionState.Completed || transaction.State == DNS_ClientTransactionState.Disposed)
                {
                    retVal = transaction.Response;

                    wait.Set();
                }
            };
            transaction.Start();

            // Wait transaction to complete.
            wait.WaitOne();
            wait.Close();

            return retVal;
        }

        internal static bool GetQName(byte[] reply, ref int offset, ref string name)
        {
            bool retVal = GetQNameI(reply, ref offset, ref name);

            // Convert domain name to unicode. For more info see RFC 5890.
            var ldn = new System.Globalization.IdnMapping();
            name = ldn.GetUnicode(name);

            return retVal;
        }

        /// <summary>
        /// Reads character-string from spefcified data and offset.
        /// </summary>
        /// <param name="data">Data from where to read.</param>
        /// <param name="offset">Offset from where to start reading.</param>
        /// <returns>Returns readed string.</returns>
        internal static string ReadCharacterString(byte[] data, ref int offset)
        {
            /* RFC 1035 3.3.
                <character-string> is a single length octet followed by that number of characters. 
                <character-string> is treated as binary information, and can be up to 256 characters 
                in length (including the length octet).
            */

            int dataLength = (int)data[offset++];
            var retVal = Encoding.Default.GetString(data, offset, dataLength);
            offset += dataLength;

            return retVal;
        }

        /// <summary>
        /// Sends specified packet to the specified target IP end point.
        /// </summary>
        /// <param name="target">Target end point.</param>
        /// <param name="packet">Packet to send.</param>
        /// <param name="count">Number of bytes to send from <b>packet</b>.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>target</b> or <b>packet</b> is null reference.</exception>
        internal void Send(IPAddress target, byte[] packet, int count)
        {
            if (target == null)
            {
                throw new ArgumentNullException("target");
            }
            if (packet == null)
            {
                throw new ArgumentNullException("packet");
            }

            try
            {
                if (target.AddressFamily == AddressFamily.InterNetwork)
                {
                    _ipv4Socket.SendTo(packet, count, SocketFlags.None, new IPEndPoint(target, 53));
                }
                else if (target.AddressFamily == AddressFamily.InterNetworkV6)
                {
                    _ipv6Socket.SendTo(packet, count, SocketFlags.None, new IPEndPoint(target, 53));
                }
            }
            catch
            {
            }
        }

        private static bool GetQNameI(byte[] reply, ref int offset, ref string name)
        {
            try
            {
                while (true)
                {
                    // Invalid DNS packet, offset goes beyound reply size, probably terminator missing.
                    if (offset >= reply.Length)
                    {
                        return false;
                    }
                    // We have label terminator "0".
                    if (reply[offset] == 0)
                    {
                        break;
                    }

                    // Check if it's pointer(In pointer first two bits always 1)
                    bool isPointer = ((reply[offset] & 0xC0) == 0xC0);

                    // If pointer
                    if (isPointer)
                    {
                        /* Pointer location number is 2 bytes long
                            0 | 1 | 2 | 3 | 4 | 5 | 6 | 7  # byte 2 # 0 | 1 | 2 | | 3 | 4 | 5 | 6 | 7
                            empty | < ---- pointer location number --------------------------------->
                        */
                        int pStart = ((reply[offset] & 0x3F) << 8) | (reply[++offset]);
                        offset++;

                        return GetQNameI(reply, ref pStart, ref name);
                    }

                    /* Label length (length = 8Bit and first 2 bits always 0)
                            0 | 1 | 2 | 3 | 4 | 5 | 6 | 7
                            empty | lablel length in bytes 
                        */
                    int labelLength = (reply[offset] & 0x3F);
                    offset++;

                    // Copy label into name
                    name += Encoding.UTF8.GetString(reply, offset, labelLength);
                    offset += labelLength;

                    // If the next char isn't terminator, label continues - add dot between two labels.
                    if (reply[offset] != 0)
                    {
                        name += ".";
                    }
                }

                // Move offset by terminator length.
                offset++;

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Parses specified count of answers from query.
        /// </summary>
        /// <param name="reply">Server returned query.</param>
        /// <param name="answerCount">Number of answers to parse.</param>
        /// <param name="offset">Position from where to start parsing answers.</param>
        /// <returns></returns>
        private List<DNS_rr> ParseAnswers(byte[] reply, int answerCount, ref int offset)
        {
            /* RFC 1035 4.1.3. Resource record format
             
                                           1  1  1  1  1  1
             0  1  2  3  4  5  6  7  8  9  0  1  2  3  4  5
            +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
            |                                               |
            /                                               /
            /                      NAME                     /
            |                                               |
            +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
            |                      TYPE                     |
            +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
            |                     CLASS                     |
            +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
            |                      TTL                      |
            |                                               |
            +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
            |                   RDLENGTH                    |
            +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--|
            /                     RDATA                     /
            /                                               /
            +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
            */

            var answers = new List<DNS_rr>();
            //---- Start parsing answers ------------------------------------------------------------------//
            for (int i = 0; i < answerCount; i++)
            {
                var name = "";
                if (!GetQName(reply, ref offset, ref name))
                {
                    break;
                }

                int type = reply[offset++] << 8 | reply[offset++];
                int rdClass = reply[offset++] << 8 | reply[offset++];
                int ttl = reply[offset++] << 24 | reply[offset++] << 16 | reply[offset++] << 8 | reply[offset++];
                int rdLength = reply[offset++] << 8 | reply[offset++];

                if ((DNS_QType)type == DNS_QType.A)
                {
                    answers.Add(DNS_rr_A.Parse(name, reply, ref offset, rdLength, ttl));
                }
                else if ((DNS_QType)type == DNS_QType.NS)
                {
                    answers.Add(DNS_rr_NS.Parse(name, reply, ref offset, rdLength, ttl));
                }
                else if ((DNS_QType)type == DNS_QType.CNAME)
                {
                    answers.Add(DNS_rr_CNAME.Parse(name, reply, ref offset, rdLength, ttl));
                }
                else if ((DNS_QType)type == DNS_QType.SOA)
                {
                    answers.Add(DNS_rr_SOA.Parse(name, reply, ref offset, rdLength, ttl));
                }
                else if ((DNS_QType)type == DNS_QType.PTR)
                {
                    answers.Add(DNS_rr_PTR.Parse(name, reply, ref offset, rdLength, ttl));
                }
                else if ((DNS_QType)type == DNS_QType.HINFO)
                {
                    answers.Add(DNS_rr_HINFO.Parse(name, reply, ref offset, rdLength, ttl));
                }
                else if ((DNS_QType)type == DNS_QType.MX)
                {
                    answers.Add(DNS_rr_MX.Parse(name, reply, ref offset, rdLength, ttl));
                }
                else if ((DNS_QType)type == DNS_QType.TXT)
                {
                    answers.Add(DNS_rr_TXT.Parse(name, reply, ref offset, rdLength, ttl));
                }
                else if ((DNS_QType)type == DNS_QType.AAAA)
                {
                    answers.Add(DNS_rr_AAAA.Parse(name, reply, ref offset, rdLength, ttl));
                }
                else if ((DNS_QType)type == DNS_QType.SRV)
                {
                    answers.Add(DNS_rr_SRV.Parse(name, reply, ref offset, rdLength, ttl));
                }
                else if ((DNS_QType)type == DNS_QType.NAPTR)
                {
                    answers.Add(DNS_rr_NAPTR.Parse(name, reply, ref offset, rdLength, ttl));
                }
                else if ((DNS_QType)type == DNS_QType.SPF)
                {
                    answers.Add(DNS_rr_SPF.Parse(name, reply, ref offset, rdLength, ttl));
                }
                else
                {
                    // Unknown record, skip it.
                    offset += rdLength;
                }
            }

            return answers;
        }

        /// <summary>
        /// Parses query.
        /// </summary>
        /// <param name="reply">Dns server reply.</param>
        /// <returns></returns>
        private DnsServerResponse ParseQuery(byte[] reply)
        {
            //--- Parse headers ------------------------------------//

            /* RFC 1035 4.1.1. Header section format
             
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
             
            QDCOUNT
                an unsigned 16 bit integer specifying the number of
                entries in the question section.

            ANCOUNT
                an unsigned 16 bit integer specifying the number of
                resource records in the answer section.
                
            NSCOUNT
                an unsigned 16 bit integer specifying the number of name
                server resource records in the authority records section.

            ARCOUNT
                an unsigned 16 bit integer specifying the number of
                resource records in the additional records section.
                
            */

            // Get reply code
            int id = (reply[0] << 8 | reply[1]);
            var opcode = (OPCODE)((reply[2] >> 3) & 15);
            var replyCode = (DNS_RCode)(reply[3] & 15);
            int queryCount = (reply[4] << 8 | reply[5]);
            int answerCount = (reply[6] << 8 | reply[7]);
            int authoritiveAnswerCount = (reply[8] << 8 | reply[9]);
            int additionalAnswerCount = (reply[10] << 8 | reply[11]);
            //---- End of headers ---------------------------------//

            int pos = 12;

            //----- Parse question part ------------//
            for (int q = 0; q < queryCount; q++)
            {
                var dummy = "";
                GetQName(reply, ref pos, ref dummy);
                //qtype + qclass
                pos += 4;
            }
            //--------------------------------------//

            // 1) parse answers
            // 2) parse authoritive answers
            // 3) parse additional answers
            var answers = ParseAnswers(reply, answerCount, ref pos);
            var authoritiveAnswers = ParseAnswers(reply, authoritiveAnswerCount, ref pos);
            var additionalAnswers = ParseAnswers(reply, additionalAnswerCount, ref pos);

            return new DnsServerResponse(true, id, replyCode, answers, authoritiveAnswers, additionalAnswers);
        }

        /// <summary>
        /// Processes received UDP packet.
        /// </summary>
        /// <param name="e">UDP packet.</param>
        private void ProcessUdpPacket(UDP_e_PacketReceived e)
        {
            try
            {
                if (_isDisposed)
                {
                    return;
                }

                var serverResponse = ParseQuery(e.Buffer);
                DNS_ClientTransaction transaction = null;
                // Pass response to transaction.
                if (_transactions.TryGetValue(serverResponse.ID, out transaction))
                {
                    if (transaction.State == DNS_ClientTransactionState.Active)
                    {
                        // Cache query.
                        if (UseDnsCache && serverResponse.ResponseCode == DNS_RCode.NO_ERROR)
                        {
                            Cache.AddToCache(transaction.QName, (int)transaction.QType, serverResponse);
                        }

                        transaction.ProcessResponse(serverResponse);
                    }
                }
                // No such transaction or transaction has timed out before answer received.
                //else{
                //}
            }
            catch
            {
                // We don't care about receiving errors here, skip them.
            }
        }

        /// <summary>
        /// This class represents <see cref="Dns_Client.GetEmailHostsAsync"/> asynchronous operation.
        /// </summary>
        public class GetEmailHostsAsyncOP : IDisposable, IAsyncOP
        {
            private readonly object _lock = new object();
            private Exception _exception;
            private string _domain;
            private HostEntry[] _hosts;
            private bool _riseCompleted;

            /// <summary>
            /// Default constructor.
            /// </summary>
            /// <param name="domain">Email domain. For example: 'domain.com'.</param>
            /// <exception cref="ArgumentNullException">Is raised when <b>domain</b> is null reference.</exception>
            /// <exception cref="ArgumentException">Is raised when any of the arguments has invalid value.</exception>
            public GetEmailHostsAsyncOP(string domain)
            {
                if (domain == null)
                {
                    throw new ArgumentNullException("domain");
                }
                if (domain == string.Empty)
                {
                    throw new ArgumentException("Argument 'domain' value must be specified.", "domain");
                }

                _domain = domain;

                // We have email address, parse domain.
                if (domain.IndexOf("@") > -1)
                {
                    _domain = domain.Split(new[] { '@' }, 2)[1];
                }
            }

            /// <summary>
            /// Cleans up any resource being used.
            /// </summary>
            public void Dispose()
            {
                if (State == AsyncOP_State.Disposed)
                {
                    return;
                }
                SetState(AsyncOP_State.Disposed);

                _exception = null;
                _domain = null;
                _hosts = null;

                CompletedAsync = null;
            }

            /// <summary>
            /// Starts operation processing.
            /// </summary>
            /// <param name="dnsClient">DNS client.</param>
            /// <returns>Returns true if asynchronous operation in progress or false if operation completed synchronously.</returns>
            /// <exception cref="ArgumentNullException">Is raised when <b>dnsClient</b> is null reference.</exception>
            internal bool Start(Dns_Client dnsClient)
            {
                if (dnsClient == null)
                {
                    throw new ArgumentNullException("dnsClient");
                }

                SetState(AsyncOP_State.Active);

                /* RFC 5321 5.
                    The lookup first attempts to locate an MX record associated with the
                    name.  If a CNAME record is found, the resulting name is processed as
                    if it were the initial name.
                 
                    If no MX records are found, but an A RR is found, the A RR is treated as if it 
                    was associated with an implicit MX RR, with a preference of 0, pointing to that host.
                */

                try
                {
                    LookupMX(dnsClient, _domain, false);
                }
                catch (Exception x)
                {
                    _exception = x;
                    SetState(AsyncOP_State.Completed);
                }

                // Set flag rise CompletedAsync event flag. The event is raised when async op completes.
                // If already completed sync, that flag has no effect.
                lock (_lock)
                {
                    _riseCompleted = true;

                    return State == AsyncOP_State.Active;
                }
            }

            /// <summary>
            /// Sets operation state.
            /// </summary>
            /// <param name="state">New state.</param>
            private void SetState(AsyncOP_State state)
            {
                if (State == AsyncOP_State.Disposed)
                {
                    return;
                }

                lock (_lock)
                {
                    State = state;

                    if (State == AsyncOP_State.Completed && _riseCompleted)
                    {
                        OnCompletedAsync();
                    }
                }
            }

            /// <summary>
            /// Starts looking up MX records for specified domain.
            /// </summary>
            /// <param name="dnsClient">DNS client.</param>
            /// <param name="domain">Domain name.</param>
            /// <param name="domainIsCName">If true domain name is CNAME(alias).</param>
            /// <exception cref="ArgumentNullException">Is riased when <b>dnsClient</b> or <b>domain</b> is null reference.</exception>
            private void LookupMX(Dns_Client dnsClient, string domain, bool domainIsCName)
            {
                if (dnsClient == null)
                {
                    throw new ArgumentNullException("dnsClient");
                }
                if (domain == null)
                {
                    throw new ArgumentNullException("domain");
                }

                // Try to get MX records.
                var transaction_MX = dnsClient.CreateTransaction(DNS_QType.MX, domain, 2000);
                transaction_MX.StateChanged += delegate (object s1, EventArgs<DNS_ClientTransaction> e1)
                {
                    try
                    {
                        if (e1.Value.State == DNS_ClientTransactionState.Completed)
                        {
                            // No errors.
                            if (e1.Value.Response.ResponseCode == DNS_RCode.NO_ERROR)
                            {
                                var mxRecords = new List<DNS_rr_MX>();
                                foreach (DNS_rr_MX mx in e1.Value.Response.GetMXRecords())
                                {
                                    // Skip invalid MX records.
                                    if (string.IsNullOrEmpty(mx.Host))
                                    {
                                    }
                                    else
                                    {
                                        mxRecords.Add(mx);
                                    }
                                }

                                // Use MX records.
                                if (mxRecords.Count > 0)
                                {
                                    _hosts = new HostEntry[mxRecords.Count];

                                    // Create name to index map, so we can map asynchronous A/AAAA lookup results back to MX priority index.
                                    var name_to_index_map = new Dictionary<string, int>();
                                    var lookupQueue = new List<string>();

                                    // Process MX records.
                                    for (int i = 0; i < _hosts.Length; i++)
                                    {
                                        var mx = mxRecords[i];

                                        var ips = Get_A_or_AAAA_FromResponse(mx.Host, e1.Value.Response);
                                        // No A or AAAA records in addtional answers section for MX, we need todo new query for that.
                                        if (ips.Length == 0)
                                        {
                                            name_to_index_map[mx.Host] = i;
                                            lookupQueue.Add(mx.Host);
                                        }
                                        else
                                        {
                                            _hosts[i] = new HostEntry(mx.Host, ips, null);
                                        }
                                    }

                                    // We have MX records which A or AAAA records not provided in DNS response, lookup them.
                                    if (lookupQueue.Count > 0)
                                    {
                                        var op = new GetHostsAddressesAsyncOP(lookupQueue.ToArray(), true);
                                        // This event is raised when lookup completes asynchronously.
                                        op.CompletedAsync += delegate (object s2, EventArgs<GetHostsAddressesAsyncOP> e2)
                                        {
                                            LookupCompleted(op, name_to_index_map);
                                        };
                                        // Lookup completed synchronously.
                                        if (!dnsClient.GetHostsAddressesAsync(op))
                                        {
                                            LookupCompleted(op, name_to_index_map);
                                        }
                                    }
                                    // All MX records resolved.
                                    else
                                    {
                                        SetState(AsyncOP_State.Completed);
                                    }
                                }
                                // Use CNAME as initial domain name.
                                else if (e1.Value.Response.GetCNAMERecords().Length > 0)
                                {
                                    if (domainIsCName)
                                    {
                                        _exception = new Exception("CNAME to CNAME loop dedected.");
                                        SetState(AsyncOP_State.Completed);
                                    }
                                    else
                                    {
                                        LookupMX(dnsClient, e1.Value.Response.GetCNAMERecords()[0].Alias, true);
                                    }
                                }
                                // Use domain name as MX.
                                else
                                {
                                    _hosts = new HostEntry[1];

                                    // Create name to index map, so we can map asynchronous A/AAAA lookup results back to MX priority index.
                                    var name_to_index_map = new Dictionary<string, int>();
                                    name_to_index_map.Add(domain, 0);

                                    var op = new GetHostsAddressesAsyncOP(new[] { domain });
                                    // This event is raised when lookup completes asynchronously.
                                    op.CompletedAsync += delegate (object s2, EventArgs<GetHostsAddressesAsyncOP> e2)
                                    {
                                        LookupCompleted(op, name_to_index_map);
                                    };
                                    // Lookup completed synchronously.
                                    if (!dnsClient.GetHostsAddressesAsync(op))
                                    {
                                        LookupCompleted(op, name_to_index_map);
                                    }
                                }
                            }
                            // DNS server returned error, just return error.
                            else
                            {
                                _exception = new DNS_ClientException(e1.Value.Response.ResponseCode);
                                SetState(AsyncOP_State.Completed);
                            }
                        }
                        transaction_MX.Timeout += delegate (object s2, EventArgs e2)
                        {
                            _exception = new IOException("DNS transaction timeout, no response from DNS server.");
                            SetState(AsyncOP_State.Completed);
                        };
                    }
                    catch (Exception x)
                    {
                        _exception = x;
                        SetState(AsyncOP_State.Completed);
                    }
                };
                transaction_MX.Start();
            }

            /// <summary>
            /// Gets A and AAAA records from DNS server additional responses section.
            /// </summary>
            /// <param name="name">Host name.</param>
            /// <param name="response">DNS server response.</param>
            /// <returns>Returns A and AAAA records from DNS server additional responses section.</returns>
            /// <exception cref="ArgumentNullException">Is raised when <b>name</b> or <b>response</b> is null reference.</exception>
            private IPAddress[] Get_A_or_AAAA_FromResponse(string name, DnsServerResponse response)
            {
                if (name == null)
                {
                    throw new ArgumentNullException("name");
                }
                if (response == null)
                {
                    throw new ArgumentNullException("response");
                }

                var aList = new List<IPAddress>();
                var aaaaList = new List<IPAddress>();

                foreach (DNS_rr rr in response.AdditionalAnswers)
                {
                    if (string.Equals(name, rr.Name, StringComparison.InvariantCultureIgnoreCase))
                    {
                        if (rr is DNS_rr_A)
                        {
                            aList.Add(((DNS_rr_A)rr).IP);
                        }
                        else if (rr is DNS_rr_AAAA)
                        {
                            aaaaList.Add(((DNS_rr_AAAA)rr).IP);
                        }
                    }
                }

                // We list IPv4 first and then IPv6 addresses.
                aList.AddRange(aaaaList);

                return aList.ToArray();
            }

            /// <summary>
            /// This method is called when A/AAAA lookup has completed.
            /// </summary>
            /// <param name="op">Asynchronous operation.</param>
            /// <param name="name_to_index">Dns name to index lookup table.</param>
            /// <exception cref="ArgumentNullException">Is raised when <b>op</b> or <b>name_to_index</b> is null reference value.</exception>
            private void LookupCompleted(GetHostsAddressesAsyncOP op, Dictionary<string, int> name_to_index)
            {
                if (op == null)
                {
                    throw new ArgumentNullException("op");
                }

                if (op.Error != null)
                {
                    // If we have any resolved DNS, we don't return error if any.
                    bool anyResolved = false;
                    foreach (HostEntry host in _hosts)
                    {
                        if (host != null)
                        {
                            anyResolved = true;

                            break;
                        }
                    }
                    if (!anyResolved)
                    {
                        _exception = op.Error;
                    }
                }
                else
                {
                    foreach (HostEntry host in op.HostEntries)
                    {
                        _hosts[name_to_index[host.HostName]] = host;
                    }
                }

                op.Dispose();

                // Remove unresolved DNS entries from response.
                var retVal = new List<HostEntry>();
                foreach (HostEntry host in _hosts)
                {
                    if (host != null)
                    {
                        retVal.Add(host);
                    }
                }
                _hosts = retVal.ToArray();

                SetState(AsyncOP_State.Completed);
            }

            /// <summary>
            /// Gets asynchronous operation state.
            /// </summary>
            public AsyncOP_State State { get; private set; } = AsyncOP_State.WaitingForStart;

            /// <summary>
            /// Gets error happened during operation. Returns null if no error.
            /// </summary>
            /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and and this property is accessed.</exception>
            /// <exception cref="InvalidOperationException">Is raised when this property is accessed other than <b>AsyncOP_State.Completed</b> state.</exception>
            public Exception Error
            {
                get
                {
                    if (State == AsyncOP_State.Disposed)
                    {
                        throw new ObjectDisposedException(GetType().Name);
                    }
                    if (State != AsyncOP_State.Completed)
                    {
                        throw new InvalidOperationException("Property 'Error' is accessible only in 'AsyncOP_State.Completed' state.");
                    }

                    return _exception;
                }
            }

            /// <summary>
            /// Gets email domain.
            /// </summary>
            /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and and this property is accessed.</exception>
            public string EmailDomain
            {
                get
                {
                    if (State == AsyncOP_State.Disposed)
                    {
                        throw new ObjectDisposedException(GetType().Name);
                    }

                    return _domain;
                }
            }

            /// <summary>
            /// Gets email hosts. Hosts are in priority order.
            /// </summary>
            /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and and this property is accessed.</exception>
            /// <exception cref="InvalidOperationException">Is raised when this property is accessed other than <b>AsyncOP_State.Completed</b> state.</exception>
            public HostEntry[] Hosts
            {
                get
                {
                    if (State == AsyncOP_State.Disposed)
                    {
                        throw new ObjectDisposedException(GetType().Name);
                    }
                    if (State != AsyncOP_State.Completed)
                    {
                        throw new InvalidOperationException("Property 'Error' is accessible only in 'AsyncOP_State.Completed' state.");
                    }
                    if (_exception != null)
                    {
                        throw _exception;
                    }

                    return _hosts;
                }
            }

            /// <summary>
            /// Is called when asynchronous operation has completed.
            /// </summary>
            public event EventHandler<EventArgs<GetEmailHostsAsyncOP>> CompletedAsync;

            /// <summary>
            /// Raises <b>CompletedAsync</b> event.
            /// </summary>
            private void OnCompletedAsync()
            {
                CompletedAsync?.Invoke(this, new EventArgs<GetEmailHostsAsyncOP>(this));
            }
        }

        /// <summary>
        /// This class represents <see cref="Dns_Client.GetHostAddressesAsync"/> asynchronous operation.
        /// </summary>
        public class GetHostAddressesAsyncOP : IDisposable, IAsyncOP
        {
            private readonly object _lock = new object();
            private Exception _exception;
            private string _hostNameOrIP;
            private List<IPAddress> _ipv4Addresses;
            private List<IPAddress> _ipv6Addresses;
            private int _counter;
            private bool _riseCompleted;

            /// <summary>
            /// Default constructor.
            /// </summary>
            /// <param name="hostNameOrIP">Host name or IP address.</param>
            /// <exception cref="ArgumentNullException">Is raised when <b>hostNameOrIP</b> is null reference.</exception>
            public GetHostAddressesAsyncOP(string hostNameOrIP)
            {
                _hostNameOrIP = hostNameOrIP ?? throw new ArgumentNullException("hostNameOrIP");

                _ipv4Addresses = new List<IPAddress>();
                _ipv6Addresses = new List<IPAddress>();
            }

            /// <summary>
            /// Cleans up any resource being used.
            /// </summary>
            public void Dispose()
            {
                if (State == AsyncOP_State.Disposed)
                {
                    return;
                }
                SetState(AsyncOP_State.Disposed);

                _exception = null;
                _hostNameOrIP = null;
                _ipv4Addresses = null;
                _ipv6Addresses = null;

                CompletedAsync = null;
            }

            /// <summary>
            /// Starts operation processing.
            /// </summary>
            /// <param name="dnsClient">DNS client.</param>
            /// <returns>Returns true if asynchronous operation in progress or false if operation completed synchronously.</returns>
            /// <exception cref="ArgumentNullException">Is raised when <b>dnsClient</b> is null reference.</exception>
            internal bool Start(Dns_Client dnsClient)
            {
                if (dnsClient == null)
                {
                    throw new ArgumentNullException("dnsClient");
                }

                SetState(AsyncOP_State.Active);

                // Argument 'hostNameOrIP' is IP address.
                if (Net_Utils.IsIPAddress(_hostNameOrIP))
                {
                    _ipv4Addresses.Add(IPAddress.Parse(_hostNameOrIP));

                    SetState(AsyncOP_State.Completed);
                }
                // This is probably NetBios name.
                if (_hostNameOrIP.IndexOf(".") == -1)
                {
                    try
                    {
                        // This callback is called when BeginGetHostAddresses method has completed.
                        AsyncCallback callback = delegate (IAsyncResult ar)
                        {
                            try
                            {
                                foreach (IPAddress ip in Dns.EndGetHostAddresses(ar))
                                {
                                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                                    {
                                        _ipv4Addresses.Add(ip);
                                    }
                                    else
                                    {
                                        _ipv6Addresses.Add(ip);
                                    }
                                }
                            }
                            catch (Exception x)
                            {
                                _exception = x;
                            }

                            SetState(AsyncOP_State.Completed);
                        };

                        // Start resolving host ip addresses.
                        Dns.BeginGetHostAddresses(_hostNameOrIP, callback, null);
                    }
                    catch (Exception x)
                    {
                        _exception = x;
                    }
                }
                // Query A/AAAA records.
                else
                {
                    var transaction_A = dnsClient.CreateTransaction(DNS_QType.A, _hostNameOrIP, 2000);
                    transaction_A.StateChanged += delegate (object s1, EventArgs<DNS_ClientTransaction> e1)
                    {
                        if (e1.Value.State == DNS_ClientTransactionState.Completed)
                        {
                            lock (_lock)
                            {
                                if (e1.Value.Response.ResponseCode != DNS_RCode.NO_ERROR)
                                {
                                    _exception = new DNS_ClientException(e1.Value.Response.ResponseCode);
                                }
                                else
                                {
                                    foreach (DNS_rr_A record in e1.Value.Response.GetARecords())
                                    {
                                        _ipv4Addresses.Add(record.IP);
                                    }
                                }

                                _counter++;

                                // Both A and AAAA transactions are completed, we are done.
                                if (_counter == 2)
                                {
                                    SetState(AsyncOP_State.Completed);
                                }
                            }
                        }
                    };
                    transaction_A.Timeout += delegate (object s1, EventArgs e1)
                    {
                        lock (_lock)
                        {
                            _exception = new IOException("DNS transaction timeout, no response from DNS server.");
                            _counter++;

                            // Both A and AAAA transactions are completed, we are done.
                            if (_counter == 2)
                            {
                                SetState(AsyncOP_State.Completed);
                            }
                        }
                    };
                    transaction_A.Start();

                    var transaction_AAAA = dnsClient.CreateTransaction(DNS_QType.AAAA, _hostNameOrIP, 2000);
                    transaction_AAAA.StateChanged += delegate (object s1, EventArgs<DNS_ClientTransaction> e1)
                    {
                        if (e1.Value.State == DNS_ClientTransactionState.Completed)
                        {
                            lock (_lock)
                            {
                                if (e1.Value.Response.ResponseCode != DNS_RCode.NO_ERROR)
                                {
                                    _exception = new DNS_ClientException(e1.Value.Response.ResponseCode);
                                }
                                else
                                {
                                    foreach (DNS_rr_AAAA record in e1.Value.Response.GetAAAARecords())
                                    {
                                        _ipv6Addresses.Add(record.IP);
                                    }
                                }

                                _counter++;

                                // Both A and AAAA transactions are completed, we are done.
                                if (_counter == 2)
                                {
                                    SetState(AsyncOP_State.Completed);
                                }
                            }
                        }
                    };
                    transaction_AAAA.Timeout += delegate (object s1, EventArgs e1)
                    {
                        lock (_lock)
                        {
                            _exception = new IOException("DNS transaction timeout, no response from DNS server.");
                            _counter++;

                            // Both A and AAAA transactions are completed, we are done.
                            if (_counter == 2)
                            {
                                SetState(AsyncOP_State.Completed);
                            }
                        }
                    };
                    transaction_AAAA.Start();
                }

                // Set flag rise CompletedAsync event flag. The event is raised when async op completes.
                // If already completed sync, that flag has no effect.
                lock (_lock)
                {
                    _riseCompleted = true;

                    return State == AsyncOP_State.Active;
                }
            }

            /// <summary>
            /// Sets operation state.
            /// </summary>
            /// <param name="state">New state.</param>
            private void SetState(AsyncOP_State state)
            {
                if (State == AsyncOP_State.Disposed)
                {
                    return;
                }

                lock (_lock)
                {
                    State = state;

                    if (State == AsyncOP_State.Completed && _riseCompleted)
                    {
                        OnCompletedAsync();
                    }
                }
            }

            /// <summary>
            /// Gets asynchronous operation state.
            /// </summary>
            public AsyncOP_State State { get; private set; } = AsyncOP_State.WaitingForStart;

            /// <summary>
            /// Gets error happened during operation. Returns null if no error.
            /// </summary>
            /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and and this property is accessed.</exception>
            /// <exception cref="InvalidOperationException">Is raised when this property is accessed other than <b>AsyncOP_State.Completed</b> state.</exception>
            public Exception Error
            {
                get
                {
                    if (State == AsyncOP_State.Disposed)
                    {
                        throw new ObjectDisposedException(GetType().Name);
                    }
                    if (State != AsyncOP_State.Completed)
                    {
                        throw new InvalidOperationException("Property 'Error' is accessible only in 'AsyncOP_State.Completed' state.");
                    }

                    return _exception;
                }
            }

            /// <summary>
            /// Gets argument <b>hostNameOrIP</b> value.
            /// </summary>
            /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and and this property is accessed.</exception>
            public string HostNameOrIP
            {
                get
                {
                    if (State == AsyncOP_State.Disposed)
                    {
                        throw new ObjectDisposedException(GetType().Name);
                    }

                    return _hostNameOrIP;
                }
            }

            /// <summary>
            /// Gets host IP addresses.
            /// </summary>
            /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and and this property is accessed.</exception>
            /// <exception cref="InvalidOperationException">Is raised when this property is accessed other than <b>AsyncOP_State.Completed</b> state.</exception>
            public IPAddress[] Addresses
            {
                get
                {
                    if (State == AsyncOP_State.Disposed)
                    {
                        throw new ObjectDisposedException(GetType().Name);
                    }
                    if (State != AsyncOP_State.Completed)
                    {
                        throw new InvalidOperationException("Property 'Addresses' is accessible only in 'AsyncOP_State.Completed' state.");
                    }
                    if (_exception != null)
                    {
                        throw _exception;
                    }

                    // We list IPv4 addresses before IPv6.
                    var retVal = new List<IPAddress>();
                    retVal.AddRange(_ipv4Addresses);
                    retVal.AddRange(_ipv6Addresses);

                    return retVal.ToArray();
                }
            }

            /// <summary>
            /// Is called when asynchronous operation has completed.
            /// </summary>
            public event EventHandler<EventArgs<GetHostAddressesAsyncOP>> CompletedAsync;

            /// <summary>
            /// Raises <b>CompletedAsync</b> event.
            /// </summary>
            private void OnCompletedAsync()
            {
                CompletedAsync?.Invoke(this, new EventArgs<GetHostAddressesAsyncOP>(this));
            }
        }

        /// <summary>
        /// This class represents <see cref="Dns_Client.GetHostsAddressesAsync"/> asynchronous operation.
        /// </summary>
        public class GetHostsAddressesAsyncOP : IDisposable, IAsyncOP
        {
            private readonly object _lock = new object();
            private Exception _exception;
            private string[] _hostNames;
            private readonly bool _resolveAny;
            private Dictionary<int, GetHostAddressesAsyncOP> _ipLookupQueue;
            private HostEntry[] _hostEntries;
            private bool _riseCompleted;
            private int _resolvedCount;

            /// <summary>
            /// Default constructor.
            /// </summary>
            /// <param name="hostNames">Host names to resolve.</param>
            /// <exception cref="ArgumentNullException">Is raised when <b>hostNames</b> is null reference.</exception>
            public GetHostsAddressesAsyncOP(string[] hostNames) : this(hostNames, false)
            {
            }

            /// <summary>
            /// Default constructor.
            /// </summary>
            /// <param name="hostNames">Host names to resolve.</param>
            /// <param name="resolveAny">If true, as long as one host name is resolved, no error returned.</param>
            /// <exception cref="ArgumentNullException">Is raised when <b>hostNames</b> is null reference.</exception>
            public GetHostsAddressesAsyncOP(string[] hostNames, bool resolveAny)
            {
                _hostNames = hostNames ?? throw new ArgumentNullException("hostNames");
                _resolveAny = resolveAny;

                _ipLookupQueue = new Dictionary<int, GetHostAddressesAsyncOP>();
            }

            /// <summary>
            /// Cleans up any resource being used.
            /// </summary>
            public void Dispose()
            {
                if (State == AsyncOP_State.Disposed)
                {
                    return;
                }
                SetState(AsyncOP_State.Disposed);

                _exception = null;
                _hostNames = null;
                _ipLookupQueue = null;
                _hostEntries = null;

                CompletedAsync = null;
            }

            /// <summary>
            /// Starts operation processing.
            /// </summary>
            /// <param name="dnsClient">DNS client.</param>
            /// <returns>Returns true if asynchronous operation in progress or false if operation completed synchronously.</returns>
            /// <exception cref="ArgumentNullException">Is raised when <b>dnsClient</b> is null reference.</exception>
            internal bool Start(Dns_Client dnsClient)
            {
                if (dnsClient == null)
                {
                    throw new ArgumentNullException("dnsClient");
                }

                SetState(AsyncOP_State.Active);

                _hostEntries = new HostEntry[_hostNames.Length];

                // Create look up operations for hosts. The "opList" copy array is needed because
                // when we start asyn OP, m_pIpLookupQueue may be altered when OP completes.
                var opList = new Dictionary<int, GetHostAddressesAsyncOP>();
                for (int i = 0; i < _hostNames.Length; i++)
                {
                    var op = new GetHostAddressesAsyncOP(_hostNames[i]);
                    _ipLookupQueue.Add(i, op);
                    opList.Add(i, op);
                }

                // Start operations.
                foreach (KeyValuePair<int, GetHostAddressesAsyncOP> entry in opList)
                {
                    // NOTE: We may not access "entry" in CompletedAsync, because next for loop reassigns this value.
                    int index = entry.Key;

                    // This event is raised when GetHostAddressesAsync completes asynchronously.
                    entry.Value.CompletedAsync += delegate (object s1, EventArgs<GetHostAddressesAsyncOP> e1)
                    {
                        GetHostAddressesCompleted(e1.Value, index);
                    };
                    // GetHostAddressesAsync completes synchronously.
                    if (!dnsClient.GetHostAddressesAsync(entry.Value))
                    {
                        GetHostAddressesCompleted(entry.Value, index);
                    }
                }

                // Set flag rise CompletedAsync event flag. The event is raised when async op completes.
                // If already completed sync, that flag has no effect.
                lock (_lock)
                {
                    _riseCompleted = true;

                    return State == AsyncOP_State.Active;
                }
            }

            /// <summary>
            /// Sets operation state.
            /// </summary>
            /// <param name="state">New state.</param>
            private void SetState(AsyncOP_State state)
            {
                if (State == AsyncOP_State.Disposed)
                {
                    return;
                }

                lock (_lock)
                {
                    State = state;

                    if (State == AsyncOP_State.Completed && _riseCompleted)
                    {
                        OnCompletedAsync();
                    }
                }
            }

            /// <summary>
            /// This method is called when GetHostAddresses operation has completed.
            /// </summary>
            /// <param name="op">Asynchronous operation.</param>
            /// <param name="index">Index in 'm_pHostEntries' where to store lookup result.</param>
            private void GetHostAddressesCompleted(GetHostAddressesAsyncOP op, int index)
            {
                lock (_lock)
                {
                    try
                    {
                        if (op.Error != null)
                        {
                            // We wanted any of the host names to resolve:
                            //  *) We have already one resolved host name.
                            //  *) We have more names to resolve, so next may succeed.
                            if (_resolveAny && (_resolvedCount > 0 || _ipLookupQueue.Count > 1))
                            {
                            }
                            else
                            {
                                _exception = op.Error;
                            }
                        }
                        else
                        {
                            _hostEntries[index] = new HostEntry(op.HostNameOrIP, op.Addresses, null);
                            _resolvedCount++;
                        }

                        _ipLookupQueue.Remove(index);
                        if (_ipLookupQueue.Count == 0)
                        {
                            // We wanted resolve any, so some host names may not be resolved and are null, remove them from response.
                            if (_resolveAny)
                            {
                                var retVal = new List<HostEntry>();
                                foreach (HostEntry host in _hostEntries)
                                {
                                    if (host != null)
                                    {
                                        retVal.Add(host);
                                    }
                                }

                                _hostEntries = retVal.ToArray();
                            }

                            SetState(AsyncOP_State.Completed);
                        }
                    }
                    catch (Exception x)
                    {
                        _exception = x;

                        SetState(AsyncOP_State.Completed);
                    }
                }

                op.Dispose();
            }

            /// <summary>
            /// Gets asynchronous operation state.
            /// </summary>
            public AsyncOP_State State { get; private set; } = AsyncOP_State.WaitingForStart;

            /// <summary>
            /// Gets error happened during operation. Returns null if no error.
            /// </summary>
            /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and and this property is accessed.</exception>
            /// <exception cref="InvalidOperationException">Is raised when this property is accessed other than <b>AsyncOP_State.Completed</b> state.</exception>
            public Exception Error
            {
                get
                {
                    if (State == AsyncOP_State.Disposed)
                    {
                        throw new ObjectDisposedException(GetType().Name);
                    }
                    if (State != AsyncOP_State.Completed)
                    {
                        throw new InvalidOperationException("Property 'Error' is accessible only in 'AsyncOP_State.Completed' state.");
                    }

                    return _exception;
                }
            }

            /// <summary>
            /// Gets argument <b>hostNames</b> value.
            /// </summary>
            /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and and this property is accessed.</exception>
            public string[] HostNames
            {
                get
                {
                    if (State == AsyncOP_State.Disposed)
                    {
                        throw new ObjectDisposedException(GetType().Name);
                    }

                    return _hostNames;
                }
            }

            /// <summary>
            /// Gets host entries.
            /// </summary>
            /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and and this property is accessed.</exception>
            /// <exception cref="InvalidOperationException">Is raised when this property is accessed other than <b>AsyncOP_State.Completed</b> state.</exception>
            public HostEntry[] HostEntries
            {
                get
                {
                    if (State == AsyncOP_State.Disposed)
                    {
                        throw new ObjectDisposedException(GetType().Name);
                    }
                    if (State != AsyncOP_State.Completed)
                    {
                        throw new InvalidOperationException("Property 'HostEntries' is accessible only in 'AsyncOP_State.Completed' state.");
                    }
                    if (_exception != null)
                    {
                        throw _exception;
                    }

                    return _hostEntries;
                }
            }

            /// <summary>
            /// Is called when asynchronous operation has completed.
            /// </summary>
            public event EventHandler<EventArgs<GetHostsAddressesAsyncOP>> CompletedAsync;

            /// <summary>
            /// Raises <b>CompletedAsync</b> event.
            /// </summary>
            private void OnCompletedAsync()
            {
                CompletedAsync?.Invoke(this, new EventArgs<GetHostsAddressesAsyncOP>(this));
            }
        }
    }
}
