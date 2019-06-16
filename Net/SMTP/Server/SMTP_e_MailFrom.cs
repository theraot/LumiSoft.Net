﻿using System;

namespace LumiSoft.Net.SMTP.Server
{
    /// <summary>
    /// This class provided data for <b cref="SMTP_Session.MailFrom">SMTP_Session.MailFrom</b> event.
    /// </summary>
    public class SMTP_e_MailFrom : EventArgs
    {
        private SMTP_Reply    m_pReply;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="session">Owner SMTP server session.</param>
        /// <param name="from">MAIL FROM: value.</param>
        /// <param name="reply">SMTP server reply.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>session</b>, <b>from</b> or <b>reply</b> is null reference.</exception>
        public SMTP_e_MailFrom(SMTP_Session session,SMTP_MailFrom from,SMTP_Reply reply)
        {
            if(session == null){
                throw new ArgumentNullException("session");
            }
            if(from == null){
                throw new ArgumentNullException("from");
            }
            if(reply == null){
                throw new ArgumentNullException("reply");
            }

            Session  = session;
            MailFrom = from;
            m_pReply    = reply;
        }

        /// <summary>
        /// Gets owner SMTP session.
        /// </summary>
        public SMTP_Session Session { get; }

        /// <summary>
        /// Gets MAIL FROM: value.
        /// </summary>
        public SMTP_MailFrom MailFrom { get; }

        /// <summary>
        /// Gets or sets SMTP server reply.
        /// </summary>
        /// <exception cref="ArgumentNullException">Is raised when null reference passed.</exception>
        public SMTP_Reply Reply
        {
            get{ return m_pReply; }

            set{
                if(value == null){
                    throw new ArgumentNullException("Reply");
                }

                m_pReply = value;
            }
        }
    }
}
