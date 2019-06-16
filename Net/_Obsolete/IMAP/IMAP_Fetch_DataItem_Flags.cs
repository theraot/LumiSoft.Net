﻿using System;

namespace LumiSoft.Net.IMAP
{
    /// <summary>
    /// This class represents FETCH FLAGS data item. Defined in RFC 3501.
    /// </summary>
    [Obsolete("Use Fetch(bool uid,IMAP_t_SeqSet seqSet,IMAP_t_Fetch_i[] items,EventHandler<EventArgs<IMAP_r_u>> callback) intead.")]
    public class IMAP_Fetch_DataItem_Flags : IMAP_Fetch_DataItem
    {
        /// <summary>
        /// Returns this as string.
        /// </summary>
        /// <returns>Returns this as string.</returns>
        public override string ToString()
        {
            return "FLAGS";
        }
    }
}
