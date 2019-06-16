using System;

namespace LumiSoft.Net.IMAP.Server
{
    /// <summary>
    /// This class provides data for <b cref="IMAP_Session.Expunge">IMAP_Session.Expunge</b> event.
    /// </summary>
    public class IMAP_e_Expunge : EventArgs
    {
        private IMAP_r_ServerStatus m_pResponse;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="folder">Folder name with optional path.</param>
        /// <param name="msgInfo">Message info.</param>
        /// <param name="response">Default IMAP server response.</param>
        /// <exception cref="ArgumentNullException">Is riased when <b>folder</b>,<b>msgInfo</b> or <b>response</b> is null reference.</exception>
        internal IMAP_e_Expunge(string folder, IMAP_MessageInfo msgInfo, IMAP_r_ServerStatus response)
        {
            m_pResponse = response ?? throw new ArgumentNullException("response");
            Folder = folder ?? throw new ArgumentNullException("folder");
            MessageInfo = msgInfo ?? throw new ArgumentNullException("msgInfo");
        }

        /// <summary>
        /// Gets folder name with optional path.
        /// </summary>
        public string Folder { get; }

        /// <summary>
        /// Gets message info.
        /// </summary>
        public IMAP_MessageInfo MessageInfo { get; }

        /// <summary>
        /// Gets or sets IMAP server response to this operation.
        /// </summary>
        /// <exception cref="ArgumentNullException">Is raised when null reference value set.</exception>
        public IMAP_r_ServerStatus Response
        {
            get => m_pResponse;

            set => m_pResponse = value ?? throw new ArgumentNullException("value");
        }
    }
}
