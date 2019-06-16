using System;
using System.Net;

namespace LumiSoft.Net.Mail
{
    /// <summary>
    /// Represents Received: header "TCP-info" value. Defined in RFC 5321. 4.4.
    /// </summary>
    /// <remarks>
    /// <code>
    /// RFC 5321 4.4.
    ///     TCP-info        = address-literal / ( Domain FWS address-literal )
    ///     address-literal = "[" ( IPv4-address-literal / IPv6-address-literal / General-address-literal ) "]"
    /// </code>
    /// </remarks>
    public class Mail_t_TcpInfo
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="ip">IP address.</param>
        /// <param name="hostName">Host name.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>ip</b> is null reference.</exception>
        public Mail_t_TcpInfo(IPAddress ip,string hostName)
        {
            if(ip == null){
                throw new ArgumentNullException("ip");
            }

            IP      = ip;
            HostName = hostName;
        }

        /// <summary>
        /// Returns this as string.
        /// </summary>
        /// <returns>Returns this as string.</returns>
        public override string ToString()
        {
            if(string.IsNullOrEmpty(HostName)){
                return "["  + IP.ToString() + "]";
            }
            else{
                return HostName + " [" + IP.ToString() + "]";
            }
        }

        /// <summary>
        /// Gets IP address.
        /// </summary>
        public IPAddress IP { get; }

        /// <summary>
        /// Gets host value. Value null means not specified.
        /// </summary>
        public string HostName { get; }
    }
}
