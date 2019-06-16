﻿using System;
using System.IO;
using System.Text;

using LumiSoft.Net.IMAP.Server;

namespace LumiSoft.Net.IMAP
{
    /// <summary>
    /// This is base class for any IMAP server response.
    /// </summary>
    public abstract class IMAP_r
    {
        /// <summary>
        /// Returns this as string.
        /// </summary>
        /// <param name="encoding">Specifies how mailbox name is encoded.</param>
        /// <returns>Returns this as string.</returns>
        public virtual string ToString(IMAP_Mailbox_Encoding encoding)
        {
            return ToString();
        }

        /// <summary>
        /// Starts writing response to the specified stream.
        /// </summary>
        /// <param name="stream">Stream where to store response.</param>
        /// <param name="mailboxEncoding">Specifies how mailbox name is encoded.</param>
        /// <param name="completedAsyncCallback">Callback to be called when this method completes asynchronously.</param>
        /// <returns>Returns true is method completed asynchronously(the completedAsyncCallback is raised upon completion of the operation).
        /// Returns false if operation completed synchronously.</returns>
        /// <exception cref="ArgumentNullException">Is raised when <b>stream</b> is null reference.</exception>
        public bool ToStreamAsync(Stream stream,IMAP_Mailbox_Encoding mailboxEncoding,EventHandler<EventArgs<Exception>> completedAsyncCallback)
        {
            if(stream == null){
                throw new ArgumentNullException("stream");
            }

            return ToStreamAsync(null,stream,mailboxEncoding,completedAsyncCallback);
        }

        /// <summary>
        /// Starts sending response to the specified IMAP session remote endpoint.
        /// </summary>
        /// <param name="session">Stream where to store response.</param>
        /// <param name="completedAsyncCallback">Callback to be called when this method completes asynchronously.</param>
        /// <returns>Returns true is method completed asynchronously(the completedAsyncCallback is raised upon completion of the operation).
        /// Returns false if operation completed synchronously.</returns>
        /// <exception cref="ArgumentNullException">Is raised when <b>session</b> is null reference.</exception>
        internal bool SendAsync(IMAP_Session session,EventHandler<EventArgs<Exception>> completedAsyncCallback)
        {
            if(session == null){
                throw new ArgumentNullException("session");
            }

            return ToStreamAsync(session,session.TcpStream,session.MailboxEncoding,completedAsyncCallback);
        }

        /// <summary>
        /// Starts writing response to the specified stream.
        /// </summary>
        /// <param name="session">Owner IMAP session.</param>
        /// <param name="stream">Stream where to store response.</param>
        /// <param name="mailboxEncoding">Specifies how mailbox name is encoded.</param>
        /// <param name="completedAsyncCallback">Callback to be called when this method completes asynchronously.</param>
        /// <returns>Returns true is method completed asynchronously(the completedAsyncCallback is raised upon completion of the operation).
        /// Returns false if operation completed synchronously.</returns>
        /// <exception cref="ArgumentNullException">Is raised when <b>stream</b> is null reference.</exception>
        protected virtual bool ToStreamAsync(IMAP_Session session,Stream stream,IMAP_Mailbox_Encoding mailboxEncoding,EventHandler<EventArgs<Exception>> completedAsyncCallback)
        {
            if(stream == null){
                throw new ArgumentNullException("stream");
            }

            var responseS = ToString(mailboxEncoding);
            var response  = Encoding.UTF8.GetBytes(responseS);

            // Log.
            if (session != null){
                session.LogAddWrite(response.Length,responseS.TrimEnd());
            }

            // Starts writing response to stream.
            var ar = stream.BeginWrite(
                response,
                0,
                response.Length,
                delegate(IAsyncResult r){                    
                    if(r.CompletedSynchronously){
                        return;
                    }

                    try{
                        stream.EndWrite(r);

                        if(completedAsyncCallback != null){
                            completedAsyncCallback(this,new EventArgs<Exception>(null));
                        }
                    }
                    catch(Exception x){
                        if(completedAsyncCallback != null){
                            completedAsyncCallback(this,new EventArgs<Exception>(x));
                        }
                    }
                },
                null
            );
            // Completed synchronously, process result.
            if (ar.CompletedSynchronously){
                stream.EndWrite(ar);

                return false;
            }
            // Completed asynchronously, stream.BeginWrite AsyncCallback will continue processing.

            return true;
        }
    }
}
