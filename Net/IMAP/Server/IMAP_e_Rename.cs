﻿using System;

namespace LumiSoft.Net.IMAP.Server
{
    /// <summary>
    /// This class provides data for <b cref="IMAP_Session.Rename">IMAP_Session.Rename</b> event.
    /// </summary>
    public class IMAP_e_Rename : EventArgs
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="cmdTag">Command tag.</param>
        /// <param name="currentFolder">Current folder name with optional path.</param>
        /// <param name="newFolder">New folder name with optional path.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>cmdTag</b>,<b>currentFolder</b> or <b>newFolder</b> is null reference.</exception>
        internal IMAP_e_Rename(string cmdTag, string currentFolder, string newFolder)
        {
            CmdTag = cmdTag ?? throw new ArgumentNullException("cmdTag");
            CurrentFolder = currentFolder ?? throw new ArgumentNullException("currentFolder");
            NewFolder = newFolder ?? throw new ArgumentNullException("newFolder");
        }

        /// <summary>
        /// Gets IMAP command tag value.
        /// </summary>
        public string CmdTag { get; }

        /// <summary>
        /// Gets current folder name with optional path.
        /// </summary>
        public string CurrentFolder { get; }

        /// <summary>
        /// Gets new folder name with optional path.
        /// </summary>
        public string NewFolder { get; }

        /// <summary>
        /// Gets or sets IMAP server response to this operation.
        /// </summary>
        public IMAP_r_ServerStatus Response { get; set; }
    }
}
