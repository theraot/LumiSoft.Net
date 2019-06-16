﻿using System;
using System.IO;
using System.Net;
using System.Threading;

using LumiSoft.Net.IO;
using LumiSoft.Net.TCP;
using LumiSoft.Net.UDP;
using LumiSoft.Net.SIP.Message;

namespace LumiSoft.Net.SIP.Stack
{
    /// <summary>
    /// Implements SIP Flow. Defined in draft-ietf-sip-outbound.
    /// </summary>
    /// <remarks>A Flow is a network protocol layer (layer 4) association
    ///  between two hosts that is represented by the network address and
    ///  port number of both ends and by the protocol.  For TCP, a flow is
    ///  equivalent to a TCP connection.  For UDP a flow is a bidirectional
    ///  stream of datagrams between a single pair of IP addresses and
    ///  ports of both peers.
    /// </remarks>
    public class SIP_Flow : IDisposable
    {
        private long m_BytesWritten;
        private readonly DateTime m_CreateTime;
        private readonly string m_ID = "";
        private readonly bool m_IsServer;
        private DateTime m_LastActivity;
        private bool m_LastCRLF;
        private DateTime m_LastPing;
        private TimerEx m_pKeepAliveTimer;
        private readonly IPEndPoint m_pLocalEP;
        private IPEndPoint m_pLocalPublicEP;
        private readonly object m_pLock = new object();
        private MemoryStream m_pMessage;
        private readonly IPEndPoint m_pRemoteEP;
        private readonly SIP_Stack m_pStack;
        private TCP_Session m_pTcpSession;
        private readonly string m_Transport = "";

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="stack">Owner stack.</param>
        /// <param name="isServer">Specifies if flow is server or client flow.</param>
        /// <param name="localEP">Local IP end point.</param>
        /// <param name="remoteEP">Remote IP end point.</param>
        /// <param name="transport">SIP transport.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>stack</b>,<b>localEP</b>,<b>remoteEP</b>  or <b>transport</b> is null reference.</exception>
        /// <exception cref="ArgumentException">Is raised whena any of the arguments has invalid value.</exception>
        internal SIP_Flow(SIP_Stack stack, bool isServer, IPEndPoint localEP, IPEndPoint remoteEP, string transport)
        {
            if (transport == null)
            {
                throw new ArgumentNullException("transport");
            }

            m_pStack = stack ?? throw new ArgumentNullException("stack");
            m_IsServer = isServer;
            m_pLocalEP = localEP ?? throw new ArgumentNullException("localEP");
            m_pRemoteEP = remoteEP ?? throw new ArgumentNullException("remoteEP");
            m_Transport = transport.ToUpper();

            m_CreateTime = DateTime.Now;
            m_LastActivity = DateTime.Now;
            m_ID = m_pLocalEP.ToString() + "-" + m_pRemoteEP.ToString() + "-" + m_Transport;
            m_pMessage = new MemoryStream();
        }

        /// <summary>
        /// Server TCP,TLS constructor.
        /// </summary>
        /// <param name="stack">Owner stack.</param>
        /// <param name="session">TCP session.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>stack</b> or <b>session</b> is null reference.</exception>
        internal SIP_Flow(SIP_Stack stack, TCP_Session session)
        {
            m_pStack = stack ?? throw new ArgumentNullException("stack");
            m_pTcpSession = session ?? throw new ArgumentNullException("session");

            m_IsServer = true;
            m_pLocalEP = session.LocalEndPoint;
            m_pRemoteEP = session.RemoteEndPoint;
            m_Transport = session.IsSecureConnection ? SIP_Transport.TLS : SIP_Transport.TCP;
            m_CreateTime = DateTime.Now;
            m_LastActivity = DateTime.Now;
            m_ID = m_pLocalEP.ToString() + "-" + m_pRemoteEP.ToString() + "-" + m_Transport;
            m_pMessage = new MemoryStream();

            BeginReadHeader();
        }

