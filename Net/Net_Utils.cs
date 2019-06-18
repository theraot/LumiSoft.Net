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

        /// <summary>
        /// Gets if specified IP address is a private LAN IPv4 address. For example 192.168.x.x is private ip.
        /// </summary>
        /// <param name="ip">IP address to check.</param>
        /// <returns>Returns true if IP is private IP.</returns>
        /// <exception cref="ArgumentNullException">Is raised when <b>ip</b> is null reference.</exception>
        public static bool IsPrivateIPv4(IPAddress ip)
        {
            if (ip == null)
            {
                throw new ArgumentNullException(nameof(ip));
            }
            if (ip.AddressFamily != AddressFamily.InterNetwork)
            {
                return false;
            }

            var bytes = ip.GetAddressBytes();

            /* Private IPs (RFC1918):
                    First Octet = 10 (Example: 10.X.X.X)
                    First Octet = 172 AND (Second Octet >= 16 AND Second Octet <= 31) (Example: 172.16.X.X - 172.31.X.X)
                    First Octet = 192 AND Second Octet = 168 (Example: 192.168.X.X)
               Link-Local IPs (RFC3927):
                    First Octet = 169 AND Second Octet = 254 (Example: 169.254.X.X)
            */

            switch (bytes[0])
            {
                case 10:
                    return true;
                case 172 when bytes[1] >= 16 && bytes[1] <= 31:
                    return true;
                case 192 when bytes[1] == 168:
                    return true;
                case 169 when bytes[1] == 254:
                    return true;
                default:
                    return false;
            }
        }
    }
}
