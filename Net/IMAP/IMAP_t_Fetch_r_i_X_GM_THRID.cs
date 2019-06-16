﻿namespace LumiSoft.Net.IMAP
{
    /// <summary>
    /// This class represents IMAP FETCH response X-GM-THRID data-item. Defined in <see href="http://code.google.com/intl/et/apis/gmail/imap">GMail API</see>.
    /// </summary>
    public class IMAP_t_Fetch_r_i_X_GM_THRID : IMAP_t_Fetch_r_i
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="threadID">Thread ID.</param>
        public IMAP_t_Fetch_r_i_X_GM_THRID(ulong threadID)
        {
            ThreadID = threadID;
        }


        /// <summary>
        /// Gets thread ID.
        /// </summary>
        public ulong ThreadID { get; }
    }
}
