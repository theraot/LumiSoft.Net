using System;
using System.Net;
using System.Net.Sockets;

namespace LumiSoft.Net.UDP
{
    /// <summary>
    /// This class provides data for the <see cref="UdpDataReceiver.PacketReceived"/> event.
    /// </summary>
    public class UdpEPacketReceived : EventArgs
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        internal UdpEPacketReceived()
        {
        }

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
        public IPEndPoint RemoteEp { get; private set; }

        /// <summary>
        /// Gets socket which received data.
        /// </summary>
        public Socket Socket { get; private set; }

        /// <summary>
        /// Reuses this class.
        /// </summary>
        /// <param name="socket">Socket which received data.</param>
        /// <param name="buffer">Data buffer.</param>
        /// <param name="count">Number of bytes stored in <b>buffer</b></param>
        /// <param name="remoteEp">Remote IP end point from where data was received.</param>
        internal void Reuse(Socket socket, byte[] buffer, int count, IPEndPoint remoteEp)
        {
            Socket = socket;
            Buffer = buffer;
            Count = count;
            RemoteEp = remoteEp;
        }
    }
}
