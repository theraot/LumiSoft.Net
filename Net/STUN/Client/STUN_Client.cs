// ReSharper disable ConvertIfStatementToReturnStatement

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using LumiSoft.Net.STUN.Message;

namespace LumiSoft.Net.STUN.Client
{
    /// <summary>
    ///     This class implements STUN client. Defined in RFC 3489.
    /// </summary>
    /// <example>
    ///     <code>
    /// // Create new socket for STUN client.
    /// Socket socket = new Socket(AddressFamily.InterNetwork,SocketType.Dgram,ProtocolType.Udp);
    /// socket.Bind(new IPEndPoint(IPAddress.Any,0));
    /// // Query STUN server
    /// STUN_Result result = STUN_Client.Query("stunserver.org",3478,socket);
    /// if(result.NetType != STUN_NetType.UdpBlocked){
    ///     // UDP blocked or !!!! bad STUN server
    /// }
    /// else{
    ///     IPEndPoint publicEP = result.PublicEndPoint;
    ///     // Do your stuff
    /// }
    /// </code>
    /// </example>
    public static class STUN_Client
    {
        /// <summary>
        ///     Resolves socket local end point to public end point.
        /// </summary>
        /// <param name="stunServer">STUN server.</param>
        /// <param name="port">STUN server port. Default port is 3478.</param>
        /// <param name="socket">UDP socket to use.</param>
        /// <returns>Returns public IP end point.</returns>
        /// <exception cref="T:System.ArgumentNullException">Is raised when <b>stunServer</b> or <b>socket</b> is null reference.</exception>
        /// <exception cref="T:System.ArgumentException">Is raised when any of the arguments has invalid value.</exception>
        /// <exception cref="T:System.IO.IOException">Is raised when no connection to STUN server.</exception>
        public static IPEndPoint GetPublicEndPoint(string stunServer, int port, Socket socket)
        {
            if (stunServer == null)
            {
                throw new ArgumentNullException(nameof(stunServer));
            }

            if (stunServer.Length == 0)
            {
                throw new ArgumentException("Argument 'stunServer' value must be specified.", nameof(stunServer));
            }

            if (port < 1)
            {
                throw new ArgumentException("Invalid argument 'port' value.", nameof(port));
            }

            if (socket == null)
            {
                throw new ArgumentNullException(nameof(socket));
            }

            if (socket.ProtocolType != ProtocolType.Udp)
            {
                throw new ArgumentException("Socket must be UDP socket.");
            }

            var remoteEndPoint = new IPEndPoint(Dns.GetHostAddresses(stunServer)[0], port);
            try
            {
                var stunMessage = DoTransaction
                (
                    new STUN_Message(STUN_MessageType.BindingRequest),
                    socket,
                    remoteEndPoint,
                    1000
                );
                if (stunMessage == null)
                {
                    throw new IOException("Failed to STUN public IP address. STUN server name is invalid or firewall blocks STUN.");
                }

                return stunMessage.SourceAddress;
            }
            catch
            {
                throw new IOException("Failed to STUN public IP address. STUN server name is invalid or firewall blocks STUN.");
            }
            finally
            {
                var now = DateTime.Now;
                while (now.AddMilliseconds(200) > DateTime.Now)
                {
                    if (!socket.Poll(1, SelectMode.SelectRead))
                    {
                        continue;
                    }

                    socket.Receive(new byte[512]);
                }
            }
        }

