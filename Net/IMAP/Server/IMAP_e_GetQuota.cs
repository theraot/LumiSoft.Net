using System;
using System.Collections.Generic;

namespace LumiSoft.Net.IMAP.Server
{
    /// <summary>
    /// This class provides data for <b cref="IMAP_Session.GetQuota">IMAP_Session.GetQuota</b> event.
    /// </summary>
    public class IMAP_e_GetQuota : EventArgs
    {
        private IMAP_r_ServerStatus m_pResponse;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="quotaRoot">Quota root name.</param>
        /// <param name="response">Default IMAP server response.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>quotaRoot</b> is null reference.</exception>
        internal IMAP_e_GetQuota(string quotaRoot, IMAP_r_ServerStatus response)
        {
            QuotaRoot = quotaRoot ?? throw new ArgumentNullException("quotaRoot");
            m_pResponse = response;

            QuotaResponses = new List<IMAP_r_u_Quota>();
        }

        /// <summary>
        /// Gets QUOTA responses collection.
        /// </summary>
        public List<IMAP_r_u_Quota> QuotaResponses { get; }

        /// <summary>
        /// Gets quopta root name.
        /// </summary>
        public string QuotaRoot { get; }

        /// <summary>
        /// Gets or sets IMAP server response to this operation.
        /// </summary>
        /// <exception cref="ArgumentNullException">Is raised when null reference value set.</exception>
        public IMAP_r_ServerStatus Response
        {
            get { return m_pResponse; }

            set
            {
                m_pResponse = value ?? throw new ArgumentNullException("value");
            }
        }
    }
}
