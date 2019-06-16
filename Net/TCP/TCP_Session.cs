using System;
using System.Net;
using System.Security.Principal;

using LumiSoft.Net.IO;

namespace LumiSoft.Net.TCP
{
    /// <summary>
    /// This is base class for TCP_Client and TCP_ServerSession.
    /// </summary>
    public abstract class TCP_Session : IDisposable
    {
        /// <summary>
        /// Gets session authenticated user identity , returns null if not authenticated.
        /// </summary>
        public virtual GenericIdentity AuthenticatedUserIdentity
        {
            get { return null; }
        }

        /// <summary>
        /// Gets the time when session was connected.
        /// </summary>
        public abstract DateTime ConnectTime
        {
            get;
        }

        /// <summary>
        /// Gets session ID.
        /// </summary>
        public abstract string ID
        {
            get;
        }

        /// <summary>
        /// Gets if this session is authenticated.
        /// </summary>
        public bool IsAuthenticated
        {
            get { return AuthenticatedUserIdentity != null; }
        }

        /// <summary>
        /// Gets if session is connected.
        /// </summary>
        public abstract bool IsConnected
        {
            get;
        }

        /// <summary>
        /// Gets if this session TCP connection is secure connection.
        /// </summary>
        public virtual bool IsSecureConnection
        {
            get { return false; }
        }

        /// <summary>
        /// Gets the last time when data was sent or received.
        /// </summary>
        public abstract DateTime LastActivity
        {
            get;
        }

        /// <summary>
        /// Gets session local IP end point.
        /// </summary>
        public abstract IPEndPoint LocalEndPoint
        {
            get;
        }

        /// <summary>
        /// Gets session remote IP end point.
        /// </summary>
        public abstract IPEndPoint RemoteEndPoint
        {
            get;
        }

        /// <summary>
        /// Gets TCP stream which must be used to send/receive data through this session.
        /// </summary>
        public abstract SmartStream TcpStream
        {
            get;
        }

        /// <summary>
        /// Disconnects session.
        /// </summary>
        public abstract void Disconnect();
        /// <summary>
        /// Cleans up any resources being used.
        /// </summary>
        public abstract void Dispose();
    }
}
