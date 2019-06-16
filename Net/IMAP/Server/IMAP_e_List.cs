using System;
using System.Collections.Generic;

namespace LumiSoft.Net.IMAP.Server
{
    /// <summary>
    /// This class provides data for <b cref="IMAP_Session.List">IMAP_Session.List</b> event.
    /// </summary>
    public class IMAP_e_List : EventArgs
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="referenceName">Folder reference name.</param>
        /// <param name="folderFilter">Folder filter.</param>
        internal IMAP_e_List(string referenceName,string folderFilter)
        {
            FolderReferenceName = referenceName;
            FolderFilter        = folderFilter;

            Folders = new List<IMAP_r_u_List>();
        }


        /// <summary>
        /// Gets folder reference name. Value null means not specified.
        /// </summary>
        public string FolderReferenceName { get; }

        /// <summary>
        /// Gets folder filter.
        /// </summary>
        /// <remarks>
        /// The character "*" is a wildcard, and matches zero or more
        /// characters at this position.  The character "%" is similar to "*",
        /// but it does not match a hierarchy delimiter.  If the "%" wildcard
        /// is the last character of a mailbox name argument, matching levels
        /// of hierarchy are also returned.
        /// </remarks>
        public string FolderFilter { get; }

        /// <summary>
        /// Gets IMAP folders collection.
        /// </summary>
        public List<IMAP_r_u_List> Folders { get; }
    }
}
