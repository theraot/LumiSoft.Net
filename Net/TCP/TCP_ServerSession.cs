using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading;

using LumiSoft.Net.IO;
using LumiSoft.Net.Log;

namespace LumiSoft.Net.TCP
{
    /// <summary>
    /// This class implements generic TCP server session.
    /// </summary>
    public class TCP_ServerSession : TCP_Session
    {
        private bool                      m_IsTerminated;
        private object                    m_pServer;
        private string                    m_ID            = "";
        private DateTime                  m_ConnectTime;
        private string                    m_LocalHostName = "";
        private IPEndPoint                m_pLocalEP;
        private IPEndPoint                m_pRemoteEP;
        private bool                      m_IsSsl;
        private bool                      m_IsSecure;
        private X509Certificate           m_pCertificate;
        private SmartStream               m_pTcpStream;
        private Dictionary<string,object> m_pTags;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public TCP_ServerSession()
        {
            m_pTags = new Dictionary<string,object>();
        }

        /// <summary>
        /// Cleans up any resources being used.
        /// </summary>
        public override void Dispose()
        {
            if(IsDisposed){
                return;
            }
            if(!m_IsTerminated){
                try{
                    Disconnect();
                }
                catch{
                    // Skip disconnect errors.
                }
            }
            IsDisposed = true;

            // We must call disposed event before we release events.
            try{
                OnDisposed();
            }
            catch{
                // We never should get exception here, user should handle it, just skip it.
            }

            m_pLocalEP = null;
            m_pRemoteEP = null;
            m_pCertificate = null;
            if(m_pTcpStream != null){
                m_pTcpStream.Dispose();
            }
            m_pTcpStream = null;
            m_pTags = null;

            // Release events.
            IdleTimeout = null;
            Disonnected  = null;
            Disposed    = null;
        }

        /// <summary>
        /// Initializes session. This method is called from TCP_Server when new session created.
        /// </summary>
        /// <param name="server">Owner TCP server.</param>
        /// <param name="socket">Connected socket.</param>
        /// <param name="hostName">Local host name.</param>
        /// <param name="ssl">Specifies if session should switch to SSL.</param>
        /// <param name="certificate">SSL certificate.</param>
        internal void Init(object server,Socket socket,string hostName,bool ssl,X509Certificate certificate)
        {   
            // NOTE: We may not raise any event here !
            
            m_pServer       = server;
            m_LocalHostName = hostName;
            m_IsSsl         = ssl;
            m_ID            = Guid.NewGuid().ToString();
            m_ConnectTime   = DateTime.Now;
            m_pLocalEP      = (IPEndPoint)socket.LocalEndPoint;
            m_pRemoteEP     = (IPEndPoint)socket.RemoteEndPoint;            
            m_pCertificate  = certificate;

            socket.ReceiveBufferSize = 32000;
            socket.SendBufferSize = 32000;

            m_pTcpStream = new SmartStream(new NetworkStream(socket,true),true);
        }

        /// <summary>
        /// This method is called from TCP server when session should start processing incoming connection.
        /// </summary>
        internal void StartI()
        {
            if(m_IsSsl){
                // Log
                LogAddText("Starting SSL negotiation now.");

                var startTime = DateTime.Now;

                // Create delegate which is called when SwitchToSecureAsync has completed.
                Action<SwitchToSecureAsyncOP> switchSecureCompleted = delegate(SwitchToSecureAsyncOP e){
                    try{
                        // Operation failed.
                        if(e.Error != null){
                            LogAddException(e.Error);
                            Disconnect();
                        }
                        // Operation suceeded.
                        else{
                            // Log
                            LogAddText("SSL negotiation completed successfully in " + (DateTime.Now - startTime).TotalSeconds.ToString("f2") + " seconds.");

                            Start();
                        }
                    }
                    catch(Exception x){
                        LogAddException(x);
                        Disconnect();
                    }
                };

                var op = new SwitchToSecureAsyncOP();
                op.CompletedAsync += delegate(object sender,EventArgs<SwitchToSecureAsyncOP> e){
                    switchSecureCompleted(op);
                };
                // Switch to secure completed synchronously.
                if(!SwitchToSecureAsync(op)){
                    switchSecureCompleted(op);
                }
            }
            else{
                Start();
            }
        }

        /// <summary>
        /// This method is called from TCP server when session should start processing incoming connection.
        /// </summary>
        protected virtual void Start()
        {
        }

