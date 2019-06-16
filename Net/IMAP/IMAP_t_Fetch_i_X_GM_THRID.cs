﻿namespace LumiSoft.Net.IMAP
{
    /// <summary>
    /// This class represents FETCH request GMail X-GM-THRID argument(data-item). Defined in <see href="http://code.google.com/intl/et/apis/gmail/imap">GMail API</see>.
    /// </summary>
    public class IMAP_t_Fetch_i_X_GM_THRID : IMAP_t_Fetch_i
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public IMAP_t_Fetch_i_X_GM_THRID()
        {
        }


        #region override method ToString

        /// <summary>
        /// Returns this as string.
        /// </summary>
        /// <returns>Returns this as string.</returns>
        public override string ToString()
        {
            return "X-GM-THRID";
        }

        #endregion
    }
}
