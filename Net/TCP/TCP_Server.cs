using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

using LumiSoft.Net.Log;

namespace LumiSoft.Net.TCP
{
    /// <summary>
    /// This class implements generic TCP session based server.
    /// </summary>
    public class TCP_Server<T> : IDisposable where T : TCP_ServerSession, new()
    {
        private long m_ConnectionsProcessed;
        private long m_MaxConnections;
        private long m_MaxConnectionsPerIP;

        private IPBindInfo[] m_pBindings = new IPBindInfo[0];
        private readonly List<TCP_Acceptor> m_pConnectionAcceptors;
        private readonly List<ListeningPoint> m_pListeningPoints;
        private Logger m_pLogger;
        private TCP_SessionCollection<TCP_ServerSession> m_pSessions;
        private TimerEx m_pTimer_IdleTimeout;
        private int m_SessionIdleTimeout = 100;
        private DateTime m_StartTime;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public TCP_Server()
        {
            m_pConnectionAcceptors = new List<TCP_Acceptor>();
            m_pListeningPoints = new List<ListeningPoint>();
            m_pSessions = new TCP_SessionCollection<TCP_ServerSession>();
        }

        /// <summary>
        /// This event is raised when TCP server has disposed.
        /// </summary>
        public event EventHandler Disposed;

        /// <summary>
        /// This event is raised when TCP server has unknown unhandled error.
        /// </summary>
        public event ErrorEventHandler Error;

        /// <summary>
        /// This event is raised when TCP server creates new session.
        /// </summary>
        public event EventHandler<TCP_ServerSessionEventArgs<T>> SessionCreated;

        /// <summary>
        /// This event is raised when TCP server has started.
        /// </summary>
        public event EventHandler Started;

        /// <summary>
        /// This event is raised when TCP server has stopped.
        /// </summary>
        public event EventHandler Stopped;

        /// <summary>
        /// Gets or sets TCP server IP bindings.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this property is accessed.</exception>
        public IPBindInfo[] Bindings
        {
            get
            {
                if (IsDisposed)
                {
                    throw new ObjectDisposedException(GetType().Name);
                }

                return m_pBindings;
            }

            set
            {
                if (IsDisposed)
                {
                    throw new ObjectDisposedException(GetType().Name);
                }
                if (value == null)
                {
                    value = new IPBindInfo[0];
                }

                //--- See binds has changed --------------
                bool changed = false;
                if (m_pBindings.Length != value.Length)
                {
                    changed = true;
                }
                else
                {
                    for (int i = 0; i < m_pBindings.Length; i++)
                    {
                        if (!m_pBindings[i].Equals(value[i]))
                        {
                            changed = true;
                            break;
                        }
                    }
                }

                if (changed)
                {
                    m_pBindings = value;

                    if (IsRunning)
                    {
                        StartListen();
                    }
                }
            }
        }

        /// <summary>
        /// Gets how many connections this TCP server has processed.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this property is accessed.</exception>
        /// <exception cref="InvalidOperationException">Is raised when TCP server is not running and this property is accesed.</exception>
        public long ConnectionsProcessed
        {
            get
            {
                if (IsDisposed)
                {
                    throw new ObjectDisposedException("TCP_Server");
                }
                if (!IsRunning)
                {
                    throw new InvalidOperationException("TCP server is not running.");
                }

                return m_ConnectionsProcessed;
            }
        }

        /// <summary>
        /// Gets if server is disposed.
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// Gets if server is running.
        /// </summary>
        public bool IsRunning { get; private set; }

        /// <summary>
        /// Gets local listening IP end points.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this property is accessed.</exception>
        public IPEndPoint[] LocalEndPoints
        {
            get
            {
                if (IsDisposed)
                {
                    throw new ObjectDisposedException(GetType().Name);
                }

                var retVal = new List<IPEndPoint>();
                foreach (IPBindInfo bind in Bindings)
                {
                    if (bind.IP.Equals(IPAddress.Any))
                    {
                        foreach (IPAddress ip in Dns.GetHostAddresses(""))
                        {
                            if (ip.AddressFamily == AddressFamily.InterNetwork && !retVal.Contains(new IPEndPoint(ip, bind.Port)))
                            {
                                retVal.Add(new IPEndPoint(ip, bind.Port));
                            }
                        }
                    }
                    else if (bind.IP.Equals(IPAddress.IPv6Any))
                    {
                        foreach (IPAddress ip in Dns.GetHostAddresses(""))
                        {
                            if (ip.AddressFamily == AddressFamily.InterNetworkV6 && !retVal.Contains(new IPEndPoint(ip, bind.Port)))
                            {
                                retVal.Add(new IPEndPoint(ip, bind.Port));
                            }
                        }
                    }
                    else
                    {
                        if (!retVal.Contains(bind.EndPoint))
                        {
                            retVal.Add(bind.EndPoint);
                        }
                    }
                }

                return retVal.ToArray();
            }
        }

