using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Security.Principal;
using LumiSoft.Net.IO;
using LumiSoft.Net.TCP;
using LumiSoft.Net.SMTP.Client;
using LumiSoft.Net.DNS.Client;
using LumiSoft.Net.Log;

namespace LumiSoft.Net.SMTP.Relay
{
    /// <summary>
    /// This class implements SMTP relay server session.
    /// </summary>
    public class Relay_Session : TCP_Session
    {
        private Relay_Target _activeTarget;
        private IPBindInfo _localBindInfo;
        private Relay_QueueItem _relayItem;

        private Relay_Server _server;
        private Relay_SmartHost[] _smartHosts;
        private SMTP_Client _smtpClient;
        private List<Relay_Target> _targets;
        private readonly Relay_Mode _relayMode = Relay_Mode.Dns;
        private readonly DateTime _sessionCreateTime;
        private readonly string _sessionID = "";

        /// <summary>
        /// Dns relay session constructor.
        /// </summary>
        /// <param name="server">Owner relay server.</param>
        /// <param name="realyItem">Relay item.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>server</b> or <b>realyItem</b> is null.</exception>
        /// <exception cref="ArgumentException">Is raised when any of the arguments has invalid value.</exception>
        internal Relay_Session(Relay_Server server, Relay_QueueItem realyItem)
        {
            _server = server ?? throw new ArgumentNullException("server");
            _relayItem = realyItem ?? throw new ArgumentNullException("realyItem");

            _sessionID = Guid.NewGuid().ToString();
            _sessionCreateTime = DateTime.Now;
            _targets = new List<Relay_Target>();
            _smtpClient = new SMTP_Client();
        }

        /// <summary>
        /// Smart host relay session constructor.
        /// </summary>
        /// <param name="server">Owner relay server.</param>
        /// <param name="realyItem">Relay item.</param>
        /// <param name="smartHosts">Smart hosts.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>server</b>,<b>realyItem</b> or <b>smartHosts</b>is null.</exception>
        /// <exception cref="ArgumentException">Is raised when any of the arguments has invalid value.</exception>
        internal Relay_Session(Relay_Server server, Relay_QueueItem realyItem, Relay_SmartHost[] smartHosts)
        {
            _server = server ?? throw new ArgumentNullException("server");
            _relayItem = realyItem ?? throw new ArgumentNullException("realyItem");
            _smartHosts = smartHosts ?? throw new ArgumentNullException("smartHosts");

            _relayMode = Relay_Mode.SmartHost;
            _sessionID = Guid.NewGuid().ToString();
            _sessionCreateTime = DateTime.Now;
            _targets = new List<Relay_Target>();
            _smtpClient = new SMTP_Client();
        }

        /// <summary>
        /// Gets session authenticated user identity, returns null if not authenticated.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this property is accessed.</exception>
        /// <exception cref="InvalidOperationException">Is raised when this property is accessed and relay session is not connected.</exception>
        public override GenericIdentity AuthenticatedUserIdentity
        {
            get
            {
                if (IsDisposed)
                {
                    throw new ObjectDisposedException(GetType().Name);
                }
                if (!_smtpClient.IsConnected)
                {
                    throw new InvalidOperationException("You must connect first.");
                }

                return _smtpClient.AuthenticatedUserIdentity;
            }
        }

        /// <summary>
        /// Gets the time when session was connected.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this property is accessed.</exception>
        public override DateTime ConnectTime
        {
            get
            {
                if (IsDisposed)
                {
                    throw new ObjectDisposedException(GetType().Name);
                }

                return _smtpClient.ConnectTime;
            }
        }

        /// <summary>
        /// Gets how many seconds has left before timout is triggered.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this property is accessed.</exception>
        public int ExpectedTimeout
        {
            get
            {
                if (IsDisposed)
                {
                    throw new ObjectDisposedException(GetType().Name);
                }

                return (int)(_server.SessionIdleTimeout - ((DateTime.Now.Ticks - TcpStream.LastActivity.Ticks) / 10000));
            }
        }

