using System;
using System.Collections.Generic;

namespace LumiSoft.Net.FTP.Server
{
    /// <summary>
    /// This class provides data for <see cref="FTP_Session.GetDirListing"/> event.
    /// </summary>
    public class FTP_e_GetDirListing : EventArgs
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="path">Path.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>path</b> is null reference.</exception>
        public FTP_e_GetDirListing(string path)
        {
            Path = path ?? throw new ArgumentNullException("path");

            Items = new List<FTP_ListItem>();
        }

        /// <summary>
        /// Gets or sets error response.
        /// </summary>
        public FTP_t_ReplyLine[] Error { get; set; }

        /// <summary>
        /// Gets directory list items.
        /// </summary>
        public List<FTP_ListItem> Items { get; }

        /// <summary>
        /// Gets path which list items to get.
        /// </summary>
        public string Path { get; }
    }
}