        /// <summary>
        /// Gets or sets logger. Value null means no logging.
        /// </summary>
        public Logger Logger
        {
            get
            {
                if (IsDisposed)
                {
                    throw new ObjectDisposedException(GetType().Name);
                }

                return m_pLogger;
            }

            set
            {
                if (IsDisposed)
                {
                    throw new ObjectDisposedException(GetType().Name);
                }

                m_pLogger = value;
            }
        }

        /// <summary>
        /// Gets or sets maximum allowed concurent connections. Value 0 means unlimited.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this property is accessed.</exception>
        /// <exception cref="ArgumentException">Is raised when negative value is passed.</exception>
        public long MaxConnections
        {
            get
            {
                if (IsDisposed)
                {
                    throw new ObjectDisposedException("TCP_Server");
                }

                return m_MaxConnections;
            }

            set
            {
                if (IsDisposed)
                {
                    throw new ObjectDisposedException("TCP_Server");
                }
                if (value < 0)
                {
                    throw new ArgumentException("Property 'MaxConnections' value must be >= 0.");
                }

                m_MaxConnections = value;
            }
        }

        /// <summary>
        /// Gets or sets maximum allowed connections for 1 IP address. Value 0 means unlimited.
        /// </summary>
        public long MaxConnectionsPerIP
        {
            get
            {
                if (IsDisposed)
                {
                    throw new ObjectDisposedException("TCP_Server");
                }

                return m_MaxConnectionsPerIP;
            }

            set
            {
                if (IsDisposed)
                {
                    throw new ObjectDisposedException("TCP_Server");
                }
                if (m_MaxConnectionsPerIP < 0)
                {
                    throw new ArgumentException("Property 'MaxConnectionsPerIP' value must be >= 0.");
                }

                m_MaxConnectionsPerIP = value;
            }
        }

        /// <summary>
        /// Gets or sets maximum allowed session idle time in seconds, after what session will be terminated. Value 0 means unlimited,
        /// but this is strongly not recommened.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this property is accessed.</exception>
        /// <exception cref="ArgumentException">Is raised when negative value is passed.</exception>
        public int SessionIdleTimeout
        {
            get
            {
                if (IsDisposed)
                {
                    throw new ObjectDisposedException("TCP_Server");
                }

                return m_SessionIdleTimeout;
            }

            set
            {
                if (IsDisposed)
                {
                    throw new ObjectDisposedException("TCP_Server");
                }
                if (value < 0)
                {
                    throw new ArgumentException("Property 'SessionIdleTimeout' value must be >= 0.");
                }

                m_SessionIdleTimeout = value;
            }
        }

        /// <summary>
        /// Gets TCP server active sessions.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this property is accessed.</exception>
        /// <exception cref="InvalidOperationException">Is raised when TCP server is not running and this property is accesed.</exception>
        public TCP_SessionCollection<TCP_ServerSession> Sessions
        {
            get
            {
                if (IsDisposed)
                {
                    throw new ObjectDisposedException("TCP_Server");
                }
                if (!IsRunning)
                {
                    throw new InvalidOperationException("TCP server is not running.");
                }

                return m_pSessions;
            }
        }

        /// <summary>
        /// Gets the time when server was started.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this property is accessed.</exception>
        /// <exception cref="InvalidOperationException">Is raised when TCP server is not running and this property is accesed.</exception>
        public DateTime StartTime
        {
            get
            {
                if (IsDisposed)
                {
                    throw new ObjectDisposedException("TCP_Server");
                }
                if (!IsRunning)
                {
                    throw new InvalidOperationException("TCP server is not running.");
                }

                return m_StartTime;
            }
        }

        /// <summary>
        /// Cleans up any resources being used.
        /// </summary>
        public void Dispose()
        {
            if (IsDisposed)
            {
                return;
            }
            if (IsRunning)
            {
                try
                {
                    Stop();
                }
                catch
                {
                }
            }
            IsDisposed = true;

            // We must call disposed event before we release events.
            try
            {
                OnDisposed();
            }
            catch
            {
                // We never should get exception here, user should handle it, just skip it.
            }

            m_pSessions = null;

            // Release all events.
            Started = null;
            Stopped = null;
            Disposed = null;
            Error = null;
        }

        /// <summary>
        /// Restarts TCP server.
        /// </summary>
        public void Restart()
        {
            Stop();
            Start();
        }

