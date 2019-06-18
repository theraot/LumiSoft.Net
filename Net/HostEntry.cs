﻿using System;
using System.Net;

namespace LumiSoft.Net
{
    /// <summary>
    /// This class represent DNS host entry.
    /// </summary>
    public class HostEntry
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="hostName">DNS host name.</param>
        /// <param name="ipAddresses">Host IP addresses.</param>
        /// <param name="aliases">Host aliases(CNAME).</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>hostName</b> or <b>ipAddresses</b> is null reference.</exception>
        public HostEntry(string hostName, IPAddress[] ipAddresses, string[] aliases)
        {
            if (hostName == null)
            {
                throw new ArgumentNullException(nameof(hostName));
            }
            if (hostName.Length == 0)
            {
                throw new ArgumentException("Argument 'hostName' value must be specified.", nameof(hostName));
            }

            HostName = hostName;
            Addresses = ipAddresses ?? throw new ArgumentNullException(nameof(ipAddresses));
            Aliases = aliases ?? new string[0];
        }

        /// <summary>
        /// Gets list of IP addresses that are associated with a host.
        /// </summary>
        public IPAddress[] Addresses { get; }

        /// <summary>
        /// Gets list of aliases(CNAME) that are associated with a host.
        /// </summary>
        public string[] Aliases { get; }

        /// <summary>
        /// Gets DNS host name.
        /// </summary>
        public string HostName { get; }
    }
}
