using System;

namespace LumiSoft.Net.DNS.Client
{
    /// <summary>
    /// DNS client exception.
    /// </summary>
    public class DNS_ClientException : Exception
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="rcode">DNS server returned error code.</param>
        public DNS_ClientException(DNS_RCode rcode) : base("Dns error: " + rcode + ".")
        {
            ErrorCode = rcode;
        }

        /// <summary>
        /// Gets DNS server returned error code.
        /// </summary>
        public DNS_RCode ErrorCode { get; }
    }
}
