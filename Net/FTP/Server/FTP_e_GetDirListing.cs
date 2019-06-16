﻿using System;
using System.Collections.Generic;

namespace LumiSoft.Net.FTP.Server
{
    /// <summary>
    /// This class provides data for <see cref="FTP_Session.GetDirListing"/> event.
    /// </summary>
    public class FTP_e_GetDirListing : EventArgs
    {
        private readonly string             m_Path;
        private readonly List<FTP_ListItem> m_pItems;
        private FTP_t_ReplyLine[]  m_pReplyLines;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="path">Path.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>path</b> is null reference.</exception>
        public FTP_e_GetDirListing(string path)
        {
            if(path == null){
                throw new ArgumentNullException("path");
            }

            m_Path = path;

            m_pItems = new List<FTP_ListItem>();
        }


        #region Properties implementation

        /// <summary>
        /// Gets or sets error response.
        /// </summary>
        public FTP_t_ReplyLine[] Error
        {
            get{ return m_pReplyLines; }

            set{ m_pReplyLines = value; }
        }

        /// <summary>
        /// Gets path which list items to get.
        /// </summary>
        public string Path
        {
            get{ return m_Path; }
        }

        /// <summary>
        /// Gets directory list items.
        /// </summary>
        public List<FTP_ListItem> Items
        {
            get{ return m_pItems; }
        }

        #endregion
    }
}
