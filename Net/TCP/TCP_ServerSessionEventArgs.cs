using System;

namespace LumiSoft.Net.TCP
{
    /// <summary>
    /// This class provides data to .... .
    /// </summary>
    public class TCP_ServerSessionEventArgs<T> : EventArgs where T : TCP_ServerSession,new()
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="server">TCP server.</param>
        /// <param name="session">TCP server session.</param>
        internal TCP_ServerSessionEventArgs(TCP_Server<T> server,T session)
        {
            Server  = server;
            Session = session;
        }


        /// <summary>
        /// Gets TCP server.
        /// </summary>
        public TCP_Server<T> Server { get; }

        /// <summary>
        /// Gets TCP server session.
        /// </summary>
        public T Session { get; }
    }
}