        /// <summary>
        /// Gets from address.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this property is accessed.</exception>
        public string From
        {
            get
            {
                if (IsDisposed)
                {
                    throw new ObjectDisposedException(GetType().Name);
                }

                return _relayItem.From;
            }
        }

        /// <summary>
        /// Gets session ID.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this property is accessed.</exception>
        public override string ID
        {
            get
            {
                if (IsDisposed)
                {
                    throw new ObjectDisposedException(GetType().Name);
                }

                return _sessionID;
            }
        }

        /// <summary>
        /// Gets if session is connected.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this property is accessed.</exception>
        public override bool IsConnected
        {
            get
            {
                if (IsDisposed)
                {
                    throw new ObjectDisposedException(GetType().Name);
                }

                return _smtpClient.IsConnected;
            }
        }

        /// <summary>
        /// Gets if this object is disposed.
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// Gets the last time when data was sent or received.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this property is accessed.</exception>
        public override DateTime LastActivity
        {
            get
            {
                if (IsDisposed)
                {
                    throw new ObjectDisposedException(GetType().Name);
                }

                return _smtpClient.LastActivity;
            }
        }

        /// <summary>
        /// Gets session local IP end point.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this property is accessed.</exception>
        public override IPEndPoint LocalEndPoint
        {
            get
            {
                if (IsDisposed)
                {
                    throw new ObjectDisposedException(GetType().Name);
                }

                return _smtpClient.LocalEndPoint;
            }
        }

        /// <summary>
        /// Gets local host name for LoaclEP.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this property is accessed.</exception>
        public string LocalHostName
        {
            get
            {
                if (IsDisposed)
                {
                    throw new ObjectDisposedException(GetType().Name);
                }

                return (_localBindInfo == null ? "" : _localBindInfo.HostName);
            }
        }

        /// <summary>
        /// Gets message ID which is being relayed now.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this property is accessed.</exception>
        public string MessageID
        {
            get
            {
                if (IsDisposed)
                {
                    throw new ObjectDisposedException(GetType().Name);
                }

                return _relayItem.MessageID;
            }
        }

        /// <summary>
        /// Gets message what is being relayed now.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this property is accessed.</exception>
        public Stream MessageStream
        {
            get
            {
                if (IsDisposed)
                {
                    throw new ObjectDisposedException(GetType().Name);
                }

                return _relayItem.MessageStream;
            }
        }

        /// <summary>
        /// Gets relay queue which session it is.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this property is accessed.</exception>
        public Relay_Queue Queue
        {
            get
            {
                if (IsDisposed)
                {
                    throw new ObjectDisposedException(GetType().Name);
                }

                return _relayItem.Queue;
            }
        }

        /// <summary>
        /// Gets user data what was procided to Relay_Queue.QueueMessage method.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this property is accessed.</exception>
        public object QueueTag
        {
            get
            {
                if (IsDisposed)
                {
                    throw new ObjectDisposedException(GetType().Name);
                }

                return _relayItem.Tag;
            }
        }

        /// <summary>
        /// Gets session remote IP end point.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this property is accessed.</exception>
        public override IPEndPoint RemoteEndPoint
        {
            get
            {
                if (IsDisposed)
                {
                    throw new ObjectDisposedException(GetType().Name);
                }

                return _smtpClient.RemoteEndPoint;
            }
        }

        /// <summary>
        /// Gets current remote host name. Returns null if not connected to any target.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this property is accessed.</exception>
        public string RemoteHostName
        {
            get
            {
                if (IsDisposed)
                {
                    throw new ObjectDisposedException(GetType().Name);
                }

                return _activeTarget?.HostName;
            }
        }

        /// <summary>
        /// Gets time when relay session created.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this property is accessed.</exception>
        public DateTime SessionCreateTime
        {
            get
            {
                if (IsDisposed)
                {
                    throw new ObjectDisposedException(GetType().Name);
                }

                return _sessionCreateTime;
            }
        }

        /// <summary>
        /// Gets TCP stream which must be used to send/receive data through this session.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this property is accessed.</exception>
        public override SmartStream TcpStream
        {
            get
            {
                if (IsDisposed)
                {
                    throw new ObjectDisposedException(GetType().Name);
                }

                return _smtpClient.TcpStream;
            }
        }

