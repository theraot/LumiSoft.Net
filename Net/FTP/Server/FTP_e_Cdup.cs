﻿using System;

namespace LumiSoft.Net.FTP.Server
{
    /// <summary>
    /// This class provides data for <see cref="FTP_Session.Cdup"/> event.
    /// </summary>
    public class FTP_e_Cdup : EventArgs
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public FTP_e_Cdup()
        {
        }


        #region Properties implementation

        /// <summary>
        /// Gets or sets FTP server response.
        /// </summary>
        public FTP_t_ReplyLine[] Response { get; set; }

#endregion
    }
}
