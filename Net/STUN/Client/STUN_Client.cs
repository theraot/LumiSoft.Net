using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using LumiSoft.Net.STUN.Message;

namespace LumiSoft.Net.STUN.Client
{
    /// <summary>
    /// This class implements STUN client. Defined in RFC 3489.
    /// </summary>
    /// <example>
    /// <code>
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
        /// Resolves socket local end point to public end point.
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
                throw new ArgumentException("Socket must be UDP socket!");
            }
            var remoteEndPoint = new IPEndPoint(Dns.GetHostAddresses(stunServer)[0], port);
            try
            {
                var stunMessage = DoTransaction
                (
                    new STUN_Message
                    {
                        Type = STUN_MessageType.BindingRequest
                    },
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

        private static STUN_Message DoTransaction(STUN_Message stunMessage, Socket socket, IPEndPoint pEndPoint, int i)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Resolves local IP to public IP using STUN.
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
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets NAT info from STUN server.
        /// </summary>
        /// <param name="host">STUN server name or IP.</param>
        /// <param name="port">STUN server port. Default port is 3478.</param>
        /// <param name="localEP">Local IP end point.</param>
        /// <returns>Returns UDP network info.</returns>
        /// <exception cref="T:System.ArgumentNullException">Is raised when <b>host</b> or <b>localEP</b> is null reference.</exception>
        /// <exception cref="T:System.Exception">Throws exception if unexpected error happens.</exception>
        public static STUN_Result Query(string host, int port, IPEndPoint localEP)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets NAT info from STUN server.
        /// </summary>
        /// <param name="host">STUN server name or IP.</param>
        /// <param name="port">STUN server port. Default port is 3478.</param>
        /// <param name="socket">UDP socket to use.</param>
        /// <returns>Returns UDP network info.</returns>
        /// <exception cref="T:System.Exception">Throws exception if unexpected error happens.</exception>
        public static STUN_Result Query(string host, int port, Socket socket)
        {
            throw new NotImplementedException();
        }
    }
}