        /// <summary>
        /// Gets target recipient.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this property is accessed.</exception>
        public string To
        {
            get
            {
                if (IsDisposed)
                {
                    throw new ObjectDisposedException(GetType().Name);
                }

                return _relayItem.To;
            }
        }

        /// <summary>
        /// Closes relay connection.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this method is accessed.</exception>
        public override void Disconnect()
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
            if (!IsConnected)
            {
                return;
            }

            _smtpClient.Disconnect();
        }

        /// <summary>
        /// Closes relay connection.
        /// </summary>
        /// <param name="text">Text to send to the connected host.</param>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this method is accessed.</exception>
        public void Disconnect(string text)
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
            if (!IsConnected)
            {
                return;
            }

            _smtpClient.TcpStream.WriteLine(text);
            Disconnect();
        }

        /// <summary>
        /// Completes relay session and does clean up. This method is thread-safe.
        /// </summary>
        public override void Dispose()
        {
            Dispose(new ObjectDisposedException(GetType().Name));
        }

        /// <summary>
        /// Completes relay session and does clean up. This method is thread-safe.
        /// </summary>
        /// <param name="exception">Exception happened or null if relay completed successfully.</param>
        public void Dispose(Exception exception)
        {
            try
            {
                lock (this)
                {
                    if (IsDisposed)
                    {
                        return;
                    }
                    try
                    {
                        _server.OnSessionCompleted(this, exception);
                    }
                    catch
                    {
                    }
                    _server.Sessions.Remove(this);
                    IsDisposed = true;

                    _localBindInfo = null;
                    _relayItem = null;
                    _smartHosts = null;
                    if (_smtpClient != null)
                    {
                        _smtpClient.Dispose();
                        _smtpClient = null;
                    }
                    _targets = null;
                    if (_activeTarget != null)
                    {
                        _server.RemoveIpUsage(_activeTarget.Target.Address);
                        _activeTarget = null;
                    }
                    _server = null;
                }
            }
            catch (Exception x)
            {
                _server?.OnError(x);
            }
        }

        /// <summary>
        /// Start processing relay message.
        /// </summary>
        /// <param name="state">User data.</param>
        internal void Start(object state)
        {
            try
            {
                if (_server.Logger != null)
                {
                    _smtpClient.Logger = new Logger();
                    _smtpClient.Logger.WriteLog += new EventHandler<WriteLogEventArgs>(SmtpClient_WriteLog);
                }

                LogText("Starting to relay message '" + _relayItem.MessageID + "' from '" + _relayItem.From + "' to '" + _relayItem.To + "'.");

                // Resolve email target hosts.
                if (_relayMode == Relay_Mode.Dns)
                {
                    var op = new Dns_Client.GetEmailHostsAsyncOP(_relayItem.To);
                    op.CompletedAsync += delegate (object s1, EventArgs<Dns_Client.GetEmailHostsAsyncOP> e1)
                    {
                        EmailHostsResolveCompleted(_relayItem.To, op);
                    };
                    if (!_server.DnsClient.GetEmailHostsAsync(op))
                    {
                        EmailHostsResolveCompleted(_relayItem.To, op);
                    }
                }
                // Resolve smart hosts IP addresses.
                else if (_relayMode == Relay_Mode.SmartHost)
                {
                    var smartHosts = new string[_smartHosts.Length];
                    for (int i = 0; i < _smartHosts.Length; i++)
                    {
                        smartHosts[i] = _smartHosts[i].Host;
                    }

                    var op = new Dns_Client.GetHostsAddressesAsyncOP(smartHosts);
                    op.CompletedAsync += delegate (object s1, EventArgs<Dns_Client.GetHostsAddressesAsyncOP> e1)
                    {
                        SmartHostsResolveCompleted(op);
                    };
                    if (!_server.DnsClient.GetHostsAddressesAsync(op))
                    {
                        SmartHostsResolveCompleted(op);
                    }
                }
            }
            catch (Exception x)
            {
                Dispose(x);
            }
        }

        /// <summary>
        /// Is called when AUTH command has completed.
        /// </summary>
        /// <param name="op">Asynchronous operation.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>op</b> is null reference.</exception>
        private void AuthCommandCompleted(SMTP_Client.AuthAsyncOP op)
        {
            if (op == null)
            {
                throw new ArgumentNullException("op");
            }

            try
            {
                if (op.Error != null)
                {
                    Dispose(op.Error);
                }
                else
                {
                    long messageSize = -1;
                    try
                    {
                        messageSize = _relayItem.MessageStream.Length - _relayItem.MessageStream.Position;
                    }
                    catch
                    {
                        // Stream doesn't support seeking.
                    }

                    var mailOP = new SMTP_Client.MailFromAsyncOP(
                        From,
                        messageSize,
                        IsDsnSupported() ? _relayItem.DSN_Ret : SMTP_DSN_Ret.NotSpecified,
                        IsDsnSupported() ? _relayItem.EnvelopeID : null
                    );
                    mailOP.CompletedAsync += delegate (object s, EventArgs<SMTP_Client.MailFromAsyncOP> e)
                    {
                        MailCommandCompleted(mailOP);
                    };
                    if (!_smtpClient.MailFromAsync(mailOP))
                    {
                        MailCommandCompleted(mailOP);
                    }
                }
            }
            catch (Exception x)
            {
                Dispose(x);
            }
        }

        /// <summary>
        /// Starts connecting to best target.
        /// </summary>
        private void BeginConnect()
        {
            // No tagets, abort relay.
            if (_targets.Count == 0)
            {
                LogText("No relay target(s) for '" + _relayItem.To + "', aborting.");
                Dispose(new Exception("No relay target(s) for '" + _relayItem.To + "', aborting."));

                return;
            }

            // Maximum connections per IP limited.
            if (_server.MaxConnectionsPerIP > 0)
            {
                // For DNS or load-balnced smart host relay, search free target if any.
                if (_server.RelayMode == Relay_Mode.Dns || _server.SmartHostsBalanceMode == BalanceMode.LoadBalance)
                {
                    foreach (Relay_Target t in _targets)
                    {
                        // Get local IP binding for remote IP.
                        _localBindInfo = _server.GetLocalBinding(t.Target.Address);

                        // We have suitable local IP binding for the target.
                        if (_localBindInfo != null)
                        {
                            // We found free target, stop searching.
                            if (_server.TryAddIpUsage(t.Target.Address))
                            {
                                _activeTarget = t;
                                _targets.Remove(t);

                                break;
                            }
                            // Connection per IP limit reached.

                            LogText("Skipping relay target (" + t.HostName + "->" + t.Target.Address + "), maximum connections to the specified IP has reached.");
                        }
                        // No suitable local IP binding, try next target.
                        else
                        {
                            LogText("Skipping relay target (" + t.HostName + "->" + t.Target.Address + "), no suitable local IPv4/IPv6 binding.");
                        }
                    }
                }
                // Smart host fail-over mode, just check if it's free.
                else
                {
                    // Get local IP binding for remote IP.
                    _localBindInfo = _server.GetLocalBinding(_targets[0].Target.Address);

                    // We have suitable local IP binding for the target.
                    if (_localBindInfo != null)
                    {
                        // Smart host IP limit not reached.
                        if (_server.TryAddIpUsage(_targets[0].Target.Address))
                        {
                            _activeTarget = _targets[0];
                            _targets.RemoveAt(0);
                        }
                        // Connection per IP limit reached.
                        else
                        {
                            LogText("Skipping relay target (" + _targets[0].HostName + "->" + _targets[0].Target.Address + "), maximum connections to the specified IP has reached.");
                        }
                    }
                    // No suitable local IP binding, try next target.
                    else
                    {
                        LogText("Skipping relay target (" + _targets[0].HostName + "->" + _targets[0].Target.Address + "), no suitable local IPv4/IPv6 binding.");
                    }
                }
            }
            // Just get first target.
            else
            {
                // Get local IP binding for remote IP.
                _localBindInfo = _server.GetLocalBinding(_targets[0].Target.Address);

                // We have suitable local IP binding for the target.
                if (_localBindInfo != null)
                {
                    _activeTarget = _targets[0];
                    _targets.RemoveAt(0);
                }
                // No suitable local IP binding, try next target.
                else
                {
                    LogText("Skipping relay target (" + _targets[0].HostName + "->" + _targets[0].Target.Address + "), no suitable local IPv4/IPv6 binding.");
                }
            }

            // We don't have suitable local IP end point for relay target.
            // This may heppen for example: if remote server supports only IPv6 and we don't have local IPv6 local end point.
            if (_localBindInfo == null)
            {
                LogText("No suitable IPv4/IPv6 local IP endpoint for relay target.");
                Dispose(new Exception("No suitable IPv4/IPv6 local IP endpoint for relay target."));

                return;
            }

            // If all targets has exeeded maximum allowed connection per IP address, end relay session,
            // next relay cycle will try to relay again.
            if (_activeTarget == null)
            {
                LogText("All targets has exeeded maximum allowed connection per IP address, skip relay.");
                Dispose(new Exception("All targets has exeeded maximum allowed connection per IP address, skip relay."));

                return;
            }

            // Set SMTP host name.
            _smtpClient.LocalHostName = _localBindInfo.HostName;

            // Start connecting to remote end point.
            var connectOP = new TCP_Client.ConnectAsyncOP(new IPEndPoint(_localBindInfo.IP, 0), _activeTarget.Target, false, null);
            connectOP.CompletedAsync += delegate (object s, EventArgs<TCP_Client.ConnectAsyncOP> e)
            {
                ConnectCompleted(connectOP);
            };
            if (!_smtpClient.ConnectAsync(connectOP))
            {
                ConnectCompleted(connectOP);
            }
        }

        /// <summary>
        /// Is called when EHLO/HELO command has completed.
        /// </summary>
        /// <param name="op">Asynchronous operation.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>op</b> is null reference.</exception>
        private void ConnectCompleted(TCP_Client.ConnectAsyncOP op)
        {
            if (op == null)
            {
                throw new ArgumentNullException("op");
            }

            try
            {
                // Connect failed.
                if (op.Error != null)
                {
                    try
                    {
                        // Release IP usage.
                        _server.RemoveIpUsage(_activeTarget.Target.Address);
                        _activeTarget = null;

                        // Connect failed, if there are more target IPs, try next one.
                        if (!IsDisposed && !IsConnected && _targets.Count > 0)
                        {
                            BeginConnect();
                        }
                        else
                        {
                            Dispose(op.Error);
                        }
                    }
                    catch (Exception x1)
                    {
                        Dispose(x1);
                    }
                }
                // Connect suceeded.
                else
                {
                    // Do EHLO/HELO.
                    var hostName = string.IsNullOrEmpty(_localBindInfo.HostName) ? Dns.GetHostName() : _localBindInfo.HostName;
                    var ehloOP = new SMTP_Client.EhloHeloAsyncOP(hostName);
                    ehloOP.CompletedAsync += delegate (object s, EventArgs<SMTP_Client.EhloHeloAsyncOP> e)
                    {
                        EhloCommandCompleted(ehloOP);
                    };
                    if (!_smtpClient.EhloHeloAsync(ehloOP))
                    {
                        EhloCommandCompleted(ehloOP);
                    }
                }
            }
            catch (Exception x)
            {
                Dispose(x);
            }
        }

        /// <summary>
        /// Is called when EHLO/HELO command has completed.
        /// </summary>
        /// <param name="op">Asynchronous operation.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>op</b> is null reference.</exception>
        private void EhloCommandCompleted(SMTP_Client.EhloHeloAsyncOP op)
        {
            if (op == null)
            {
                throw new ArgumentNullException("op");
            }

            try
            {
                if (op.Error != null)
                {
                    Dispose(op.Error);
                }
                else
                {
                    // Start TLS requested, start switching to secure.
                    if (!_smtpClient.IsSecureConnection && _activeTarget.SslMode == SslMode.TLS)
                    {
                        var startTlsOP = new SMTP_Client.StartTlsAsyncOP(null);
                        startTlsOP.CompletedAsync += delegate (object s, EventArgs<SMTP_Client.StartTlsAsyncOP> e)
                        {
                            StartTlsCommandCompleted(startTlsOP);
                        };
                        if (!_smtpClient.StartTlsAsync(startTlsOP))
                        {
                            StartTlsCommandCompleted(startTlsOP);
                        }
                    }
                    // Authentication requested, start authenticating.
                    else if (!string.IsNullOrEmpty(_activeTarget.UserName))
                    {
                        var authOP = new SMTP_Client.AuthAsyncOP(_smtpClient.AuthGetStrongestMethod(_activeTarget.UserName, _activeTarget.Password));
                        authOP.CompletedAsync += delegate (object s, EventArgs<SMTP_Client.AuthAsyncOP> e)
                        {
                            AuthCommandCompleted(authOP);
                        };
                        if (!_smtpClient.AuthAsync(authOP))
                        {
                            AuthCommandCompleted(authOP);
                        }
                    }
                    // Start MAIL command.
                    else
                    {
                        long messageSize = -1;
                        try
                        {
                            messageSize = _relayItem.MessageStream.Length - _relayItem.MessageStream.Position;
                        }
                        catch
                        {
                            // Stream doesn't support seeking.
                        }

                        var mailOP = new SMTP_Client.MailFromAsyncOP(
                            From,
                            messageSize,
                            IsDsnSupported() ? _relayItem.DSN_Ret : SMTP_DSN_Ret.NotSpecified,
                            IsDsnSupported() ? _relayItem.EnvelopeID : null
                        );
                        mailOP.CompletedAsync += delegate (object s, EventArgs<SMTP_Client.MailFromAsyncOP> e)
                        {
                            MailCommandCompleted(mailOP);
                        };
                        if (!_smtpClient.MailFromAsync(mailOP))
                        {
                            MailCommandCompleted(mailOP);
                        }
                    }
                }
            }
            catch (Exception x)
            {
                Dispose(x);
            }
        }

        /// <summary>
        /// Is called when email domain target servers resolve operation has completed.
        /// </summary>
        /// <param name="to">RCPT TO: address.</param>
        /// <param name="op">Asynchronous operation.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>to</b> or <b>op</b> is null reference.</exception>
        private void EmailHostsResolveCompleted(string to, Dns_Client.GetEmailHostsAsyncOP op)
        {
            if (to == null)
            {
                throw new ArgumentNullException("to");
            }
            if (op == null)
            {
                throw new ArgumentNullException("op");
            }

            if (op.Error != null)
            {
                LogText("Failed to resolve email domain for email address '" + to + "' with error: " + op.Error.Message + ".");

                Dispose(op.Error);
            }
            else
            {
                var buf = new StringBuilder();
                foreach (HostEntry host in op.Hosts)
                {
                    foreach (IPAddress ip in host.Addresses)
                    {
                        _targets.Add(new Relay_Target(host.HostName, new IPEndPoint(ip, 25)));
                    }
                    buf.Append(host.HostName + " ");
                }
                LogText("Resolved to following email hosts: (" + buf.ToString().TrimEnd() + ").");

                BeginConnect();
            }

            op.Dispose();
        }

        /// <summary>
        /// Gets if DSN extention is supported by remote server.
        /// </summary>
        /// <returns></returns>
        private bool IsDsnSupported()
        {
            foreach (string feature in _smtpClient.EsmtpFeatures)
            {
                if (string.Equals(feature, SMTP_ServiceExtensions.DSN, StringComparison.InvariantCultureIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Logs specified text if logging enabled.
        /// </summary>
        /// <param name="text">Text to log.</param>
        private void LogText(string text)
        {
            if (_server.Logger != null)
            {
                GenericIdentity identity = null;
                try
                {
                    identity = AuthenticatedUserIdentity;
                }
                catch
                {
                }
                IPEndPoint localEP = null;
                IPEndPoint remoteEP = null;
                try
                {
                    localEP = _smtpClient.LocalEndPoint;
                    remoteEP = _smtpClient.RemoteEndPoint;
                }
                catch
                {
                }
                _server.Logger.AddText(_sessionID, identity, text, localEP, remoteEP);
            }
        }

        /// <summary>
        /// Is called when MAIL command has completed.
        /// </summary>
        /// <param name="op">Asynchronous operation.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>op</b> is null reference.</exception>
        private void MailCommandCompleted(SMTP_Client.MailFromAsyncOP op)
        {
            if (op == null)
            {
                throw new ArgumentNullException("op");
            }

            try
            {
                if (op.Error != null)
                {
                    Dispose(op.Error);
                }
                else
                {
                    var rcptOP = new SMTP_Client.RcptToAsyncOP(
                        To,
                        IsDsnSupported() ? _relayItem.DSN_Notify : SMTP_DSN_Notify.NotSpecified,
                        IsDsnSupported() ? _relayItem.OriginalRecipient : null
                    );
                    rcptOP.CompletedAsync += delegate (object s, EventArgs<SMTP_Client.RcptToAsyncOP> e)
                    {
                        RcptCommandCompleted(rcptOP);
                    };
                    if (!_smtpClient.RcptToAsync(rcptOP))
                    {
                        RcptCommandCompleted(rcptOP);
                    }
                }
            }
            catch (Exception x)
            {
                Dispose(x);
            }
        }

        /// <summary>
        /// Is called when message sending has completed.
        /// </summary>
        /// <param name="op">Asynchronous operation.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>op</b> is null reference.</exception>
        private void MessageSendingCompleted(SMTP_Client.SendMessageAsyncOP op)
        {
            if (op == null)
            {
                throw new ArgumentNullException("op");
            }

            try
            {
                if (op.Error != null)
                {
                    Dispose(op.Error);
                }
                // Message sent sucessfully.
                else
                {
                    Dispose(null);
                }
            }
            catch (Exception x)
            {
                Dispose(x);
            }

            op.Dispose();
        }

        /// <summary>
        /// Is called when RCPT command has completed.
        /// </summary>
        /// <param name="op">Asynchronous operation.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>op</b> is null reference.</exception>
        private void RcptCommandCompleted(SMTP_Client.RcptToAsyncOP op)
        {
            if (op == null)
            {
                throw new ArgumentNullException("op");
            }

            try
            {
                if (op.Error != null)
                {
                    Dispose(op.Error);
                }
                else
                {
                    // Start sending message.
                    var sendMsgOP = new SMTP_Client.SendMessageAsyncOP(_relayItem.MessageStream, false);
                    sendMsgOP.CompletedAsync += delegate (object s, EventArgs<SMTP_Client.SendMessageAsyncOP> e)
                    {
                        MessageSendingCompleted(sendMsgOP);
                    };
                    if (!_smtpClient.SendMessageAsync(sendMsgOP))
                    {
                        MessageSendingCompleted(sendMsgOP);
                    }
                }
            }
            catch (Exception x)
            {
                Dispose(x);
            }
        }

        /// <summary>
        /// Is called when smart hosts ip addresses resolve operation has completed.
        /// </summary>
        /// <param name="op">Asynchronous operation.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>op</b> is null reference.</exception>
        private void SmartHostsResolveCompleted(Dns_Client.GetHostsAddressesAsyncOP op)
        {
            if (op == null)
            {
                throw new ArgumentNullException("op");
            }

            if (op.Error != null)
            {
                LogText("Failed to resolve relay smart host(s) ip addresses with error: " + op.Error.Message + ".");

                Dispose(op.Error);
            }
            else
            {
                for (int i = 0; i < op.HostEntries.Length; i++)
                {
                    var smartHost = _smartHosts[i];

                    foreach (IPAddress ip in op.HostEntries[i].Addresses)
                    {
                        _targets.Add(new Relay_Target(smartHost.Host, new IPEndPoint(ip, smartHost.Port), smartHost.SslMode, smartHost.UserName, smartHost.Password));
                    }
                }

                BeginConnect();
            }

            op.Dispose();
        }

        /// <summary>
        /// Thsi method is called when SMTP client has new log entry available.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">Event data.</param>
        private void SmtpClient_WriteLog(object sender, WriteLogEventArgs e)
        {
            try
            {
                if (_server.Logger == null)
                {
                }
                else if (e.LogEntry.EntryType == LogEntryType.Read)
                {
                    _server.Logger.AddRead(_sessionID, e.LogEntry.UserIdentity, e.LogEntry.Size, e.LogEntry.Text, e.LogEntry.LocalEndPoint, e.LogEntry.RemoteEndPoint);
                }
                else if (e.LogEntry.EntryType == LogEntryType.Text)
                {
                    _server.Logger.AddText(_sessionID, e.LogEntry.UserIdentity, e.LogEntry.Text, e.LogEntry.LocalEndPoint, e.LogEntry.RemoteEndPoint);
                }
                else if (e.LogEntry.EntryType == LogEntryType.Write)
                {
                    _server.Logger.AddWrite(_sessionID, e.LogEntry.UserIdentity, e.LogEntry.Size, e.LogEntry.Text, e.LogEntry.LocalEndPoint, e.LogEntry.RemoteEndPoint);
                }
                else if (e.LogEntry.EntryType == LogEntryType.Exception)
                {
                    _server.Logger.AddException(_sessionID, e.LogEntry.UserIdentity, e.LogEntry.Text, e.LogEntry.LocalEndPoint, e.LogEntry.RemoteEndPoint, e.LogEntry.Exception);
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// Is called when STARTTLS command has completed.
        /// </summary>
        /// <param name="op">Asynchronous operation.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>op</b> is null reference.</exception>
        private void StartTlsCommandCompleted(SMTP_Client.StartTlsAsyncOP op)
        {
            if (op == null)
            {
                throw new ArgumentNullException("op");
            }

            try
            {
                if (op.Error != null)
                {
                    Dispose(op.Error);
                }
                else
                {
                    // Do EHLO/HELO.
                    var ehloOP = new SMTP_Client.EhloHeloAsyncOP(null);
                    ehloOP.CompletedAsync += delegate (object s, EventArgs<SMTP_Client.EhloHeloAsyncOP> e)
                    {
                        EhloCommandCompleted(ehloOP);
                    };
                    if (!_smtpClient.EhloHeloAsync(ehloOP))
                    {
                        EhloCommandCompleted(ehloOP);
                    }
                }
            }
            catch (Exception x)
            {
                Dispose(x);
            }
        }
        /// <summary>
        /// This class holds relay target information.
        /// </summary>
        private class Relay_Target
        {
            /// <summary>
            /// Default constructor.
            /// </summary>
            /// <param name="hostName">Target host name.</param>
            /// <param name="target">Target host IP end point.</param>
            public Relay_Target(string hostName, IPEndPoint target)
            {
                HostName = hostName;
                Target = target;
            }

            /// <summary>
            /// Default constructor.
            /// </summary>
            /// <param name="hostName">Target host name.</param>
            /// <param name="target">Target host IP end point.</param>
            /// <param name="sslMode">SSL mode.</param>
            /// <param name="userName">Target host user name.</param>
            /// <param name="password">Target host password.</param>
            public Relay_Target(string hostName, IPEndPoint target, SslMode sslMode, string userName, string password)
            {
                HostName = hostName;
                Target = target;
                SslMode = sslMode;
                UserName = userName;
                Password = password;
            }

            /// <summary>
            /// Gets target host name.
            /// </summary>
            public string HostName { get; } = "";

            /// <summary>
            /// Gets specified target IP end point.
            /// </summary>
            public IPEndPoint Target { get; }

            /// <summary>
            /// Gets target SSL mode.
            /// </summary>
            public SslMode SslMode { get; } = SslMode.None;

            /// <summary>
            /// Gets target server user name.
            /// </summary>
            public string UserName { get; }

            /// <summary>
            /// Gets target server password.
            /// </summary>
            public string Password { get; }
        }
    }
}
