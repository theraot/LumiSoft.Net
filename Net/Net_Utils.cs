using System;
using System.Net;
using System.Net.Sockets;

namespace LumiSoft.Net
{
    public static class Net_Utils
    {
        /// <summary>
        /// Creates new socket for the specified end point.
        /// </summary>
        /// <param name="localEndPoint">Local end point.</param>
        /// <param name="protocolType">Protocol type.</param>
        /// <returns>Returns newly created socket.</returns>
        /// <exception cref="ArgumentNullException">Is raised when <b>localEndPoint</b> is null reference.</exception>
        public static Socket CreateSocket(IPEndPoint localEndPoint, ProtocolType protocolType)
        {
            if (localEndPoint == null)
            {
                throw new ArgumentNullException(nameof(localEndPoint));
            }

            var socketType = SocketType.Stream;
            if (protocolType == ProtocolType.Udp)
            {
                socketType = SocketType.Dgram;
            }

            switch (localEndPoint.AddressFamily)
            {
                case AddressFamily.InterNetwork:
                {
                    var socket = new Socket(AddressFamily.InterNetwork, socketType, protocolType);
                    socket.Bind(localEndPoint);
                    return socket;
                }

                case AddressFamily.InterNetworkV6:
                {
                    var socket = new Socket(AddressFamily.InterNetworkV6, socketType, protocolType);
                    socket.Bind(localEndPoint);
                    return socket;
                }

                default:
                    throw new ArgumentException("Invalid IPEndPoint address family.");
            }
        }
    }
}
