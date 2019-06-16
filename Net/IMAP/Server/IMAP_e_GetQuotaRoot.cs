﻿using System;
using System.Collections.Generic;

namespace LumiSoft.Net.IMAP.Server
{
    /// <summary>
    /// This class provides data for <b cref="IMAP_Session.GetQuotaRoot">IMAP_Session.GetQuotaRoot</b> event.
    /// </summary>
    public class IMAP_e_GetQuotaRoot : EventArgs
    {
        private IMAP_r_ServerStatus      m_pResponse;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="folder">Folder name with optional path.</param>
        /// <param name="response">Default IMAP server response.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>folder</b> or <b>response</b> is null reference.</exception>
        internal IMAP_e_GetQuotaRoot(string folder,IMAP_r_ServerStatus response)
        {
            Folder    = folder ?? throw new ArgumentNullException("folder");
            m_pResponse = response ?? throw new ArgumentNullException("response");

            QuotaRootResponses = new List<IMAP_r_u_QuotaRoot>();
            QuotaResponses = new List<IMAP_r_u_Quota>();
        }

        /// <summary>
        /// Gets QUOTAROOT responses collection.
        /// </summary>
        public List<IMAP_r_u_QuotaRoot> QuotaRootResponses { get; }

        /// <summary>
        /// Gets QUOTA responses collection.
        /// </summary>
        public List<IMAP_r_u_Quota> QuotaResponses { get; }

        /// <summary>
        /// Gets or sets IMAP server response to this operation.
        /// </summary>
        /// <exception cref="ArgumentNullException">Is raised when null reference value set.</exception>
        public IMAP_r_ServerStatus Response
        {
            get{ return m_pResponse; }

            set{
                m_pResponse = value ?? throw new ArgumentNullException("value"); 
            }
        }

        /// <summary>
        /// Gets folder name with optional path.
        /// </summary>
        public string Folder { get; }
    }
}
