using System;

namespace LumiSoft.Net.IMAP
{
    /// <summary>
    /// This is class represents IMAP server <b>APPENDUID</b> optional response code. Defined in RFC 4315.
    /// </summary>
    public class IMAP_t_orc_AppendUid : IMAP_t_orc
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="mailboxUid">Mailbox UID value.</param>
        /// <param name="msgUid">Message UID value.</param>
        public IMAP_t_orc_AppendUid(long mailboxUid,int msgUid)
        {
            MailboxUid = mailboxUid;
            MessageUid = msgUid;
        }

        /// <summary>
        /// Parses APPENDUID optional response from string.
        /// </summary>
        /// <param name="value">APPENDUID optional response string.</param>
        /// <returns>Returns APPENDUID optional response.</returns>
        /// <exception cref="ArgumentNullException">Is raised when <b>value</b> is null reference.</exception>
        public new static IMAP_t_orc_AppendUid Parse(string value)
        {
            if(value == null){
                throw new ArgumentNullException("value");
            }

            /* RFC 4315 3.
                APPENDUID
                  Followed by the UIDVALIDITY of the destination mailbox and the UID
                  assigned to the appended message in the destination mailbox,
                  indicates that the message has been appended to the destination
                  mailbox with that UID.
            */

            string[] code_mailboxUid_msgUid = value.Split(new char[]{' '},3);
            if(!string.Equals("APPENDUID",code_mailboxUid_msgUid[0],StringComparison.InvariantCultureIgnoreCase)){
                throw new ArgumentException("Invalid APPENDUID response value.","value");
            }
            if(code_mailboxUid_msgUid.Length != 3){
                throw new ArgumentException("Invalid APPENDUID response value.","value");
            }

            return new IMAP_t_orc_AppendUid(Convert.ToInt64(code_mailboxUid_msgUid[1]),Convert.ToInt32(code_mailboxUid_msgUid[2]));
        }

        /// <summary>
        /// Returns this as string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "APPENDUID " + MailboxUid + " " + MessageUid;
        }

        /// <summary>
        /// Gets mailbox(folder) UID value.
        /// </summary>
        public long MailboxUid { get; }

        /// <summary>
        /// Gets message UID value.
        /// </summary>
        public int MessageUid { get; }
    }
}
