﻿using System;

namespace LumiSoft.Net.IMAP
{
    /// <summary>
    /// This is class represents IMAP server <b>COPYUID</b> optional response code. Defined in RFC 4315.
    /// </summary>
    public class IMAP_t_orc_CopyUid : IMAP_t_orc
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="targetMailboxUid">Target folde UID value.</param>
        /// <param name="sourceSeqSet">Source messages UID's.</param>
        /// <param name="targetSeqSet">Target messages UID's.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>sourceSeqSet</b> or <b>targetSeqSet</b> is null reference.</exception>
        public IMAP_t_orc_CopyUid(long targetMailboxUid, IMAP_t_SeqSet sourceSeqSet, IMAP_t_SeqSet targetSeqSet)
        {
            TargetMailboxUid = targetMailboxUid;
            SourceSeqSet = sourceSeqSet ?? throw new ArgumentNullException("sourceSeqSet");
            TargetSeqSet = targetSeqSet ?? throw new ArgumentNullException("targetSeqSet");
        }

        /// <summary>
        /// Gets source messages UID sequence set.
        /// </summary>
        public IMAP_t_SeqSet SourceSeqSet { get; }

        /// <summary>
        /// Gets target mailbox UID value.
        /// </summary>
        public long TargetMailboxUid { get; }

        /// <summary>
        /// Gets target messages UID sequence set.
        /// </summary>
        public IMAP_t_SeqSet TargetSeqSet { get; }

        /// <summary>
        /// Parses COPYUID optional response from string.
        /// </summary>
        /// <param name="value">COPYUID optional response string.</param>
        /// <returns>Returns COPYUID optional response.</returns>
        /// <exception cref="ArgumentNullException">Is raised when <b>value</b> is null reference.</exception>
        public new static IMAP_t_orc_CopyUid Parse(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            /* RFC 4315 3.
                COPYUID
                    Followed by the UIDVALIDITY of the destination mailbox, a UID set
                    containing the UIDs of the message(s) in the source mailbox that
                    were copied to the destination mailbox and containing the UIDs
                    assigned to the copied message(s) in the destination mailbox,
                    indicates that the message(s) have been copied to the destination
                    mailbox with the stated UID(s).
            */

            var code_mailboxUid_sourceSeqSet_targetSeqSet = value.Split(new[] { ' ' }, 4);
            if (!string.Equals("COPYUID", code_mailboxUid_sourceSeqSet_targetSeqSet[0], StringComparison.InvariantCultureIgnoreCase))
            {
                throw new ArgumentException("Invalid COPYUID response value.", "value");
            }
            if (code_mailboxUid_sourceSeqSet_targetSeqSet.Length != 4)
            {
                throw new ArgumentException("Invalid COPYUID response value.", "value");
            }

            return new IMAP_t_orc_CopyUid(
                Convert.ToInt64(code_mailboxUid_sourceSeqSet_targetSeqSet[1]),
                IMAP_t_SeqSet.Parse(code_mailboxUid_sourceSeqSet_targetSeqSet[2]),
                IMAP_t_SeqSet.Parse(code_mailboxUid_sourceSeqSet_targetSeqSet[3])
            );
        }

        /// <summary>
        /// Returns this as string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "COPYUID " + "m_MailboxUid" + " " + "m_MessageUid";
        }
    }
}
