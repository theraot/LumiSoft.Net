namespace LumiSoft.Net.IMAP
{
    /// <summary>
    /// This class represents IMAP FETCH response X-GM-MSGID data-item. Defined in <see href="http://code.google.com/intl/et/apis/gmail/imap">GMail API</see>.
    /// </summary>
    public class IMAP_t_Fetch_r_i_X_GM_MSGID : IMAP_t_Fetch_r_i
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="msgID">Message ID.</param>
        public IMAP_t_Fetch_r_i_X_GM_MSGID(ulong msgID)
        {
            MsgID = msgID;
        }


        #region Properties implementation

        /// <summary>
        /// Gets message ID.
        /// </summary>
        public ulong MsgID { get; }

#endregion
    }
}
