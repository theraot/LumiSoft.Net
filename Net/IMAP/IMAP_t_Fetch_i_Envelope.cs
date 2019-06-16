﻿namespace LumiSoft.Net.IMAP
{
    /// <summary>
    /// This class represents FETCH request ENVELOPE argument(data-item). Defined in RFC 3501.
    /// </summary>
    public class IMAP_t_Fetch_i_Envelope : IMAP_t_Fetch_i
    {
        /// <summary>
        /// Returns this as string.
        /// </summary>
        /// <returns>Returns this as string.</returns>
        public override string ToString()
        {
            return "ENVELOPE";
        }
    }
}
