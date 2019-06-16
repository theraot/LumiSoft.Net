using System;

namespace LumiSoft.Net.IMAP.Server
{
    /// <summary>
    /// This class provides data for <b cref="IMAP_Session.Store">IMAP_Session.Store</b> event.
    /// </summary>
    public class IMAP_e_Store : EventArgs
    {
        private IMAP_r_ServerStatus m_pResponse;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="folder">Folder name with optional path.</param>
        /// <param name="msgInfo">Message info.</param>
        /// <param name="flagsSetType">Flags set type.</param>
        /// <param name="flags">Flags.</param>
        /// <param name="response">Default IMAP server response.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>folder</b>,<b>msgInfo</b>,<b>flags</b> or <b>response</b> is null reference.</exception>
        internal IMAP_e_Store(string folder,IMAP_MessageInfo msgInfo,IMAP_Flags_SetType flagsSetType,string[] flags,IMAP_r_ServerStatus response)
        {
            m_pResponse = response;
            Folder    = folder ?? throw new ArgumentNullException("folder");
            MessageInfo  = msgInfo ?? throw new ArgumentNullException("msgInfo");
            FlagsSetType   = flagsSetType;
            Flags    = flags ?? throw new ArgumentNullException("flags");
        }

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

        /// <summary>
        /// Gets IMAP message info.
        /// </summary>
        public IMAP_MessageInfo MessageInfo { get; }

        /// <summary>
        /// Gets flags set type.
        /// </summary>
        public IMAP_Flags_SetType FlagsSetType { get; } = IMAP_Flags_SetType.Replace;

        /// <summary>
        /// Gets flags.
        /// </summary>
        public string[] Flags { get; }
    }
}
