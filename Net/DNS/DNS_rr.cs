namespace LumiSoft.Net.DNS
{
    /// <summary>
    /// This is base class for DNS records.
    /// </summary>
    public abstract class DNS_rr
	{
        /// <summary>
		/// Default constructor.
		/// </summary>
        /// <param name="name">DNS domain name that owns a resource record.</param>
		/// <param name="recordType">Record type (A,MX, ...).</param>
		/// <param name="ttl">TTL (time to live) value in seconds.</param>
		public DNS_rr(string name,DNS_QType recordType,int ttl)
		{
            Name = name;
			RecordType = recordType;
			TTL  = ttl;
        }

        /// <summary>
        /// Gets DNS domain name that owns a resource record.
        /// </summary>
        public string Name { get; } = "";

        /// <summary>
		/// Gets record type (A,MX,...).
		/// </summary>
		public DNS_QType RecordType { get; } = DNS_QType.A;

        /// <summary>
		/// Gets TTL (time to live) value in seconds.
		/// </summary>
		public int TTL { get; } = -1;
    }
}
