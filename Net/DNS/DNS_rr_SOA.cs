using System;

using LumiSoft.Net.DNS.Client;

namespace LumiSoft.Net.DNS
{
	/// <summary>
	/// SOA record class.
	/// </summary>
	[Serializable]
	public class DNS_rr_SOA : DNS_rr
	{
        /// <summary>
		/// Default constructor.
		/// </summary>
        /// <param name="name">DNS domain name that owns a resource record.</param>
		/// <param name="nameServer">Name server.</param>
		/// <param name="adminEmail">Zone administrator email.</param>
		/// <param name="serial">Version number of the original copy of the zone.</param>
		/// <param name="refresh">Time interval(in seconds) before the zone should be refreshed.</param>
		/// <param name="retry">Time interval(in seconds) that should elapse before a failed refresh should be retried.</param>
		/// <param name="expire">Time value(in seconds) that specifies the upper limit on the time interval that can elapse before the zone is no longer authoritative.</param>
		/// <param name="minimum">Minimum TTL(in seconds) field that should be exported with any RR from this zone.</param>
		/// <param name="ttl">TTL value.</param>
		public DNS_rr_SOA(string name,string nameServer,string adminEmail,long serial,long refresh,long retry,long expire,long minimum,int ttl) : base(name,DNS_QType.SOA,ttl)
		{
			NameServer = nameServer;
			AdminEmail = adminEmail;
			Serial     = serial;
			Refresh    = refresh;
			Retry      = retry;
			Expire     = expire;
			Minimum    = minimum;
		}

        /// <summary>
        /// Parses resource record from reply data.
        /// </summary>
        /// <param name="name">DNS domain name that owns a resource record.</param>
        /// <param name="reply">DNS server reply data.</param>
        /// <param name="offset">Current offset in reply data.</param>
        /// <param name="rdLength">Resource record data length.</param>
        /// <param name="ttl">Time to live in seconds.</param>
        public static DNS_rr_SOA Parse(string name,byte[] reply,ref int offset,int rdLength,int ttl)
        {
            /* RFC 1035 3.3.13. SOA RDATA format

				+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
				/                     MNAME                     /
				/                                               /
				+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
				/                     RNAME                     /
				+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
				|                    SERIAL                     |
				|                                               |
				+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
				|                    REFRESH                    |
				|                                               |
				+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
				|                     RETRY                     |
				|                                               |
				+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
				|                    EXPIRE                     |
				|                                               |
				+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
				|                    MINIMUM                    |
				|                                               |
				+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+

			where:

			MNAME           The <domain-name> of the name server that was the
							original or primary source of data for this zone.

			RNAME           A <domain-name> which specifies the mailbox of the
							person responsible for this zone.

			SERIAL          The unsigned 32 bit version number of the original copy
							of the zone.  Zone transfers preserve this value.  This
							value wraps and should be compared using sequence space
							arithmetic.

			REFRESH         A 32 bit time interval before the zone should be
							refreshed.

			RETRY           A 32 bit time interval that should elapse before a
							failed refresh should be retried.

			EXPIRE          A 32 bit time value that specifies the upper limit on
							the time interval that can elapse before the zone is no
							longer authoritative.
							
			MINIMUM         The unsigned 32 bit minimum TTL field that should be
							exported with any RR from this zone.
			*/

			//---- Parse record -------------------------------------------------------------//
			// MNAME
			var nameserver = "";
            Dns_Client.GetQName(reply,ref offset,ref nameserver);

			// RNAME
			var adminMailBox = "";
            Dns_Client.GetQName(reply,ref offset,ref adminMailBox);
			var adminMailBoxAr = adminMailBox.ToCharArray();
            for (int i=0;i<adminMailBoxAr.Length;i++){			
				if(adminMailBoxAr[i] == '.'){
					adminMailBoxAr[i] = '@';
					break;
				}
			}
			adminMailBox = new string(adminMailBoxAr);

			// SERIAL
			long serial = reply[offset++] << 24 | reply[offset++] << 16 | reply[offset++] << 8 | reply[offset++];

			// REFRESH
			long refresh = reply[offset++] << 24 | reply[offset++] << 16 | reply[offset++] << 8 | reply[offset++];

			// RETRY
			long retry = reply[offset++] << 24 | reply[offset++] << 16 | reply[offset++] << 8 | reply[offset++];

			// EXPIRE
			long expire = reply[offset++] << 24 | reply[offset++] << 16 | reply[offset++] << 8 | reply[offset++];

			// MINIMUM
			long minimum = reply[offset++] << 24 | reply[offset++] << 16 | reply[offset++] << 8 | reply[offset++];
			//--------------------------------------------------------------------------------//

			return new DNS_rr_SOA(name,nameserver,adminMailBox,serial,refresh,retry,expire,minimum,ttl);
        }

        /// <summary>
		/// Gets name server.
		/// </summary>
		public string NameServer { get; } = "";

        /// <summary>
		/// Gets zone administrator email.
		/// </summary>
		public string AdminEmail { get; } = "";

        /// <summary>
		/// Gets version number of the original copy of the zone.
		/// </summary>
		public long Serial { get; }

        /// <summary>
		/// Gets time interval(in seconds) before the zone should be refreshed.
		/// </summary>
		public long Refresh { get; }

        /// <summary>
		/// Gets time interval(in seconds) that should elapse before a failed refresh should be retried.
		/// </summary>
		public long Retry { get; }

        /// <summary>
		/// Gets time value(in seconds) that specifies the upper limit on the time interval that can elapse before the zone is no longer authoritative.
		/// </summary>
		public long Expire { get; }

        /// <summary>
		/// Gets minimum TTL(in seconds) field that should be exported with any RR from this zone. 
		/// </summary>
		public long Minimum { get; }
    }
}
