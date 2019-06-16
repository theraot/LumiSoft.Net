using System;

using LumiSoft.Net.DNS.Client;

namespace LumiSoft.Net.DNS
{
	/// <summary>
	/// PTR record class.
	/// </summary>
	[Serializable]
	public class DNS_rr_PTR : DNS_rr
	{
        /// <summary>
		/// Default constructor.
		/// </summary>
        /// <param name="name">DNS domain name that owns a resource record.</param>
		/// <param name="domainName">DomainName.</param>
		/// <param name="ttl">TTL value.</param>
		public DNS_rr_PTR(string name,string domainName,int ttl) : base(name,DNS_QType.PTR,ttl)
		{
			DomainName = domainName;
		}

        /// <summary>
        /// Parses resource record from reply data.
        /// </summary>
        /// <param name="name">DNS domain name that owns a resource record.</param>
        /// <param name="reply">DNS server reply data.</param>
        /// <param name="offset">Current offset in reply data.</param>
        /// <param name="rdLength">Resource record data length.</param>
        /// <param name="ttl">Time to live in seconds.</param>
        public static DNS_rr_PTR Parse(string name,byte[] reply,ref int offset,int rdLength,int ttl)
        {
            var domainName = "";
            if (Dns_Client.GetQName(reply,ref offset,ref domainName)){
			    return new DNS_rr_PTR(name,domainName,ttl);
            }

            throw new ArgumentException("Invalid PTR resource record data !");
        }

        /// <summary>
		/// Gets domain name.
		/// </summary>
		public string DomainName { get; } = "";
    }
}
