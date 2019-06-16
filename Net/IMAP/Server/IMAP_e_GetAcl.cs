﻿using System;
using System.Collections.Generic;

namespace LumiSoft.Net.IMAP.Server
{
    /// <summary>
    /// This class provides data for <b cref="IMAP_Session.GetAcl">IMAP_Session.GetAcl</b> event.
    /// </summary>
    public class IMAP_e_GetAcl : EventArgs
    {
        private IMAP_r_ServerStatus m_pResponse;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="folder">Folder name with optional path.</param>
        /// <param name="response">Default IMAP server response.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>folder</b> or <b>response</b> is null reference.</exception>
        internal IMAP_e_GetAcl(string folder,IMAP_r_ServerStatus response)
        {
            Folder    = folder ?? throw new ArgumentNullException("folder");
            m_pResponse = response ?? throw new ArgumentNullException("response");

            AclResponses = new List<IMAP_r_u_Acl>();
        }

        /// <summary>
        /// Gets ACL responses collection.
        /// </summary>
        public List<IMAP_r_u_Acl> AclResponses { get; }

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
