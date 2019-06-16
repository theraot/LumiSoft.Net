﻿using System;

namespace LumiSoft.Net.SMTP.Server
{
    /// <summary>
    /// This class provided data for <b cref="SMTP_Session.Ehlo">SMTP_Session.Ehlo</b> event.
    /// </summary>
    public class SMTP_e_Ehlo : EventArgs
    {
        private SMTP_Reply   m_pReply;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="session">Owner SMTP server session.</param>
        /// <param name="domain">Ehlo/Helo domain name.</param>
        /// <param name="reply">SMTP server reply.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>session</b>, <b>domain</b> or <b>reply</b> is null reference.</exception>
        /// <exception cref="ArgumentException">Is raised when any of the arguments has invalid value.</exception>
        public SMTP_e_Ehlo(SMTP_Session session,string domain,SMTP_Reply reply)
        {
            if(session == null){
                throw new ArgumentNullException("session");
            }
            if(domain == null){
                throw new ArgumentNullException("domain");
            }
            if(domain == string.Empty){
                throw new ArgumentException("Argument 'domain' value must be sepcified.","domain");
            }
            if(reply == null){
                throw new ArgumentNullException("reply");
            }

            Session = session;
            Domain   = domain;
            m_pReply   = reply;
        }

        /// <summary>
        /// Gets owner SMTP session.
        /// </summary>
        public SMTP_Session Session { get; }

        /// <summary>
        /// Gets connected client reported domain name.
        /// </summary>
        public string Domain { get; } = "";

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