        /// <summary>
        ///     Resolves local IP to public IP using STUN.
        /// </summary>
        /// <param name="stunServer">STUN server.</param>
        /// <param name="port">STUN server port. Default port is 3478.</param>
        /// <param name="localIP">Local IP address.</param>
        /// <returns>Returns public IP address.</returns>
        /// <exception cref="T:System.ArgumentNullException">Is raised when <b>stunServer</b> or <b>localIP</b> is null reference.</exception>
        /// <exception cref="T:System.ArgumentException">Is raised when any of the arguments has invalid value.</exception>
        /// <exception cref="T:System.IO.IOException">Is raised when no connection to STUN server.</exception>
        public static IPAddress GetPublicIP(string stunServer, int port, IPAddress localIP)
        {
            if (stunServer == null)
            {
                throw new ArgumentNullException(nameof(stunServer));
            }

            if (stunServer.Length == 0)
            {
                throw new ArgumentException("Argument 'stunServer' value must be specified.", nameof(stunServer));
            }

            if (port < 1)
            {
                throw new ArgumentException("Invalid argument 'port' value.", nameof(port));
            }

            if (localIP == null)
            {
                throw new ArgumentNullException(nameof(localIP));
            }

            if (!IsPrivateIPv4(localIP))
            {
                return localIP;
            }

            using (var ret = CreateSocket(localIP))
            {
                var stunResult = Query(stunServer, port, ret);
                if (stunResult.PublicEndPoint == null)
                {
                    throw new IOException("Failed to STUN public IP address. STUN server name is invalid or firewall blocks STUN.");
                }

                return stunResult.PublicEndPoint.Address;
            }

            bool IsPrivateIPv4(IPAddress ipAddress)
            {
                if (ipAddress.AddressFamily != AddressFamily.InterNetwork)
                {
                    return false;
                }

                var bytes = ipAddress.GetAddressBytes();

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
                    case 172 when bytes[1] >= 16 && bytes[1] <= 31:
                    case 192 when bytes[1] == 168:
                    case 169 when bytes[1] == 254:
                        return true;
                    default:
                        return false;
                }
            }

            Socket CreateSocket(IPAddress localIP1)
            {
                var localEndPoint = new IPEndPoint(localIP, 0);
                // ReSharper disable once SwitchStatementMissingSomeCases
                switch (localIP1.AddressFamily)
                {
                    case AddressFamily.InterNetwork:
                    {
                        var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                        socket.Bind(localEndPoint);
                        return socket;
                    }

                    case AddressFamily.InterNetworkV6:
                    {
                        var socket = new Socket(AddressFamily.InterNetworkV6, SocketType.Dgram, ProtocolType.Udp);
                        socket.Bind(localEndPoint);
                        return socket;
                    }

                    default:
                        throw new ArgumentException("Invalid IPEndPoint address family.");
                }
            }
        }

        /// <summary>
        ///     Gets NAT info from STUN server.
        /// </summary>
        /// <param name="host">STUN server name or IP.</param>
        /// <param name="port">STUN server port. Default port is 3478.</param>
        /// <param name="localEndPoint">Local IP end point.</param>
        /// <returns>Returns UDP network info.</returns>
        /// <exception cref="T:System.ArgumentNullException">Is raised when <b>host</b> or <b>localEndPoint</b> is null reference.</exception>
        /// <exception cref="T:System.Exception">Throws exception if unexpected error happens.</exception>
        public static STUN_Result Query(string host, int port, IPEndPoint localEndPoint)
        {
            if (host == null)
            {
                throw new ArgumentNullException(nameof(host));
            }

            if (localEndPoint == null)
            {
                throw new ArgumentNullException(nameof(localEndPoint));
            }

            using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
            {
                socket.Bind(localEndPoint);
                return Query(host, port, socket);
            }
        }

