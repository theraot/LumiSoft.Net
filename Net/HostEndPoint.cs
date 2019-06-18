using System;
using System.Net;

namespace LumiSoft.Net
{
    /// <summary>
    /// Represents a network endpoint as an host(name or IP address) and a port number.
    /// </summary>
    public class HostEndPoint
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="host">Host name or IP address.</param>
        /// <param name="port">The port number associated with the host. Value -1 means port not specified.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>host</b> is null.</exception>
        /// <exception cref="ArgumentException">Is raised when any of the arguments has invalid value.</exception>
        public HostEndPoint(string host, int port)
        {
            if (host == null)
            {
                throw new ArgumentNullException(nameof(host));
            }
            if (host == "")
            {
                throw new ArgumentException("Argument 'host' value must be specified.");
            }

            Host = host;
            Port = port;
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="endPoint">Host IP end point.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>endPoint</b> is null reference.</exception>
        public HostEndPoint(IPEndPoint endPoint)
        {
            if (endPoint == null)
            {
                throw new ArgumentNullException(nameof(endPoint));
            }

            Host = endPoint.Address.ToString();
            Port = endPoint.Port;
        }

        /// <summary>
        /// Gets host name or IP address.
        /// </summary>
        public string Host { get; }

        /// <summary>
        /// Gets if <b>Host</b> is IP address.
        /// </summary>
        public bool IsIPAddress => NetUtils.IsIPAddress(Host);

        /// <summary>
        /// Gets the port number of the endpoint. Value -1 means port not specified.
        /// </summary>
        public int Port { get; }

        /// <summary>
        /// Parses HostEndPoint from the specified string.
        /// </summary>
        /// <param name="value">HostEndPoint value.</param>
        /// <returns>Returns parsed HostEndPoint value.</returns>
        /// <exception cref="ArgumentNullException">Is raised when <b>value</b> is null.</exception>
        /// <exception cref="ArgumentException">Is raised when any of the arguments has invalid value.</exception>
        public static HostEndPoint Parse(string value)
        {
            return Parse(value, -1);
        }

        /// <summary>
        /// Parses HostEndPoint from the specified string.
        /// </summary>
        /// <param name="value">HostEndPoint value.</param>
        /// <param name="defaultPort">If port isn't specified in value, specified port will be used.</param>
        /// <returns>Returns parsed HostEndPoint value.</returns>
        /// <exception cref="ArgumentNullException">Is raised when <b>value</b> is null.</exception>
        /// <exception cref="ArgumentException">Is raised when any of the arguments has invalid value.</exception>
        public static HostEndPoint Parse(string value, int defaultPort)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            if (value == "")
            {
                throw new ArgumentException("Argument 'value' value must be specified.");
            }

            // We have host name with port.
            if (value.IndexOf(':') <= -1)
            {
                return new HostEndPoint(value, defaultPort);
            }

            var hostPort = value.Split(new[] { ':' }, 2);

            try
            {
                return new HostEndPoint(hostPort[0], Convert.ToInt32(hostPort[1]));
            }
            catch
            {
                throw new ArgumentException("Argument 'value' has invalid value.");
            }
        }

        /// <summary>
        /// Returns HostEndPoint as string.
        /// </summary>
        /// <returns>Returns HostEndPoint as string.</returns>
        public override string ToString()
        {
            if (Port == -1)
            {
                return Host;
            }

            return Host + ":" + Port;
        }
    }
}
