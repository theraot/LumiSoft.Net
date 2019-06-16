﻿using System;

using LumiSoft.Net.TCP;

namespace LumiSoft.Net.IMAP.Server
{
    /// <summary>
    /// This class implements IMAPv4 server. Defined RFC 3501.
    /// </summary>
    public class IMAP_Server : TCP_Server<IMAP_Session>
    {
        private string m_GreetingText   = "";
        private int    m_MaxBadCommands = 30;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public IMAP_Server()
        {
        }


        /// <summary>
        /// Is called when new incoming session and server maximum allowed connections exceeded.
        /// </summary>
        /// <param name="session">Incoming session.</param>
        /// <remarks>This method allows inhereted classes to report error message to connected client.
        /// Session will be disconnected after this method completes.
        /// </remarks>
        protected override void OnMaxConnectionsExceeded(IMAP_Session session)
        {
            session.TcpStream.WriteLine("* NO Client host rejected: too many connections, please try again later.");
        }

        /// <summary>
        /// Is called when new incoming session and server maximum allowed connections per connected IP exceeded.
        /// </summary>
        /// <param name="session">Incoming session.</param>
        /// <remarks>This method allows inhereted classes to report error message to connected client.
        /// Session will be disconnected after this method completes.
        /// </remarks>
        protected override void OnMaxConnectionsPerIPExceeded(IMAP_Session session)
        {
            session.TcpStream.WriteLine("* NO Client host rejected: too many connections from your IP(" + session.RemoteEndPoint.Address + "), please try again later.");
        }


        /// <summary>
        /// Gets or sets server greeting text.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this property is accessed.</exception>
        public string GreetingText
        {
            get{                
                if(this.IsDisposed){
                    throw new ObjectDisposedException(this.GetType().Name);
                }

                return m_GreetingText; }

            set{
                if(this.IsDisposed){
                    throw new ObjectDisposedException(this.GetType().Name);
                }

                m_GreetingText = value;
            }
        }

        /// <summary>
        /// Gets or sets how many bad commands session can have before it's terminated. Value 0 means unlimited.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this property is accessed.</exception>
        /// <exception cref="ArgumentException">Is raised when invalid value is passed.</exception>
        public int MaxBadCommands
        {
            get{
                if(this.IsDisposed){
                    throw new ObjectDisposedException(this.GetType().Name);
                }

                return m_MaxBadCommands; 
            }

            set{
                if(this.IsDisposed){
                    throw new ObjectDisposedException(this.GetType().Name);
                }
                if(value < 0){
                    throw new ArgumentException("Property 'MaxBadCommands' value must be >= 0.");
                }

                m_MaxBadCommands = value;
            }
        }
    }
}
