﻿using System;

namespace LumiSoft.Net.IMAP.Server
{
    /// <summary>
    /// This class provides data for <b cref="IMAP_Session.Create">IMAP_Session.Create</b>,
    /// <b cref="IMAP_Session.Delete">IMAP_Session.Delete</b>,<b cref="IMAP_Session.Subscribe">IMAP_Session.Subscribe</b>,
    /// <b cref="IMAP_Session.Unsubscribe">IMAP_Session.Unsubscribe</b> events.
    /// </summary>
    public class IMAP_e_Folder : EventArgs
    {
        /// <summary>
        /// Defaultc constructor.
        /// </summary>
        /// <param name="cmdTag">Command tag.</param>
        /// <param name="folder">Folder name with optional path.</param>
        /// <param name="response">Default IMAP server response.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>cmdTag</b>,<b>folder</b> or <b>response</b> is null reference.</exception>
        /// <exception cref="ArgumentException">Is raised when any of the arguments has invalid value.</exception>
        internal IMAP_e_Folder(string cmdTag,string folder,IMAP_r_ServerStatus response)
        {
            if(cmdTag == null){
                throw new ArgumentNullException("cmdTag");
            }
            if(cmdTag == string.Empty){
                throw new ArgumentException("Argument 'cmdTag' value must be specified.","cmdTag");
            }

            Response = response ?? throw new ArgumentNullException("response");
            CmdTag    = cmdTag;
            Folder    = folder ?? throw new ArgumentNullException("folder");
        }

        /// <summary>
        /// Gets or sets IMAP server response to this operation.
        /// </summary>
        public IMAP_r_ServerStatus Response { get; set; }

        /// <summary>
        /// Gets IMAP command tag value.
        /// </summary>
        public string CmdTag { get; }

        /// <summary>
        /// Gets folder name with optional path.
        /// </summary>
        public string Folder { get; } = "";
    }
}
