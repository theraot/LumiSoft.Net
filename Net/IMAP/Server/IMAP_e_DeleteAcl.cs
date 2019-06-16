using System;

namespace LumiSoft.Net.IMAP.Server
{
    /// <summary>
    /// This class provides data for <b cref="IMAP_Session.DeleteAcl">IMAP_Session.DeleteAcl</b> event.
    /// </summary>
    public class IMAP_e_DeleteAcl : EventArgs
    {
        private IMAP_r_ServerStatus m_pResponse;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="folder">Folder name with optional path.</param>
        /// <param name="identifier">ACL identifier (normally user or group name).</param>
        /// <param name="response">Default IMAP server response.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>folder</b>,<b>identifier</b> or <b>response</b> is null reference.</exception>
        internal IMAP_e_DeleteAcl(string folder, string identifier, IMAP_r_ServerStatus response)
        {
            m_pResponse = response ?? throw new ArgumentNullException("response");
            Folder = folder ?? throw new ArgumentNullException("folder");
            Identifier = identifier ?? throw new ArgumentNullException("identifier");
        }

        /// <summary>
        /// Gets folder name with optional path.
        /// </summary>
        public string Folder { get; }

        /// <summary>
        /// Gets ACL identifier (normally user or group name).
        /// </summary>
        public string Identifier { get; }

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