        /// <summary>
        /// Switches session to secure connection.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this method is accessed.</exception>
        /// <exception cref="InvalidOperationException">Is raised when connection is already secure or when SSL certificate is not specified.</exception>
        public void SwitchToSecure()
        {
            if(IsDisposed){
                throw new ObjectDisposedException("TCP_ServerSession");
            }
            if(m_IsSecure){
                throw new InvalidOperationException("Session is already SSL/TLS.");
            }
            if(m_pCertificate == null){
                throw new InvalidOperationException("There is no certificate specified.");
            }

            var wait = new ManualResetEvent(false);
            using (SwitchToSecureAsyncOP op = new SwitchToSecureAsyncOP()){
                op.CompletedAsync += delegate(object s1,EventArgs<SwitchToSecureAsyncOP> e1){
                    wait.Set();
                };
                if(!SwitchToSecureAsync(op)){
                    wait.Set();
                }
                wait.WaitOne();
                wait.Close();

                if(op.Error != null){
                    throw op.Error;
                }
            }
        }

        /// <summary>
        /// This class represents <see cref="TCP_ServerSession.SwitchToSecureAsync"/> asynchronous operation.
        /// </summary>
        public class SwitchToSecureAsyncOP : IDisposable,IAsyncOP
        {
            private readonly object            m_pLock         = new object();
            private bool              m_RiseCompleted;
            private Exception         m_pException;
            private TCP_ServerSession m_pTcpSession;
            private SslStream         m_pSslStream;

            /// <summary>
            /// Cleans up any resource being used.
            /// </summary>
            public void Dispose()
            {
                if(State == AsyncOP_State.Disposed){
                    return;
                }
                SetState(AsyncOP_State.Disposed);
                
                m_pException  = null;
                m_pTcpSession = null;
                m_pSslStream  = null;

                CompletedAsync = null;
            }

            /// <summary>
            /// Starts operation processing.
            /// </summary>
            /// <param name="owner">Owner TCP session.</param>
            /// <returns>Returns true if asynchronous operation in progress or false if operation completed synchronously.</returns>
            /// <exception cref="ArgumentNullException">Is raised when <b>owner</b> is null reference.</exception>
            internal bool Start(TCP_ServerSession owner)
            {
                m_pTcpSession = owner ?? throw new ArgumentNullException("owner");

                SetState(AsyncOP_State.Active);

                try{
                    m_pSslStream = new SslStream(m_pTcpSession.TcpStream.SourceStream,true);
                    m_pSslStream.BeginAuthenticateAsServer(m_pTcpSession.m_pCertificate,BeginAuthenticateAsServerCompleted,null);
                }
                catch(Exception x){
                    m_pException = x;
                    SetState(AsyncOP_State.Completed);
                }

                // Set flag rise CompletedAsync event flag. The event is raised when async op completes.
                // If already completed sync, that flag has no effect.
                lock(m_pLock){
                    m_RiseCompleted = true;

                    return State == AsyncOP_State.Active;
                }
            }

            /// <summary>
            /// Sets operation state.
            /// </summary>
            /// <param name="state">New state.</param>
            private void SetState(AsyncOP_State state)
            {
                if(State == AsyncOP_State.Disposed){
                    return;
                }

                lock(m_pLock){
                    State = state;

                    if(State == AsyncOP_State.Completed && m_RiseCompleted){
                        OnCompletedAsync();
                    }
                }
            }

            /// <summary>
            /// This method is called when "BeginAuthenticateAsServer" has completed.
            /// </summary>
            /// <param name="ar">Asynchronous result.</param>
            private void BeginAuthenticateAsServerCompleted(IAsyncResult ar)
            {
                try{
                    m_pSslStream.EndAuthenticateAsServer(ar);

                    // Close old stream, but leave source stream open.
                    m_pTcpSession.m_pTcpStream.IsOwner = false;
                    m_pTcpSession.m_pTcpStream.Dispose();

                    m_pTcpSession.m_IsSecure = true;
                    m_pTcpSession.m_pTcpStream = new SmartStream(m_pSslStream,true);
                }
                catch(Exception x){
                    m_pException = x;                    
                }

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
                get{ 
                    if(State == AsyncOP_State.Disposed){
                        throw new ObjectDisposedException(GetType().Name);
                    }
                    if(State != AsyncOP_State.Completed){
                        throw new InvalidOperationException("Property 'Error' is accessible only in 'AsyncOP_State.Completed' state.");
                    }

                    return m_pException; 
                }
            }

            /// <summary>
            /// Is called when asynchronous operation has completed.
            /// </summary>
            public event EventHandler<EventArgs<SwitchToSecureAsyncOP>> CompletedAsync;