        /// <summary>
        /// Starts TCP server.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this property is accessed.</exception>
        public void Start()
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException("TCP_Server");
            }
            if (IsRunning)
            {
                return;
            }
            IsRunning = true;

            m_StartTime = DateTime.Now;
            m_ConnectionsProcessed = 0;

            ThreadPool.QueueUserWorkItem(new WaitCallback(delegate (object state)
            {
                StartListen();
            }));

            m_pTimer_IdleTimeout = new TimerEx(30000, true);
            m_pTimer_IdleTimeout.Elapsed += new System.Timers.ElapsedEventHandler(m_pTimer_IdleTimeout_Elapsed);
            m_pTimer_IdleTimeout.Enabled = true;

            OnStarted();
        }

        /// <summary>
        /// Stops TCP server, all active connections will be terminated.
        /// </summary>
        public void Stop()
        {
            if (!IsRunning)
            {
                return;
            }
            IsRunning = false;

            // Dispose all old binds.
            foreach (ListeningPoint listeningPoint in m_pListeningPoints.ToArray())
            {
                try
                {
                    listeningPoint.Socket.Close();
                }
                catch (Exception x)
                {
                    OnError(x);
                }
            }
            m_pListeningPoints.Clear();

            m_pTimer_IdleTimeout.Dispose();
            m_pTimer_IdleTimeout = null;

            OnStopped();
        }

        /// <summary>
        /// Raises <b>Disposed</b> event.
        /// </summary>
        protected void OnDisposed()
        {
            Disposed?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// Is called when new incoming session and server maximum allowed connections exceeded.
        /// </summary>
        /// <param name="session">Incoming session.</param>
        /// <remarks>This method allows inhereted classes to report error message to connected client.
        /// Session will be disconnected after this method completes.
        /// </remarks>
        protected virtual void OnMaxConnectionsExceeded(T session)
        {
        }

        /// <summary>
        /// Is called when new incoming session and server maximum allowed connections per connected IP exceeded.
        /// </summary>
        /// <param name="session">Incoming session.</param>
        /// <remarks>This method allows inhereted classes to report error message to connected client.
        /// Session will be disconnected after this method completes.
        /// </remarks>
        protected virtual void OnMaxConnectionsPerIPExceeded(T session)
        {
        }

        /// <summary>
        /// Raises <b>Started</b> event.
        /// </summary>
        protected void OnStarted()
        {
            Started?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// Raises <b>Stopped</b> event.
        /// </summary>
        protected void OnStopped()
        {
            Stopped?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// Is called when session idle check timer triggered.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">Event data.</param>
        private void m_pTimer_IdleTimeout_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                foreach (T session in Sessions.ToArray())
                {
                    try
                    {
                        if (DateTime.Now > session.TcpStream.LastActivity.AddSeconds(m_SessionIdleTimeout))
                        {
                            ;
                            session.OnTimeoutI();
                            // Session didn't dispose itself, so dispose it.
                            if (!session.IsDisposed)
                            {
                                session.Disconnect();
                                session.Dispose();
                            }
                        }
                    }
                    catch
                    {
                    }
                }
            }
            catch (Exception x)
            {
                OnError(x);
            }
        }

        /// <summary>
        /// Raises <b>Error</b> event.
        /// </summary>
        /// <param name="x">Exception happened.</param>
        private void OnError(Exception x)
        {
            Error?.Invoke(this, new Error_EventArgs(x, new System.Diagnostics.StackTrace()));
        }

        /// <summary>
        /// Raises <b>SessionCreated</b> event.
        /// </summary>
        /// <param name="session">TCP server session that was created.</param>
        private void OnSessionCreated(T session)
        {
            SessionCreated?.Invoke(this, new TCP_ServerSessionEventArgs<T>(this, session));
        }

        /// <summary>
        /// Processes specified connection.
        /// </summary>
        /// <param name="socket">Accpeted socket.</param>
        /// <param name="bindInfo">Local bind info what accpeted connection.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>socket</b> or <b>bindInfo</b> is null reference.</exception>
        private void ProcessConnection(Socket socket, IPBindInfo bindInfo)
        {
            if (socket == null)
            {
                throw new ArgumentNullException("socket");
            }
            if (bindInfo == null)
            {
                throw new ArgumentNullException("bindInfo");
            }

            m_ConnectionsProcessed++;

            try
            {
                var session = new T();
                session.Init(this, socket, bindInfo.HostName, bindInfo.SslMode == SslMode.SSL, bindInfo.Certificate);

                // Maximum allowed connections exceeded, reject connection.
                if (m_MaxConnections != 0 && m_pSessions.Count > m_MaxConnections)
                {
                    OnMaxConnectionsExceeded(session);
                    session.Dispose();
                }
                // Maximum allowed connections per IP exceeded, reject connection.
                else if (m_MaxConnectionsPerIP != 0 && m_pSessions.GetConnectionsPerIP(session.RemoteEndPoint.Address) > m_MaxConnectionsPerIP)
                {
                    OnMaxConnectionsPerIPExceeded(session);
                    session.Dispose();
                }
                // Start processing new session.
                else
                {
                    session.Disonnected += new EventHandler(delegate (object sender, EventArgs e)
                    {
                        m_pSessions.Remove((TCP_ServerSession)sender);
                    });
                    m_pSessions.Add(session);

                    OnSessionCreated(session);

                    session.StartI();
                }
            }
            catch (Exception x)
            {
                OnError(x);
            }
        }

        /// <summary>
        /// Starts listening incoming connections. NOTE: All active listening points will be disposed.
        /// </summary>
        private void StartListen()
        {
            try
            {
                // Dispose all old binds.
                foreach (ListeningPoint listeningPoint in m_pListeningPoints.ToArray())
                {
                    try
                    {
                        listeningPoint.Socket.Close();
                    }
                    catch (Exception x)
                    {
                        OnError(x);
                    }
                }
                m_pListeningPoints.Clear();

                // Create new listening points and start accepting connections.
                foreach (IPBindInfo bind in m_pBindings)
                {
                    try
                    {
                        Socket socket = null;
                        if (bind.IP.AddressFamily == AddressFamily.InterNetwork)
                        {
                            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                        }
                        else if (bind.IP.AddressFamily == AddressFamily.InterNetworkV6)
                        {
                            socket = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);
                        }
                        else
                        {
                            // Invalid address family, just skip it.
                            continue;
                        }
                        socket.Bind(new IPEndPoint(bind.IP, bind.Port));
                        socket.Listen(100);

                        var listeningPoint = new ListeningPoint(socket, bind);
                        m_pListeningPoints.Add(listeningPoint);

                        // Create TCP connection acceptors.
                        for (int i = 0; i < 10; i++)
                        {
                            var acceptor = new TCP_Acceptor(socket);
                            acceptor.Tags["bind"] = bind;
                            acceptor.ConnectionAccepted += delegate (object s1, EventArgs<Socket> e1)
                            {
                                // NOTE: We may not use 'bind' variable here, foreach changes it's value before we reach here.
                                ProcessConnection(e1.Value, (IPBindInfo)acceptor.Tags["bind"]);
                            };
                            acceptor.Error += delegate (object s1, ExceptionEventArgs e1)
                            {
                                OnError(e1.Exception);
                            };
                            m_pConnectionAcceptors.Add(acceptor);
                            acceptor.Start();
                        }
                    }
                    catch (Exception x)
                    {
                        // The only exception what we should get there is if socket is in use.
                        OnError(x);
                    }
                }
            }
            catch (Exception x)
            {
                OnError(x);
            }
        }
        /// <summary>
        /// This class holds listening point info.
        /// </summary>
        private class ListeningPoint
        {
            /// <summary>
            /// Default constructor.
            /// </summary>
            /// <param name="socket">Listening socket.</param>
            /// <param name="bind">Bind info what acceped socket.</param>
            public ListeningPoint(Socket socket, IPBindInfo bind)
            {
                Socket = socket;
                BindInfo = bind;
            }

            /// <summary>
            /// Gets socket.
            /// </summary>
            public Socket Socket { get; }

            /// <summary>
            /// Gets bind info.
            /// </summary>
            public IPBindInfo BindInfo { get; }
        }

        /// <summary>
        /// Implements single TCP connection acceptor.
        /// </summary>
        /// <remarks>For higher performance, mutiple acceptors per socket must be created.</remarks>
        private class TCP_Acceptor : IDisposable
        {
            private bool m_IsDisposed;
            private bool m_IsRunning;
            private Socket m_pSocket;
            private SocketAsyncEventArgs m_pSocketArgs;

            /// <summary>
            /// Default constructor.
            /// </summary>
            /// <param name="socket">Socket.</param>
            /// <exception cref="ArgumentNullException">Is raised when <b>socket</b> is null reference.</exception>
            public TCP_Acceptor(Socket socket)
            {
                m_pSocket = socket ?? throw new ArgumentNullException("socket");

                Tags = new Dictionary<string, object>();
            }

            /// <summary>
            /// Cleans up any resources being used.
            /// </summary>
            public void Dispose()
            {
                if (m_IsDisposed)
                {
                    return;
                }
                m_IsDisposed = true;

                m_pSocket = null;
                m_pSocketArgs = null;
                Tags = null;

                ConnectionAccepted = null;
                Error = null;
            }

            /// <summary>
            /// Starts accpeting connections.
            /// </summary>
            /// <exception cref="ObjectDisposedException">Is raised when this calss is disposed and this method is accessed.</exception>
            public void Start()
            {
                if (m_IsDisposed)
                {
                    throw new ObjectDisposedException(GetType().Name);
                }
                if (m_IsRunning)
                {
                    return;
                }
                m_IsRunning = true;

                // Move processing to thread pool.
                ThreadPool.QueueUserWorkItem(delegate (object state)
                {
                    try
                    {
                        if (Net_Utils.IsSocketAsyncSupported())
                        {
                            m_pSocketArgs = new SocketAsyncEventArgs();
                            m_pSocketArgs.Completed += delegate (object s1, SocketAsyncEventArgs e1)
                            {
                                if (m_IsDisposed)
                                {
                                    return;
                                }

                                try
                                {
                                    if (m_pSocketArgs.SocketError == SocketError.Success)
                                    {
                                        OnConnectionAccepted(m_pSocketArgs.AcceptSocket);
                                    }
                                    else
                                    {
                                        OnError(new Exception("Socket error '" + m_pSocketArgs.SocketError + "'."));
                                    }

                                    IOCompletionAccept();
                                }
                                catch (Exception x)
                                {
                                    OnError(x);
                                }
                            };

                            IOCompletionAccept();
                        }
                        else
                        {
                            m_pSocket.BeginAccept(new AsyncCallback(AsyncSocketAccept), null);
                        }
                    }
                    catch (Exception x)
                    {
                        OnError(x);
                    }
                });
            }

            /// <summary>
            /// Accpets connection synchornously(if connection(s) available now) or starts waiting TCP connection asynchronously if no connections at moment.
            /// </summary>
            private void IOCompletionAccept()
            {
                try
                {
                    // We need to clear it, before reuse.
                    m_pSocketArgs.AcceptSocket = null;

                    // Use active worker thread as long as ReceiveFromAsync completes synchronously.
                    // (With this approach we don't have thread context switches while ReceiveFromAsync completes synchronously)
                    while (!m_IsDisposed && !m_pSocket.AcceptAsync(m_pSocketArgs))
                    {
                        if (m_pSocketArgs.SocketError == SocketError.Success)
                        {
                            try
                            {
                                OnConnectionAccepted(m_pSocketArgs.AcceptSocket);

                                // We need to clear it, before reuse.
                                m_pSocketArgs.AcceptSocket = null;
                            }
                            catch (Exception x)
                            {
                                OnError(x);
                            }
                        }
                        else
                        {
                            OnError(new Exception("Socket error '" + m_pSocketArgs.SocketError + "'."));
                        }
                    }
                }
                catch (Exception x)
                {
                    OnError(x);
                }
            }

            /// <summary>
            /// Is called BeginAccept has completed.
            /// </summary>
            /// <param name="ar">The result of the asynchronous operation.</param>
            private void AsyncSocketAccept(IAsyncResult ar)
            {
                if (m_IsDisposed)
                {
                    return;
                }

                try
                {
                    OnConnectionAccepted(m_pSocket.EndAccept(ar));
                }
                catch (Exception x)
                {
                    OnError(x);
                }

                try
                {
                    m_pSocket.BeginAccept(new AsyncCallback(AsyncSocketAccept), null);
                }
                catch (Exception x)
                {
                    OnError(x);
                }
            }

            /// <summary>
            /// Gets user data items.
            /// </summary>
            public Dictionary<string, object> Tags { get; private set; }

            /// <summary>
            /// Is raised when new TCP connection was accepted.
            /// </summary>
            public event EventHandler<EventArgs<Socket>> ConnectionAccepted;

            /// <summary>
            /// Raises <b>ConnectionAccepted</b> event.
            /// </summary>
            /// <param name="socket">Accepted socket.</param>
            private void OnConnectionAccepted(Socket socket)
            {
                ConnectionAccepted?.Invoke(this, new EventArgs<Socket>(socket));
            }

            /// <summary>
            /// Is raised when unhandled error happens.
            /// </summary>
            public event EventHandler<ExceptionEventArgs> Error;

            /// <summary>
            /// Raises <b>Error</b> event.
            /// </summary>
            /// <param name="x">Exception happened.</param>
            private void OnError(Exception x)
            {
                Error?.Invoke(this, new ExceptionEventArgs(x));
            }
        }
    }
}
