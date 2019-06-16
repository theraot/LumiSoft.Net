﻿using System;

namespace LumiSoft.Net.IMAP.Server
{
    /// <summary>
    /// This class provides data for <b cref="IMAP_Session.Copy">IMAP_Session.Copy</b> event.
    /// </summary>
    public class IMAP_e_Copy : EventArgs
    {
        private IMAP_r_ServerStatus m_pResponse;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="sourceFolder">Source folder name with optional path.</param>
        /// <param name="targetFolder">Target folder name </param>
        /// <param name="messagesInfo">Messages info.</param>
        /// <param name="response">Default IMAP server response.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>sourceFolder</b>,<b>targetFolder</b>,<b>messagesInfo</b> or <b>response</b> is null reference.</exception>
        internal IMAP_e_Copy(string sourceFolder, string targetFolder, IMAP_MessageInfo[] messagesInfo, IMAP_r_ServerStatus response)
        {
            m_pResponse = response ?? throw new ArgumentNullException("response");
            SourceFolder = sourceFolder ?? throw new ArgumentNullException("sourceFolder");
            TargetFolder = targetFolder ?? throw new ArgumentNullException("targetFolder");
            MessagesInfo = messagesInfo ?? throw new ArgumentNullException("messagesInfo");
        }

        /// <summary>
        /// Gets messages info.
        /// </summary>
        public IMAP_MessageInfo[] MessagesInfo { get; }

        /// <summary>
        /// Gets or sets IMAP server response to this operation.
        /// </summary>
        /// <exception cref="ArgumentNullException">Is raised when null reference value set.</exception>
        public IMAP_r_ServerStatus Response
        {
            get => m_pResponse;

            set => m_pResponse = value ?? throw new ArgumentNullException("value");
        }

        /// <summary>
        /// Gets source folder name with optional path.
        /// </summary>
        public string SourceFolder { get; }

        /// <summary>
        /// Gets target folder name with optional path.
        /// </summary>
        public string TargetFolder { get; }
    }
}