            /// <summary>
            /// Raises <b>CompletedAsync</b> event.
            /// </summary>
            private void OnCompletedAsync()
            {
                if(CompletedAsync != null){
                    CompletedAsync(this,new EventArgs<SwitchToSecureAsyncOP>(this));
                }
            }
        }

        /// <summary>
        /// Starts switching connection to secure.
        /// </summary>
        /// <param name="op">Asynchronous operation.</param>
        /// <returns>Returns true if aynchronous operation is pending (The <see cref="SwitchToSecureAsyncOP.CompletedAsync"/> event is raised upon completion of the operation).
        /// Returns false if operation completed synchronously.</returns>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and and this method is accessed.</exception>
        /// <exception cref="InvalidOperationException">Is raised when connection is already secure or when SSL certificate is not specified.</exception>
        /// <exception cref="ArgumentNullException">Is raised when <b>op</b> is null reference.</exception>
        public bool SwitchToSecureAsync(SwitchToSecureAsyncOP op)
        {
            if(IsDisposed){
                throw new ObjectDisposedException(GetType().Name);
            }
            if(IsSecureConnection){
                throw new InvalidOperationException("Connection is already secure.");
            }            
            if(m_pCertificate == null){
                throw new InvalidOperationException("There is no certificate specified.");
            }
            if(op == null){
                throw new ArgumentNullException("op");
            }
            if(op.State != AsyncOP_State.WaitingForStart){
                throw new ArgumentException("Invalid argument 'op' state, 'op' must be in 'AsyncOP_State.WaitingForStart' state.","op");
            }

            return op.Start(this);
        }

        /// <summary>
        /// Disconnects session.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this property is accessed.</exception>
        public override void Disconnect()
        {
            Disconnect(null);
        }

        /// <summary>
        /// Disconnects session.
        /// </summary>
        /// <param name="text">Text what is sent to connected host before disconnecting.</param>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this method is accessed.</exception>
        public void Disconnect(string text)
        {
            if(IsDisposed){
                throw new ObjectDisposedException("TCP_ServerSession");
            }
            if(m_IsTerminated){
                return;
            }
            m_IsTerminated = true;

            if(!string.IsNullOrEmpty(text)){
                try{                    
                    m_pTcpStream.Write(text);
                }
                catch(Exception x){
                    OnError(x);
                }
            }

            try{
                OnDisonnected();
            }
            catch(Exception x){
                // We never should get exception here, user should handle it.
                OnError(x);
            }

            Dispose();
        }

        /// <summary>
        /// This method is called when specified session times out.
        /// </summary>
        /// <remarks>
        /// This method allows inhereted classes to report error message to connected client.
        /// Session will be disconnected after this method completes.
        /// </remarks>
        protected virtual void OnTimeout()
        {
        }

        /// <summary>
        /// Just calls <b>OnTimeout</b> method.
        /// </summary>
        internal virtual void OnTimeoutI()
        {
            OnTimeout();
        }

        /// <summary>
        /// Logs specified text.
        /// </summary>
        /// <param name="text">text to log.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>text</b> is null reference.</exception>
        private void LogAddText(string text)
        {
            if(text == null){
                throw new ArgumentNullException("text");
            }
            
            try{
                var logger = Server.GetType().GetProperty("Logger").GetValue(Server,null);
                if (logger != null){
                    ((Logger)logger).AddText(
                        ID,
                        AuthenticatedUserIdentity,
                        text,                        
                        LocalEndPoint,
                        RemoteEndPoint
                    );
                }
            }
            catch{
            }
        }

        /// <summary>
        /// Logs specified exception.
        /// </summary>
        /// <param name="exception">Exception to log.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>exception</b> is null reference.</exception>
        private void LogAddException(Exception exception)
        {
            if(exception == null){
                throw new ArgumentNullException("exception");
            }
            
            try{
                var logger = Server.GetType().GetProperty("Logger").GetValue(Server,null);
                if (logger != null){
                    ((Logger)logger).AddException(
                        ID,
                        AuthenticatedUserIdentity,
                        exception.Message,                        
                        LocalEndPoint,
                        RemoteEndPoint,
                        exception
                    );
                }
            }
            catch{
            }
        }

        /// <summary>
        /// Gets if TCP server session is disposed.
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// Gets owner TCP server.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this property is accessed.</exception>
        public object Server
        {
            get{ 
                if(IsDisposed){
                    throw new ObjectDisposedException("TCP_ServerSession");
                }

                return m_pServer; 
            }
        }

        /// <summary>
        /// Gets local host name.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this property is accessed.</exception>
        public string LocalHostName
        {
            get{
                if(IsDisposed){
                    throw new ObjectDisposedException("TCP_ServerSession");
                }

                return m_LocalHostName;
            }
        }

