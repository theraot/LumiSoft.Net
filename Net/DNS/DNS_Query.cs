using System;

namespace LumiSoft.Net.DNS
{
    /// <summary>
    /// This class represent DSN server query. Defined in RFC 1035.
    /// </summary>
    public class DNS_Query
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="qtype">Query type.</param>
        /// <param name="qname">Query text. It depends on query type.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>qname</b> is null reference.</exception>
        public DNS_Query(DNS_QType qtype,string qname) : this(DNS_QClass.IN,qtype,qname)
        {
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="qclass">Query class.</param>
        /// <param name="qtype">Query type.</param>
        /// <param name="qname">Query text. It depends on query type.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>qname</b> is null reference.</exception>
        public DNS_Query(DNS_QClass qclass,DNS_QType qtype,string qname)
        {
            if(qname == null){
                throw new ArgumentNullException("qname");
            }

            QueryClass = qclass;
            QueryType  = qtype;
            QueryName  = qname;
        }

        /// <summary>
        /// Gets DNS query class.
        /// </summary>
        public DNS_QClass QueryClass { get; } = DNS_QClass.IN;

        /// <summary>
        /// Gets DNS query type.
        /// </summary>
        public DNS_QType QueryType { get; } = DNS_QType.ANY;

        /// <summary>
        /// Gets query text.
        /// </summary>
        public string QueryName { get; } = "";
    }
}
