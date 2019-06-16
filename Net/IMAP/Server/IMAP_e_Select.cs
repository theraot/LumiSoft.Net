using System;
using System.Collections.Generic;

namespace LumiSoft.Net.IMAP.Server
{
    /// <summary>
    /// This class provides data for <b cref="IMAP_Session.Select">IMAP_Session.Select</b> event.
    /// </summary>
    public class IMAP_e_Select : EventArgs
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="cmdTag">Command tag.</param>
        /// <param name="folder">Folder name with optional path.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>cmdTag</b> or <b>folder</b> is null reference.</exception>
        internal IMAP_e_Select(string cmdTag,string folder)
        {
            CmdTag = cmdTag ?? throw new ArgumentNullException("cmdTag");
            Folder = folder ?? throw new ArgumentNullException("folder");

            Flags = new List<string>();
            PermanentFlags = new List<string>();

            // Add default falgs.
            Flags.AddRange(new string[]{"\\Answered","\\Flagged","\\Deleted","\\Seen","\\Draft"});
            PermanentFlags.AddRange(new string[]{"\\Answered","\\Flagged","\\Deleted","\\Seen","\\Draft"});
        }

        /// <summary>
        /// Gets command tag.
        /// </summary>
        public string CmdTag { get; }

        /// <summary>
        /// Gets or sets IMAP server error response to this operation. Value means no error.
        /// </summary>
        public IMAP_r_ServerStatus ErrorResponse { get; set; }

        /// <summary>
        /// Gets folder name with optional path.
        /// </summary>
        public string Folder { get; }

        /// <summary>
        /// Gets or sets if specified folder is read-only.
        /// </summary>
        public bool IsReadOnly { get; set; }

        /// <summary>
        /// Gets or sets folder UID value. Value 0 means not specified.
        /// </summary>
        public int FolderUID { get; set; }

        /// <summary>
        /// Gets folder supported flags collection.
        /// </summary>
        public List<string> Flags { get; }

        /// <summary>
        /// Gets folder supported permanent flags collection.
        /// </summary>
        public List<string> PermanentFlags { get; }
    }
}
