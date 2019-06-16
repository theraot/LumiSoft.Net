using System;
using System.IO;

namespace LumiSoft.Net.SMTP.Server
{
    /// <summary>
    /// This class provided data for <b cref="SMTP_Session.MessageStoringCompleted">SMTP_Session.MessageStoringCompleted</b> event.
    /// </summary>
    public class SMTP_e_MessageStored : EventArgs
    {
        private SMTP_Reply m_pReply;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="session">Owner SMTP server session.</param>
        /// <param name="stream">Message stream.</param>
        /// <param name="reply">SMTP server reply.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>session</b>, <b>stream</b> or <b>reply</b> is null reference.</exception>
        public SMTP_e_MessageStored(SMTP_Session session, Stream stream, SMTP_Reply reply)
        {
            Session = session ?? throw new ArgumentNullException("session");
            Stream = stream ?? throw new ArgumentNullException("stream");
            m_pReply = reply ?? throw new ArgumentNullException("reply");
        }

        /// <summary>
        /// Gets or sets SMTP server reply.
        /// </summary>
        /// <exception cref="ArgumentNullException">Is raised when null reference passed.</exception>
        public SMTP_Reply Reply
        {
            get => m_pReply;

            set => m_pReply = value ?? throw new ArgumentNullException("Reply");
        }

        /// <summary>
        /// Gets owner SMTP session.
        /// </summary>
        public SMTP_Session Session { get; }

        /// <summary>
        /// Gets message stream where message has stored.
        /// </summary>
        public Stream Stream { get; }
    }
}