        /// <summary>
        /// Is raised when flow is disposing.
        /// </summary>
        public event EventHandler IsDisposing;

        // TODO: BytesReaded

        /// <summary>
        /// Gets how many bytes this flow has sent to remote party.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this property is accessed.</exception>
        public long BytesWritten
        {
            get
            {
                if (IsDisposed)
                {
                    throw new ObjectDisposedException(GetType().Name);
                }

                return m_BytesWritten;
            }
        }

        /// <summary>
        /// Gets time when flow was created.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this property is accessed.</exception>
        public DateTime CreateTime
        {
            get
            {
                if (IsDisposed)
                {
                    throw new ObjectDisposedException(GetType().Name);
                }

                return m_CreateTime;
            }
        }

        /// <summary>
        /// Gets flow ID.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this property is accessed.</exception>
        public string ID
        {
            get
            {
                if (IsDisposed)
                {
                    throw new ObjectDisposedException(GetType().Name);
                }

                return m_ID;
            }
        }

        /// <summary>
        /// Gets if this object is disposed.
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// Gets if flow is reliable transport.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this property is accessed.</exception>
        public bool IsReliable
        {
            get
            {
                if (IsDisposed)
                {
                    throw new ObjectDisposedException(GetType().Name);
                }

                return m_Transport != SIP_Transport.UDP;
            }
        }

