using System;
using System.Net;

namespace LumiSoft.Net.RTP
{
    /// <summary>
    /// This class implements RTP session address.
    /// </summary>
    public class RTP_Address
    {
        /// <summary>
        /// Unicast constructor.
        /// </summary>
        /// <param name="ip">Unicast IP address.</param>
        /// <param name="dataPort">RTP data port.</param>
        /// <param name="controlPort">RTP control port. Usualy this is <b>dataPort</b> + 1.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>ip</b> is null reference.</exception>
        /// <exception cref="ArgumentException">Is raised when any of the arguments has invalid values.</exception>
        public RTP_Address(IPAddress ip, int dataPort, int controlPort)
        {
            if (dataPort < IPEndPoint.MinPort || dataPort > IPEndPoint.MaxPort)
            {
                throw new ArgumentException("Argument 'dataPort' value must be between '" + IPEndPoint.MinPort + "' and '" + IPEndPoint.MaxPort + "'.");
            }
            if (controlPort < IPEndPoint.MinPort || controlPort > IPEndPoint.MaxPort)
            {
                throw new ArgumentException("Argument 'controlPort' value must be between '" + IPEndPoint.MinPort + "' and '" + IPEndPoint.MaxPort + "'.");
            }
            if (dataPort == controlPort)
            {
                throw new ArgumentException("Arguments 'dataPort' and 'controlPort' values must be different.");
            }

            IP = ip ?? throw new ArgumentNullException("ip");
            DataPort = dataPort;
            ControlPort = controlPort;

            RtpEP = new IPEndPoint(ip, dataPort);
            RtcpEP = new IPEndPoint(ip, controlPort);
        }

        /// <summary>
        /// Multicast constructor.
        /// </summary>
        /// <param name="ip">Multicast IP address.</param>
        /// <param name="dataPort">RTP data port.</param>
        /// <param name="controlPort">RTP control port. Usualy this is <b>dataPort</b> + 1.</param>
        /// <param name="ttl">RTP control port. Usualy this is <b>dataPort</b> + 1.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>ip</b> is null reference.</exception>
        /// <exception cref="ArgumentException">Is raised when any of the arguments has invalid values.</exception>
        public RTP_Address(IPAddress ip, int dataPort, int controlPort, int ttl)
        {
            if (ip == null)
            {
                throw new ArgumentNullException("ip");
            }
            if (!Net_Utils.IsMulticastAddress(ip))
            {
                throw new ArgumentException("Argument 'ip' is not multicast ip address.");
            }
            if (dataPort < IPEndPoint.MinPort || dataPort > IPEndPoint.MaxPort)
            {
                throw new ArgumentException("Argument 'dataPort' value must be between '" + IPEndPoint.MinPort + "' and '" + IPEndPoint.MaxPort + "'.");
            }
            if (controlPort < IPEndPoint.MinPort || controlPort > IPEndPoint.MaxPort)
            {
                throw new ArgumentException("Argument 'controlPort' value must be between '" + IPEndPoint.MinPort + "' and '" + IPEndPoint.MaxPort + "'.");
            }
            if (dataPort == controlPort)
            {
                throw new ArgumentException("Arguments 'dataPort' and 'controlPort' values must be different.");
            }
            if (ttl < 0 || ttl > 255)
            {
                throw new ArgumentException("Argument 'ttl' value must be between '0' and '255'.");
            }

            IP = ip;
            DataPort = dataPort;
            ControlPort = controlPort;
            TTL = ttl;

            RtpEP = new IPEndPoint(ip, dataPort);
            RtcpEP = new IPEndPoint(ip, controlPort);
        }

        /// <summary>
        /// Gets RTCP control port.
        /// </summary>
        public int ControlPort { get; }

        /// <summary>
        /// Gets RTP data port.
        /// </summary>
        public int DataPort { get; }

        /// <summary>
        /// Gets IP address.
        /// </summary>
        public IPAddress IP { get; }

        /// <summary>
        /// Gets if this is multicast RTP address.
        /// </summary>
        public bool IsMulticast
        {
            get { return Net_Utils.IsMulticastAddress(IP); }
        }

        /// <summary>
        /// Gets RTPCP end point.
        /// </summary>
        public IPEndPoint RtcpEP { get; }

        /// <summary>
        /// Gets RTP end point.
        /// </summary>
        public IPEndPoint RtpEP { get; }

        /// <summary>
        /// Gets mulicast TTL(time to live) value.
        /// </summary>
        public int TTL { get; }

        /// <summary>
        /// Determines whether the specified Object is equal to the current Object.
        /// </summary>
        /// <param name="obj">The Object to compare with the current Object.</param>
        /// <returns>True if the specified Object is equal to the current Object; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (obj is RTP_Address)
            {
                var a = (RTP_Address)obj;

                if (a.IP.Equals(IP) && a.ControlPort == ControlPort && a.DataPort == DataPort)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Gets this hash code.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