        /// <summary>
        /// Gets session certificate.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this property is accessed.</exception>
        public X509Certificate Certificate
        {
            get{  
                if(IsDisposed){
                    throw new ObjectDisposedException("TCP_ServerSession");
                }
                
                return m_pCertificate; 
            }
        }

        /// <summary>
        /// Gets or sets user data.
        /// </summary>
        public object Tag { get; set; }

        /// <summary>
        /// Gets user data items collection.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this property is accessed.</exception>
        public Dictionary<string,object> Tags
        {
            get{ 
                if(IsDisposed){
                    throw new ObjectDisposedException("TCP_ServerSession");
                }

                return m_pTags; 
            }
        }

        /// <summary>
        /// Gets if session is connected.
        /// </summary>
        public override bool IsConnected
        {
            get{ return true; }
        }

        /// <summary>
        /// Gets session ID.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this property is accessed.</exception>
        public override string ID
        {
            get{ 
                if(IsDisposed){
                    throw new ObjectDisposedException("TCP_ServerSession");
                }

                return m_ID; 
            }
        }

        /// <summary>
        /// Gets the time when session was connected.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this property is accessed.</exception>
        public override DateTime ConnectTime
        {
            get{ 
                if(IsDisposed){
                    throw new ObjectDisposedException("TCP_ServerSession");
                }

                return m_ConnectTime; 
            }
        }

        /// <summary>
        /// Gets the last time when data was sent or received.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this property is accessed.</exception>
        public override DateTime LastActivity
        {
            get{
                if(IsDisposed){
                    throw new ObjectDisposedException("TCP_ServerSession");
                }
 
                return m_pTcpStream.LastActivity; 
            }
        }

        /// <summary>
        /// Gets session local IP end point.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this property is accessed.</exception>
        public override IPEndPoint LocalEndPoint
        {
            get{ 
                if(IsDisposed){
                    throw new ObjectDisposedException("TCP_ServerSession");
                }

                return m_pLocalEP; 
            }
        }

        /// <summary>
        /// Gets session remote IP end point.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this property is accessed.</exception>
        public override IPEndPoint RemoteEndPoint
        {
            get{
                if(IsDisposed){
                    throw new ObjectDisposedException("TCP_ServerSession");
                }
                
                return m_pRemoteEP; 
            }
        }
        
        /// <summary>
        /// Gets if this session TCP connection is secure connection.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this property is accessed.</exception>
        public override bool IsSecureConnection
        {
            get{ 
                if(IsDisposed){
                    throw new ObjectDisposedException("TCP_ServerSession");
                }

                return m_IsSecure; 
            }
        }
                
        /// <summary>
        /// Gets TCP stream which must be used to send/receive data through this session.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this property is accessed.</exception>
        public override SmartStream TcpStream
        {
            get{  
                if(IsDisposed){
                    throw new ObjectDisposedException("TCP_ServerSession");
                }
                
                return m_pTcpStream; 
            }
        }

        /// <summary>
        /// This event is raised when session idle(no activity) timeout reached.
        /// </summary>
        public event EventHandler IdleTimeout;

        /// <summary>
        /// Raises <b>IdleTimeout</b> event.
        /// </summary>
        private void OnIdleTimeout()
        {
            if(IdleTimeout != null){
                IdleTimeout(this,new EventArgs());
            }
        }

        /// <summary>
        /// This event is raised when session has disconnected and will be disposed soon.
        /// </summary>
        public event EventHandler Disonnected;

        /// <summary>
        /// Raises <b>Disonnected</b> event.
        /// </summary>
        private void OnDisonnected()
        {
            if(Disonnected != null){
                Disonnected(this,new EventArgs());
            }
        }

        /// <summary>
        /// This event is raised when session has disposed.
        /// </summary>
        public event EventHandler Disposed;

        /// <summary>
        /// Raises <b>Disposed</b> event.
        /// </summary>
        private void OnDisposed()
        {
            if(Disposed != null){
                Disposed(this,new EventArgs());
            }
        }

        /// <summary>
        /// This event is raised when TCP server session has unknown unhandled error.
        /// </summary>
        public event ErrorEventHandler Error;

        /// <summary>
        /// Raises <b>Error</b> event.
        /// </summary>
        /// <param name="x">Exception happened.</param>
        protected virtual void OnError(Exception x)
        {
            if(Error != null){
                Error(this,new Error_EventArgs(x,new System.Diagnostics.StackTrace()));
            }
        }
    }
}
