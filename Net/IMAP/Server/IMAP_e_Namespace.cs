using System;

namespace LumiSoft.Net.IMAP.Server
{
    /// <summary>
    /// This class provides data for <b cref="IMAP_Session.Namespace">IMAP_Session.Namespace</b> event.
    /// </summary>
    public class IMAP_e_Namespace : EventArgs
    {
        private IMAP_r_ServerStatus m_pResponse;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="response">Default IMAP server response.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>response</b> is null reference.</exception>
        internal IMAP_e_Namespace(IMAP_r_ServerStatus response)
        {
            m_pResponse = response ?? throw new ArgumentNullException("response");
        }

        /// <summary>
        /// Gets or sets IMAP server NAMESPACE response.
        /// </summary>
        public IMAP_r_u_Namespace NamespaceResponse { get; set; }

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
    }
}