        /// <summary>
        ///     Gets NAT info from STUN server.
        /// </summary>
        /// <param name="host">STUN server name or IP.</param>
        /// <param name="port">STUN server port. Default port is 3478.</param>
        /// <param name="socket">UDP socket to use.</param>
        /// <returns>Returns UDP network info.</returns>
        /// <exception cref="T:System.Exception">Throws exception if unexpected error happens.</exception>
        private static STUN_Result Query(string host, int port, Socket socket)
        {
            if (host == null)
            {
                throw new ArgumentNullException(nameof(host));
            }

            if (socket == null)
            {
                throw new ArgumentNullException(nameof(socket));
            }

            if (port < 1)
            {
                throw new ArgumentException("Port value must be >= 1.", nameof(port));
            }

            if (socket.ProtocolType != ProtocolType.Udp)
            {
                throw new ArgumentException("Socket must be UDP socket.");
            }

            var endPoint = new IPEndPoint(Dns.GetHostAddresses(host)[0], port);
            var comparer = EqualityComparer<IPEndPoint>.Default;

            // Refer to https://upload.wikimedia.org/wikipedia/commons/e/ea/STUN_Algorithm4.svg

            try
            {
                // Test I: Request echo from same address, same port
                var test1 = DoTransaction
                (
                    new STUN_Message(STUN_MessageType.BindingRequest),
                    socket,
                    endPoint,
                    1600
                );
                // received?
                if (test1 == null)
                {
                    // UDP blocked
                    return new STUN_Result(STUN_NetType.UdpBlocked, null);
                }

                // Public IP is link's IP?
                if (socket.LocalEndPoint.Equals(test1.MappedAddress))
                {
                    // Test II: request echo from different address, different port
                    var test2A = DoTransaction
                    (
                        new STUN_Message(STUN_MessageType.BindingRequest)
                        {
                            ChangeRequest = new STUN_t_ChangeRequest(true, true)
                        },
                        socket,
                        endPoint,
                        1600
                    );
                    // received?
                    if (test2A == null)
                    {
                        // "Symmetric" Firewall
                        return new STUN_Result(STUN_NetType.SymmetricUdpFirewall, test1.MappedAddress);
                    }

                    // Open Internet
                    return new STUN_Result(STUN_NetType.OpenInternet, test1.MappedAddress);
                }

                // Test II: request echo from different address, different port
                var test2B = DoTransaction
                (
                    new STUN_Message(STUN_MessageType.BindingRequest)
                    {
                        ChangeRequest = new STUN_t_ChangeRequest(true, true)
                    },
                    socket,
                    endPoint,
                    1600
                );
                // received?
                if (test2B != null)
                {
                    // "Full-cone" NAT
                    return new STUN_Result(STUN_NetType.FullCone, test1.MappedAddress);
                }

                // Test I (Server #2): Request echo from same address, same port
                var test3 = DoTransaction
                (
                    new STUN_Message(STUN_MessageType.BindingRequest),
                    socket,
                    test1.ChangedAddress,
                    1600
                );
                if (test3 == null)
                {
                    throw new Exception("STUN not available.");
                }

                // Public IP and port are constant?
                if (!comparer.Equals(test3.MappedAddress, test1.MappedAddress))
                {
                    // "Symmetric" NAT
                    return new STUN_Result(STUN_NetType.Symmetric, test1.MappedAddress);
                }

                // Test III:Request echo from same address, different port
                var test4 = DoTransaction
                (
                    new STUN_Message(STUN_MessageType.BindingRequest)
                    {
                        ChangeRequest = new STUN_t_ChangeRequest(false, true)
                    },
                    socket,
                    test1.ChangedAddress,
                    1600
                );
                // received?
                if (test4 == null)
                {
                    // "Restricted port" NAT
                    return new STUN_Result(STUN_NetType.PortRestrictedCone, test1.MappedAddress);
                }
                
                // "Restricted cone" NAT
                return new STUN_Result(STUN_NetType.RestrictedCone, test1.MappedAddress);
            }
            finally
            {
                var start = DateTime.Now;
                while ((DateTime.Now - start).TotalMilliseconds < 200)
                {
                    if (!socket.Poll(1, SelectMode.SelectRead))
                    {
                        continue;
                    }

                    socket.Receive(new byte[512]);
                }
            }
        }

        private static STUN_Message DoTransaction(STUN_Message request, Socket socket, EndPoint remoteEndPoint, int millisecondsTimeout)
        {
            var byteData = request.ToByteData();
            var start = DateTime.Now;
            do
            {
                socket.SendTo(byteData, remoteEndPoint);
                if (!socket.Poll(500000, SelectMode.SelectRead))
                {
                    continue;
                }

                var buffer = new byte[512];
                socket.Receive(buffer);
                var stunMessage = new STUN_Message(buffer);
                var requestTransactionId = request.TransactionId;
                var stunMessageTransactionId = stunMessage.TransactionId;
                var arrayEquals = true;
                if (requestTransactionId.Length != stunMessageTransactionId.Length)
                {
                    arrayEquals = false;
                }
                else
                {
                    for (var index = 0; index < requestTransactionId.Length; index++)
                    {
                        if (requestTransactionId.GetValue(index).Equals(stunMessageTransactionId.GetValue(index)))
                        {
                            continue;
                        }

                        arrayEquals = false;
                        break;
                    }
                }

                if (arrayEquals)
                {
                    return stunMessage;
                }
            } while ((DateTime.Now - start).TotalMilliseconds > millisecondsTimeout);

            return null;
        }
    }
}