        /// <summary>
        /// Gets if this connection is secure.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this property is accessed.</exception>
        public bool IsSecure
        {
            get
            {
                if (IsDisposed)
                {
                    throw new ObjectDisposedException(GetType().Name);
                }

                if (m_Transport == SIP_Transport.TLS)
                {
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Gets if this flow is server flow or client flow.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this property is accessed.</exception>
        public bool IsServer
        {
            get
            {
                if (IsDisposed)
                {
                    throw new ObjectDisposedException(GetType().Name);
                }

                return m_IsServer;
            }
        }

        /// <summary>
        /// Gets when flow had last(send or receive) activity.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this property is accessed.</exception>
        public DateTime LastActivity
        {
            get
            {
                if (IsDisposed)
                {
                    throw new ObjectDisposedException(GetType().Name);
                }

                if (m_Transport == SIP_Transport.TCP || m_Transport == SIP_Transport.TLS)
                {
                    return m_pTcpSession.LastActivity;
                }

                return m_LastActivity;
            }
        }

        /// <summary>
        /// Gets time when last ping request was sent.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this property is accessed.</exception>
        public DateTime LastPing
        {
            get
            {
                if (IsDisposed)
                {
                    throw new ObjectDisposedException(GetType().Name);
                }

                return m_LastPing;
            }
        }

        /// <summary>
        /// Gets flow local IP end point.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this property is accessed.</exception>
        public IPEndPoint LocalEP
        {
            get
            {
                if (IsDisposed)
                {
                    throw new ObjectDisposedException(GetType().Name);
                }

                return m_pLocalEP;
            }
        }

        /// <summary>
        /// Gets local EP what actually visible to <b>RemoteEP</b>. This value is different from <b>LocalEP</b> when stack is behind NAT.
        /// </summary>
        public IPEndPoint LocalPublicEP
        {
            get
            {
                if (IsDisposed)
                {
                    throw new ObjectDisposedException(GetType().Name);
                }

                // We may not lock here, because dead lock will happen. Client transaction runs on thread pool threads.
                //lock(m_pLock){
                if (m_pLocalPublicEP != null)
                {
                    return m_pLocalPublicEP;
                }

                m_pLocalPublicEP = LocalEP;

                try
                {
                    var completionWaiter = new AutoResetEvent(false);
                    // Create OPTIONS request
                    var optionsRequest = m_pStack.CreateRequest(SIP_Methods.OPTIONS, new SIP_t_NameAddress("sip:ping@publicIP.com"), new SIP_t_NameAddress("sip:ping@publicIP.com"));
                    optionsRequest.MaxForwards = 0;
                    var optionsTransaction = m_pStack.TransactionLayer.CreateClientTransaction(this, optionsRequest, true);
                    optionsTransaction.ResponseReceived += new EventHandler<SIP_ResponseReceivedEventArgs>(delegate (object s, SIP_ResponseReceivedEventArgs e)
                    {
                        var via = e.Response.Via.GetTopMostValue();

                        var publicEP = new IPEndPoint(via.Received == null ? LocalEP.Address : via.Received, via.RPort > 0 ? via.RPort : LocalEP.Port);
                        // Set public EP port only if public IP is also different from local EP.
                        if (!LocalEP.Address.Equals(publicEP.Address))
                        {
                            m_pLocalPublicEP = publicEP;
                        }

                        completionWaiter.Set();
                    });
                    optionsTransaction.StateChanged += new EventHandler(delegate (object s, EventArgs e)
                    {
                        if (optionsTransaction.State == SIP_TransactionState.Terminated)
                        {
                            completionWaiter.Set();
                        }
                    });
                    optionsTransaction.Start();

                    // Wait OPTIONS request to complete.
                    completionWaiter.WaitOne();
                    completionWaiter.Close();
                }
                catch
                {
                }

                return m_pLocalPublicEP;
                // }
            }
        }

        /// <summary>
        /// Gets flow remote IP end point.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this property is accessed.</exception>
        public IPEndPoint RemoteEP
        {
            get
            {
                if (IsDisposed)
                {
                    throw new ObjectDisposedException(GetType().Name);
                }

                return m_pRemoteEP;
            }
        }

        /// <summary>
        /// Gets or sets if flow sends keep-alive packets.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this property is accessed.</exception>
        public bool SendKeepAlives
        {
            get
            {
                if (IsDisposed)
                {
                    throw new ObjectDisposedException(GetType().Name);
                }

                return m_pKeepAliveTimer != null;
            }

            set
            {
                if (IsDisposed)
                {
                    throw new ObjectDisposedException(GetType().Name);
                }

                if (value)
                {
                    if (m_pKeepAliveTimer == null)
                    {
                        m_pKeepAliveTimer = new TimerEx(15000, true);
                        m_pKeepAliveTimer.Elapsed += delegate (object s, System.Timers.ElapsedEventArgs e)
                        {
                            try
                            {
                                // Log:
                                m_pStack.TransportLayer.Stack.Logger?.AddWrite("", null, 2, "Flow [id='" + ID + "'] sent \"ping\"", LocalEP, RemoteEP);

                                SendInternal(new[] { (byte)'\r', (byte)'\n', (byte)'\r', (byte)'\n' });
                            }
                            catch
                            {
                                // We don't care about errors here.
                            }
                        };
                        m_pKeepAliveTimer.Enabled = true;
                    }
                }
                else
                {
                    if (m_pKeepAliveTimer != null)
                    {
                        m_pKeepAliveTimer.Dispose();
                        m_pKeepAliveTimer = null;
                    }
                }
            }
        }

        /// <summary>
        /// Gets flow transport.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this property is accessed.</exception>
        public string Transport
        {
            get
            {
                if (IsDisposed)
                {
                    throw new ObjectDisposedException(GetType().Name);
                }

                return m_Transport;
            }
        }

        /// <summary>
        /// Cleans up any resources being used.
        /// </summary>
        public void Dispose()
        {
            lock (m_pLock)
            {
                if (IsDisposed)
                {
                    return;
                }
                OnDisposing();
                IsDisposed = true;

                if (m_pTcpSession != null)
                {
                    m_pTcpSession.Dispose();
                    m_pTcpSession = null;
                }
                m_pMessage = null;
                if (m_pKeepAliveTimer != null)
                {
                    m_pKeepAliveTimer.Dispose();
                    m_pKeepAliveTimer = null;
                }
            }
        }

        /// <summary>
        /// Sends specified request to flow remote end point.
        /// </summary>
        /// <param name="request">SIP request to send.</param>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this method is accessed.</exception>
        /// <exception cref="ArgumentNullException">Is raised when <b>request</b> is null reference.</exception>
        public void Send(SIP_Request request)
        {
            lock (m_pLock)
            {
                if (IsDisposed)
                {
                    throw new ObjectDisposedException(GetType().Name);
                }
                if (request == null)
                {
                    throw new ArgumentNullException("request");
                }

                SendInternal(request.ToByteData());
            }
        }

        /// <summary>
        /// Sends specified response to flow remote end point.
        /// </summary>
        /// <param name="response">SIP response to send.</param>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this method is accessed.</exception>
        /// <exception cref="ArgumentNullException">Is raised when <b>response</b> is null reference.</exception>
        public void Send(SIP_Response response)
        {
            lock (m_pLock)
            {
                if (IsDisposed)
                {
                    throw new ObjectDisposedException(GetType().Name);
                }
                if (response == null)
                {
                    throw new ArgumentNullException("response");
                }

                SendInternal(response.ToByteData());
                m_LastPing = DateTime.Now;
            }
        }

        /// <summary>
        /// Send ping request to flow remote end point.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this method is accessed.</exception>
        public void SendPing()
        {
            lock (m_pLock)
            {
                if (IsDisposed)
                {
                    throw new ObjectDisposedException(GetType().Name);
                }

                // Log:
                m_pStack.TransportLayer.Stack.Logger?.AddWrite("", null, 2, "Flow [id='" + ID + "'] sent \"ping\"", LocalEP, RemoteEP);

                SendInternal(new[] { (byte)'\r', (byte)'\n', (byte)'\r', (byte)'\n' });
            }
        }

        /// <summary>
        /// This method is called when flow gets new UDP packet.
        /// </summary>
        /// <param name="e">Event data..</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>e</b> is null reference.</exception>
        internal void OnUdpPacketReceived(UDP_e_PacketReceived e)
        {
            if (e == null)
            {
                throw new ArgumentNullException("e");
            }

            m_LastActivity = DateTime.Now;

            var data = new byte[e.Count];
            Array.Copy(e.Buffer, data, e.Count);

            m_pStack.TransportLayer.OnMessageReceived(this, data);
        }

        /// <summary>
        /// Sends specified data to the remote end point.
        /// </summary>
        /// <param name="data">Data to send.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>data</b> is null reference.</exception>
        internal void SendInternal(byte[] data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            try
            {
                if (m_Transport == SIP_Transport.UDP)
                {
                    m_pStack.TransportLayer.UdpServer.SendPacket(m_pLocalEP, data, 0, data.Length, m_pRemoteEP);
                }
                else if (m_Transport == SIP_Transport.TCP)
                {
                    m_pTcpSession.TcpStream.Write(data, 0, data.Length);
                }
                else if (m_Transport == SIP_Transport.TLS)
                {
                    m_pTcpSession.TcpStream.Write(data, 0, data.Length);
                }

                m_BytesWritten += data.Length;
            }
            catch (IOException x)
            {
                Dispose();

                throw x;
            }
        }

        /// <summary>
        /// Starts flow processing.
        /// </summary>
        internal void Start()
        {
            // Move processing to thread pool.
            var startLock = new AutoResetEvent(false);
            ThreadPool.QueueUserWorkItem(new WaitCallback(delegate (object state)
            {
                lock (m_pLock)
                {
                    startLock.Set();

                    // TCP / TLS client, connect to remote end point.
                    if (!m_IsServer && m_Transport != SIP_Transport.UDP)
                    {
                        try
                        {
                            var client = new TCP_Client();
                            client.Connect(m_pLocalEP, m_pRemoteEP, m_Transport == SIP_Transport.TLS);

                            m_pTcpSession = client;

                            BeginReadHeader();
                        }
                        catch
                        {
                            Dispose();
                        }
                    }
                }
            }));
            startLock.WaitOne();
            startLock.Close();
        }

        /// <summary>
        /// This method is called when SIP message data reading has completed.
        /// </summary>
        /// <param name="asyncResult">An IAsyncResult that represents an asynchronous call.</param>
        private void BeginReadData_Completed(IAsyncResult asyncResult)
        {
            try
            {
                m_pTcpSession.TcpStream.EndReadFixedCount(asyncResult);

                var messageData = m_pMessage.ToArray();
                // Wait for new SIP message.
                BeginReadHeader();

                m_pStack.TransportLayer.OnMessageReceived(this, messageData);
            }
            catch
            {
                Dispose();
            }
        }

        /// <summary>
        /// Starts reading SIP message header.
        /// </summary>
        private void BeginReadHeader()
        {
            // Clear old data.
            m_pMessage.SetLength(0);

            // Start reading SIP message header.
            m_pTcpSession.TcpStream.BeginReadHeader(
                m_pMessage,
                m_pStack.TransportLayer.Stack.MaximumMessageSize,
                SizeExceededAction.JunkAndThrowException,
                new AsyncCallback(BeginReadHeader_Completed),
                null
            );
        }

        /// <summary>
        /// This method is called when SIP message header reading has completed.
        /// </summary>
        /// <param name="asyncResult">An IAsyncResult that represents an asynchronous call.</param>
        private void BeginReadHeader_Completed(IAsyncResult asyncResult)
        {
            try
            {
                int countStored = m_pTcpSession.TcpStream.EndReadHeader(asyncResult);

                // We got CRLF(ping or pong).
                if (countStored == 0)
                {
                    // We have ping request.
                    if (IsServer)
                    {
                        // We have full ping request.
                        if (m_LastCRLF)
                        {
                            m_LastCRLF = false;

                            m_pStack.TransportLayer.OnMessageReceived(this, new[] { (byte)'\r', (byte)'\n', (byte)'\r', (byte)'\n' });
                        }
                        // We have first CRLF of ping request.
                        else
                        {
                            m_LastCRLF = true;
                        }
                    }
                    // We got pong to our ping request.
                    else
                    {
                        m_pStack.TransportLayer.OnMessageReceived(this, new[] { (byte)'\r', (byte)'\n' });
                    }

                    // Wait for new SIP message.
                    BeginReadHeader();
                }
                // We have SIP message header.
                else
                {
                    m_LastCRLF = false;

                    // Add header terminator blank line.
                    m_pMessage.Write(new[] { (byte)'\r', (byte)'\n' }, 0, 2);

                    m_pMessage.Position = 0;
                    var contentLengthValue = MIME.MIME_Utils.ParseHeaderField("Content-Length:", m_pMessage);
                    m_pMessage.Position = m_pMessage.Length;

                    int contentLength = 0;

                    // Read message body.
                    if (contentLengthValue != "")
                    {
                        contentLength = Convert.ToInt32(contentLengthValue);
                    }

                    // Start reading message body.
                    if (contentLength > 0)
                    {
                        // Read body data.
                        m_pTcpSession.TcpStream.BeginReadFixedCount(m_pMessage, contentLength, new AsyncCallback(BeginReadData_Completed), null);
                    }
                    // Message with no body.
                    else
                    {
                        var messageData = m_pMessage.ToArray();
                        // Wait for new SIP message.
                        BeginReadHeader();

                        m_pStack.TransportLayer.OnMessageReceived(this, messageData);
                    }
                }
            }
            catch
            {
                Dispose();
            }
        }

        /// <summary>
        /// Raises <b>Disposed</b> event.
        /// </summary>
        private void OnDisposing()
        {
            IsDisposing?.Invoke(this, new EventArgs());
        }
    }
}
