using System;
using System.Collections.Generic;

namespace LumiSoft.Net.DNS.Client
{
    /// <summary>
    /// This class represents dns server response.
    /// </summary>
    [Serializable]
    public class DnsServerResponse
    {
        private readonly List<DNS_rr> m_pAdditionalAnswers;
        private readonly List<DNS_rr> m_pAnswers;
        private readonly List<DNS_rr> m_pAuthoritiveAnswers;

        internal DnsServerResponse(bool connectionOk, int id, DNS_RCode rcode, List<DNS_rr> answers, List<DNS_rr> authoritiveAnswers, List<DNS_rr> additionalAnswers)
        {
            ConnectionOk = connectionOk;
            ID = id;
            ResponseCode = rcode;
            m_pAnswers = answers;
            m_pAuthoritiveAnswers = authoritiveAnswers;
            m_pAdditionalAnswers = additionalAnswers;
        }

        /// <summary>
        /// Gets resource records in the additional records section. NOTE: Before using this property ensure that ConnectionOk=true and ResponseCode=RCODE.NO_ERROR.
        /// </summary>
        public DNS_rr[] AdditionalAnswers
        {
            get { return m_pAdditionalAnswers.ToArray(); }
        }

        /// <summary>
		/// Gets all resource records returned by server (answer records section + authority records section + additional records section). 
		/// NOTE: Before using this property ensure that ConnectionOk=true and ResponseCode=RCODE.NO_ERROR.
		/// </summary>
		public DNS_rr[] AllAnswers
        {
            get
            {
                var retVal = new List<DNS_rr>();
                retVal.AddRange(m_pAnswers.ToArray());
                retVal.AddRange(m_pAuthoritiveAnswers.ToArray());
                retVal.AddRange(m_pAdditionalAnswers.ToArray());

                return retVal.ToArray();
            }
        }

        /// <summary>
        /// Gets dns server returned answers. NOTE: Before using this property ensure that ConnectionOk=true and ResponseCode=RCODE.NO_ERROR.
        /// </summary>
        /// <code>
        /// // NOTE: DNS server may return diffrent record types even if you query MX.
        /// //       For example you query lumisoft.ee MX and server may response:	
        ///	//		 1) MX - mail.lumisoft.ee
        ///	//		 2) A  - lumisoft.ee
        ///	// 
        ///	//       Before casting to right record type, see what type record is !
        ///				
        /// 
        /// foreach(DnsRecordBase record in Answers){
        ///		// MX record, cast it to MX_Record
        ///		if(record.RecordType == QTYPE.MX){
        ///			MX_Record mx = (MX_Record)record;
        ///		}
        /// }
        /// </code>
        public DNS_rr[] Answers
        {
            get { return m_pAnswers.ToArray(); }
        }

        /// <summary>
        /// Gets name server resource records in the authority records section. NOTE: Before using this property ensure that ConnectionOk=true and ResponseCode=RCODE.NO_ERROR.
        /// </summary>
        public DNS_rr[] AuthoritiveAnswers
        {
            get { return m_pAuthoritiveAnswers.ToArray(); }
        }

        /// <summary>
		/// Gets if connection to dns server was successful.
		/// </summary>
		public bool ConnectionOk { get; } = true;

        /// <summary>
        /// Gets DNS transaction ID.
        /// </summary>
        public int ID { get; }

        /// <summary>
		/// Gets dns server response code.
		/// </summary>
		public DNS_RCode ResponseCode { get; } = DNS_RCode.NO_ERROR;

        /// <summary>
		/// Gets IPv6 host addess records.
		/// </summary>
		/// <returns></returns>
		public DNS_rr_AAAA[] GetAAAARecords()
        {
            var retVal = new List<DNS_rr_AAAA>();
            foreach (DNS_rr record in m_pAnswers)
            {
                if (record.RecordType == DNS_QType.AAAA)
                {
                    retVal.Add((DNS_rr_AAAA)record);
                }
            }

            return retVal.ToArray();
        }

        /// <summary>
		/// Gets IPv4 host addess records.
		/// </summary>
		/// <returns></returns>
		public DNS_rr_A[] GetARecords()
        {
            var retVal = new List<DNS_rr_A>();
            foreach (DNS_rr record in m_pAnswers)
            {
                if (record.RecordType == DNS_QType.A)
                {
                    retVal.Add((DNS_rr_A)record);
                }
            }

            return retVal.ToArray();
        }

        /// <summary>
		/// Gets CNAME records.
		/// </summary>
		/// <returns></returns>
		public DNS_rr_CNAME[] GetCNAMERecords()
        {
            var retVal = new List<DNS_rr_CNAME>();
            foreach (DNS_rr record in m_pAnswers)
            {
                if (record.RecordType == DNS_QType.CNAME)
                {
                    retVal.Add((DNS_rr_CNAME)record);
                }
            }

            return retVal.ToArray();
        }

