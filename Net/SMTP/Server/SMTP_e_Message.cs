using System;
using System.IO;

namespace LumiSoft.Net.SMTP.Server
{
    /// <summary>
    /// This class provided data for <b cref="SMTP_Session.GetMessageStream">SMTP_Session.GetMessageStream</b> event.
    /// </summary>
    public class SMTP_e_Message : EventArgs
    {
        private Stream       m_pStream;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="session">Owner SMTP server session.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>session</b> is null reference.</exception>
        public SMTP_e_Message(SMTP_Session session)
        {
            if(session == null){
                throw new ArgumentNullException("session");
            }

            Session = session;
        }


        /// <summary>
        /// Gets owner SMTP session.
        /// </summary>
        public SMTP_Session Session { get; }

        /// <summary>
        /// Gets or stes stream where to store incoming message.
        /// </summary>
        /// <exception cref="ArgumentNullException">Is raised when null reference is passed.</exception>
        public Stream Stream
        {
            get{ return m_pStream; }

            set{ 
                if(value == null){
                    throw new ArgumentNullException("Stream");
                }

                m_pStream = value; 
            }
        }
    }
}
