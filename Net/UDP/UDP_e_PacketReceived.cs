using System;
using System.Net;
using System.Net.Sockets;

namespace LumiSoft.Net.UDP
{
    /// <summary>
    /// This class provides data for the <see cref="UDP_DataReceiver.PacketReceived"/> event.
    /// </summary>
    public class UDP_e_PacketReceived : EventArgs
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        internal UDP_e_PacketReceived()
        {
        }

        /// <summary>
        /// Reuses this class.
        /// </summary>
        /// <param name="socket">Socket which received data.</param>
        /// <param name="buffer">Data buffer.</param>
        /// <param name="count">Number of bytes stored in <b>buffer</b></param>
        /// <param name="remoteEP">Remote IP end point from where data was received.</param>
        internal void Reuse(Socket socket,byte[] buffer,int count,IPEndPoint remoteEP)
        {        
            Socket   = socket;
            Buffer   = buffer;
            Count     = count;
            RemoteEP = remoteEP;
        }

        /// <summary>
        /// Gets socket which received data.
        /// </summary>
        public Socket Socket { get; private set; }

        /// <summary>
        /// Gets data buffer.
        /// </summary>
        public byte[] Buffer { get; private set; }

        /// <summary>
        /// Gets number of bytes stored to <b>Buffer</b>.
        /// </summary>
        public int Count { get; private set; }

        /// <summary>
        /// Gets remote host from where data was received.
        /// </summary>
        public IPEndPoint RemoteEP { get; private set; }
    }
}
