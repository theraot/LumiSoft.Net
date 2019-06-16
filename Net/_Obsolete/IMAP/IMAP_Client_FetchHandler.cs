using System;

namespace LumiSoft.Net.IMAP.Client
{
    /// <summary>
    /// This class provides IMAP FETCH response handling methods.
    /// </summary>
    [Obsolete("Use Fetch(bool uid,IMAP_t_SeqSet seqSet,IMAP_t_Fetch_i[] items,EventHandler<EventArgs<IMAP_r_u>> callback) intead.")]
    public class IMAP_Client_FetchHandler
    {

        //public event EventHandler BodyS = null;

        /// <summary>
        /// Is raised when current message FETCH BODY[] data-item is returned.
        /// </summary>
        public event EventHandler<IMAP_Client_Fetch_Body_EArgs> Body;

        //public event EventHandler BodyStructure = null;

        /// <summary>
        /// Is raised when current message FETCH ENVELOPE data-item is returned.
        /// </summary>
        public event EventHandler<EventArgs<IMAP_Envelope>> Envelope;

        /// <summary>
        /// Is raised when current message FETCH FLAGS data-item is returned.
        /// </summary>
        public event EventHandler<EventArgs<string[]>> Flags;

        /// <summary>
        /// Is raised when current message FETCH INTERNALDATE data-item is returned.
        /// </summary>
        public event EventHandler<EventArgs<DateTime>> InternalDate;

        /// <summary>
        /// This event is raised when current message changes and next message FETCH data-items will be returned.
        /// </summary>
        public event EventHandler NextMessage;

        /// <summary>
        /// Is raised when current message FETCH RFC822 data-item is returned.
        /// </summary>
        public event EventHandler<IMAP_Client_Fetch_Rfc822_EArgs> Rfc822;

        /// <summary>
        /// Is raised when current message FETCH RFC822.HEADER data-item is returned.
        /// </summary>
        public event EventHandler<EventArgs<string>> Rfc822Header;

        /// <summary>
        /// Is raised when current message FETCH RFC822.SIZE data-item is returned.
        /// </summary>
        public event EventHandler<EventArgs<int>> Rfc822Size;

        /// <summary>
        /// Is raised when current message FETCH RFC822.TEXT data-item is returned.
        /// </summary>
        public event EventHandler<EventArgs<string>> Rfc822Text;

        /// <summary>
        /// Is raised when current message FETCH UID data-item is returned.
        /// </summary>
        public event EventHandler<EventArgs<long>> UID;

        /// <summary>
        /// Is raised when current message FETCH GMail X-GM-MSGID data-item is returned.
        /// </summary>
        public event EventHandler<EventArgs<ulong>> X_GM_MSGID;

        /// <summary>
        /// Is raised when current message FETCH GMail X-GM-THRID data-item is returned.
        /// </summary>
        public event EventHandler<EventArgs<ulong>> X_GM_THRID;

        /// <summary>
        /// Gets current message sequence number. Value -1 means no current message.
        /// </summary>
        public int CurrentSeqNo { get; private set; } = -1;

        /// <summary>
        /// Raises <b>Body</b> event.
        /// </summary>
        /// <param name="eArgs">Event args.</param>
        internal void OnBody(IMAP_Client_Fetch_Body_EArgs eArgs)
        {
            if (Body != null)
            {
                Body(this, eArgs);
            }
        }

        /// <summary>
        /// Raises <b>Envelope</b> event.
        /// </summary>
        /// <param name="envelope">Envelope value.</param>
        internal void OnEnvelope(IMAP_Envelope envelope)
        {
            if (Envelope != null)
            {
                Envelope(this, new EventArgs<IMAP_Envelope>(envelope));
            }
        }

        /// <summary>
        /// Raises <b>Flags</b> event.
        /// </summary>
        /// <param name="flags">Message flags.</param>
        internal void OnFlags(string[] flags)
        {
            if (Flags != null)
            {
                Flags(this, new EventArgs<string[]>(flags));
            }
        }

        /// <summary>
        /// Raises <b>InternalDate</b> event.
        /// </summary>
        /// <param name="date">Message IMAP server internal date.</param>
        internal void OnInternalDate(DateTime date)
        {
            if (InternalDate != null)
            {
                InternalDate(this, new EventArgs<DateTime>(date));
            }
        }

        /// <summary>
        /// Raises <b>NextMessage</b> event.
        /// </summary>
        internal void OnNextMessage()
        {
            if (NextMessage != null)
            {
                NextMessage(this, new EventArgs());
            }
        }

        /// <summary>
        /// Raises <b>Rfc822</b> event.
        /// </summary>
        /// <param name="eArgs">Event args.</param>
        internal void OnRfc822(IMAP_Client_Fetch_Rfc822_EArgs eArgs)
        {
            if (Rfc822 != null)
            {
                Rfc822(this, eArgs);
            }
        }

        /// <summary>
        /// Raises <b>Rfc822Text</b> event.
        /// </summary>
        /// <param name="header">Message header.</param>
        internal void OnRfc822Header(string header)
        {
            if (Rfc822Header != null)
            {
                Rfc822Header(this, new EventArgs<string>(header));
            }
        }

        /// <summary>
        /// Raises <b>Rfc822Text</b> event.
        /// </summary>
        /// <param name="text">Message body text.</param>
        internal void OnRfc822Text(string text)
        {
            if (Rfc822Text != null)
            {
                Rfc822Text(this, new EventArgs<string>(text));
            }
        }

        /// <summary>
        /// Raises <b>Rfc822Size</b> event.
        /// </summary>
        /// <param name="size">Message size in bytes.</param>
        internal void OnSize(int size)
        {
            if (Rfc822Size != null)
            {
                Rfc822Size(this, new EventArgs<int>(size));
            }
        }

        /// <summary>
        /// Raises <b>UID</b> event.
        /// </summary>
        /// <param name="uid">Message UID value.</param>
        internal void OnUID(long uid)
        {
            if (UID != null)
            {
                UID(this, new EventArgs<long>(uid));
            }
        }

        /// <summary>
        /// Raises <b>X_GM_MSGID</b> event.
        /// </summary>
        /// <param name="msgID">Message ID.</param>
        internal void OnX_GM_MSGID(ulong msgID)
        {
            if (X_GM_MSGID != null)
            {
                X_GM_MSGID(this, new EventArgs<ulong>(msgID));
            }
        }

        /// <summary>
        /// Raises <b>X_GM_THRID</b> event.
        /// </summary>
        /// <param name="threadID">Message thread ID.</param>
        internal void OnX_GM_THRID(ulong threadID)
        {
            if (X_GM_THRID != null)
            {
                X_GM_THRID(this, new EventArgs<ulong>(threadID));
            }
        }
        /// <summary>
        /// Sets <b>CurrentSeqNo</b> property value.
        /// </summary>
        /// <param name="seqNo">Message sequnece number.</param>
        internal void SetCurrentSeqNo(int seqNo)
        {
            CurrentSeqNo = seqNo;
        }
    }
}
