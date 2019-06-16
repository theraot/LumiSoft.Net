using System.IO;

namespace LumiSoft.Net.SMTP.Relay
{
    /// <summary>
    /// Thsi class holds Relay_Queue queued item.
    /// </summary>
    public class Relay_QueueItem
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="queue">Item owner queue.</param>
        /// <param name="from">Sender address.</param>
        /// <param name="envelopeID">Envelope ID_(MAIL FROM: ENVID).</param>
        /// <param name="ret">Specifies what parts of message are returned in DSN report.</param>
        /// <param name="to">Target recipient address.</param>
        /// <param name="originalRecipient">Original recipient(RCPT TO: ORCPT).</param>
        /// <param name="notify">DSN notify condition.</param>
        /// <param name="messageID">Message ID.</param>
        /// <param name="message">Raw mime message. Message reading starts from current position.</param>
        /// <param name="tag">User data.</param>
        internal Relay_QueueItem(Relay_Queue queue, string from, string envelopeID, SMTP_DSN_Ret ret, string to, string originalRecipient, SMTP_DSN_Notify notify, string messageID, Stream message, object tag)
        {
            Queue = queue;
            From = from;
            EnvelopeID = envelopeID;
            DSN_Ret = ret;
            To = to;
            OriginalRecipient = originalRecipient;
            DSN_Notify = notify;
            MessageID = messageID;
            MessageStream = message;
            Tag = tag;
        }

        /// <summary>
        /// Gets DSN Notify value.
        /// </summary>
        public SMTP_DSN_Notify DSN_Notify { get; } = SMTP_DSN_Notify.NotSpecified;

        /// <summary>
        /// Gets DSN RET value.
        /// </summary>
        public SMTP_DSN_Ret DSN_Ret { get; } = SMTP_DSN_Ret.NotSpecified;

        /// <summary>
        /// Gets DSN ENVID value.
        /// </summary>
        public string EnvelopeID { get; }

        /// <summary>
        /// Gets from address.
        /// </summary>
        public string From { get; } = "";

        /// <summary>
        /// Gets message ID which is being relayed now.
        /// </summary>
        public string MessageID { get; } = "";

        /// <summary>
        /// Gets raw mime message which must be relayed.
        /// </summary>
        public Stream MessageStream { get; }

        /// <summary>
        /// Gets DSN ORCPT value.
        /// </summary>
        public string OriginalRecipient { get; }

        /// <summary>
        /// Gets this relay item owner queue.
        /// </summary>
        public Relay_Queue Queue { get; }

        /// <summary>
        /// Gets or sets user data.
        /// </summary>
        public object Tag { get; set; }

        /// <summary>
        /// Gets target recipient.
        /// </summary>
        public string To { get; } = "";
    }
}