        /// <summary>
		/// Gets HINFO records.
		/// </summary>
		/// <returns></returns>
		public DNS_rr_HINFO[] GetHINFORecords()
        {
            var retVal = new List<DNS_rr_HINFO>();
            foreach (DNS_rr record in m_pAnswers)
            {
                if (record.RecordType == DNS_QType.HINFO)
                {
                    retVal.Add((DNS_rr_HINFO)record);
                }
            }

            return retVal.ToArray();
        }

        /// <summary>
		/// Gets MX records.(MX records are sorted by preference, lower array element is prefered)
		/// </summary>
		/// <returns></returns>
		public DNS_rr_MX[] GetMXRecords()
        {
            var mx = new List<DNS_rr_MX>();
            foreach (DNS_rr record in m_pAnswers)
            {
                if (record.RecordType == DNS_QType.MX)
                {
                    mx.Add((DNS_rr_MX)record);
                }
            }

            // Sort MX records by preference.
            var retVal = mx.ToArray();
            Array.Sort(retVal);

            return retVal;
        }

        /// <summary>
		/// Gets NAPTR resource records.
		/// </summary>
		/// <returns></returns>
		public DNS_rr_NAPTR[] GetNAPTRRecords()
        {
            var retVal = new List<DNS_rr_NAPTR>();
            foreach (DNS_rr record in m_pAnswers)
            {
                if (record.RecordType == DNS_QType.NAPTR)
                {
                    retVal.Add((DNS_rr_NAPTR)record);
                }
            }

            return retVal.ToArray();
        }

        /// <summary>
		/// Gets name server records.
		/// </summary>
		/// <returns></returns>
		public DNS_rr_NS[] GetNSRecords()
        {
            var retVal = new List<DNS_rr_NS>();
            foreach (DNS_rr record in m_pAnswers)
            {
                if (record.RecordType == DNS_QType.NS)
                {
                    retVal.Add((DNS_rr_NS)record);
                }
            }

            return retVal.ToArray();
        }

        /// <summary>
		/// Gets PTR records.
		/// </summary>
		/// <returns></returns>
		public DNS_rr_PTR[] GetPTRRecords()
        {
            var retVal = new List<DNS_rr_PTR>();
            foreach (DNS_rr record in m_pAnswers)
            {
                if (record.RecordType == DNS_QType.PTR)
                {
                    retVal.Add((DNS_rr_PTR)record);
                }
            }

            return retVal.ToArray();
        }

        /// <summary>
		/// Gets SOA records.
		/// </summary>
		/// <returns></returns>
		public DNS_rr_SOA[] GetSOARecords()
        {
            var retVal = new List<DNS_rr_SOA>();
            foreach (DNS_rr record in m_pAnswers)
            {
                if (record.RecordType == DNS_QType.SOA)
                {
                    retVal.Add((DNS_rr_SOA)record);
                }
            }

            return retVal.ToArray();
        }

        /// <summary>
		/// Gets SPF resource records.
		/// </summary>
		/// <returns></returns>
		public DNS_rr_SPF[] GetSPFRecords()
        {
            var retVal = new List<DNS_rr_SPF>();
            foreach (DNS_rr record in m_pAnswers)
            {
                if (record.RecordType == DNS_QType.SPF)
                {
                    retVal.Add((DNS_rr_SPF)record);
                }
            }

            return retVal.ToArray();
        }

        /// <summary>
		/// Gets SRV resource records.
		/// </summary>
		/// <returns></returns>
		public DNS_rr_SRV[] GetSRVRecords()
        {
            var retVal = new List<DNS_rr_SRV>();
            foreach (DNS_rr record in m_pAnswers)
            {
                if (record.RecordType == DNS_QType.SRV)
                {
                    retVal.Add((DNS_rr_SRV)record);
                }
            }

            return retVal.ToArray();
        }

        /// <summary>
		/// Gets text records.
		/// </summary>
		/// <returns></returns>
		public DNS_rr_TXT[] GetTXTRecords()
        {
            var retVal = new List<DNS_rr_TXT>();
            foreach (DNS_rr record in m_pAnswers)
            {
                if (record.RecordType == DNS_QType.TXT)
                {
                    retVal.Add((DNS_rr_TXT)record);
                }
            }

            return retVal.ToArray();
        }

        /// <summary>
		/// Filters out specified type of records from answer.
		/// </summary>
		/// <param name="answers"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		private List<DNS_rr> FilterRecordsX(List<DNS_rr> answers, DNS_QType type)
        {
            var retVal = new List<DNS_rr>();
            foreach (DNS_rr record in answers)
            {
                if (record.RecordType == type)
                {
                    retVal.Add(record);
                }
            }

            return retVal;
        }
    }
}
