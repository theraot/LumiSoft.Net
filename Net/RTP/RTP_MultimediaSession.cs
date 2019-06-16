﻿using System;
using System.Collections.Generic;

namespace LumiSoft.Net.RTP
{
    /// <summary>
    /// This class represents RTP single-media and multimedia session.
    /// </summary>
    public class RTP_MultimediaSession : IDisposable
    {
        private RTP_Participant_Local                     m_pLocalParticipant;
        private List<RTP_Session>                         m_pSessions;
        private Dictionary<string,RTP_Participant_Remote> m_pParticipants;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="cname">Canonical name of participant. <seealso cref="LumiSoft.Net.RTP.RTP_Utils.GenerateCNAME"/>RTP_Utils.GenerateCNAME 
        /// can be used to create this value.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>cname</b> is null reference.</exception>
        /// <exception cref="ArgumentException">Is raised when any of the arguments has invalid value.</exception>
        public RTP_MultimediaSession(string cname)
        {
            if(cname == null){
                throw new ArgumentNullException("cname");
            }
            if(cname == string.Empty){
                throw new ArgumentException("Argument 'cname' value must be specified.");
            }

            m_pLocalParticipant = new RTP_Participant_Local(cname);
            m_pSessions = new List<RTP_Session>();
            m_pParticipants = new Dictionary<string,RTP_Participant_Remote>();
        }

        /// <summary>
        /// Cleans up any resources being used.
        /// </summary>
        public void Dispose()
        {
            if(IsDisposed){
                return;
            }
            foreach(RTP_Session session in m_pSessions.ToArray()){
                session.Dispose();
            }
            IsDisposed = true;

            m_pLocalParticipant = null;
            m_pSessions         = null;
            m_pParticipants     = null;            

            this.NewParticipant = null;
            this.Error = null;
        }

        /// <summary>
        /// Closes RTP multimedia session, sends BYE with optional reason text to remote targets.
        /// </summary>
        /// <param name="closeReason">Close reason. Value null means not specified.</param>
        /// <exception cref="ObjectDisposedException">Is raised when this class is Disposed and this method is accessed.</exception>
        public void Close(string closeReason)
        {
            if(IsDisposed){
                throw new ObjectDisposedException(this.GetType().Name);
            }

            foreach(RTP_Session session in m_pSessions.ToArray()){
                session.Close(closeReason);
            }

            Dispose();
        }

        /// <summary>
        /// Starts session.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this class is Disposed and this method is accessed.</exception>
        public void Start()
        {
            if(IsDisposed){
                throw new ObjectDisposedException(this.GetType().Name);
            }

            // TODO:
        }

        /// <summary>
        /// Stops session.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this class is Disposed and this method is accessed.</exception>
        public void Stop()
        {
            if(IsDisposed){
                throw new ObjectDisposedException(this.GetType().Name);
            }

            // TODO:
        }

        /// <summary>
        /// Creates new RTP session.
        /// </summary>
        /// <param name="localEP">Local RTP end point.</param>
        /// <param name="clock">RTP media clock.</param>
        /// <returns>Returns created session.</returns>
        /// <exception cref="ObjectDisposedException">Is raised when this class is Disposed and this method is accessed.</exception>
        /// <exception cref="ArgumentNullException">Is raised when <b>localEP</b> or <b>clock</b> is null reference.</exception>
        public RTP_Session CreateSession(RTP_Address localEP,RTP_Clock clock)
        {
            if(IsDisposed){
                throw new ObjectDisposedException(this.GetType().Name);
            }
            if(localEP == null){
                throw new ArgumentNullException("localEP");
            }
            if(clock == null){
                throw new ArgumentNullException("clock");
            }

            var session = new RTP_Session(this,localEP,clock);
            session.Disposed += new EventHandler(delegate(object s,EventArgs e){
                m_pSessions.Remove((RTP_Session)s);
            });
            m_pSessions.Add(session);

            OnSessionCreated(session);

            return session;
        }

        /// <summary>
        /// Gets or creates new participant if participant does not exist.
        /// </summary>
        /// <param name="cname">Participant canonical name.</param>
        /// <returns>Returns specified participant.</returns>
        internal RTP_Participant_Remote GetOrCreateParticipant(string cname)
        {
            if(cname == null){
                throw new ArgumentNullException("cname");
            }
            if(cname == string.Empty){
                throw new ArgumentException("Argument 'cname' value must be specified.");
            }

            lock(m_pParticipants){
                RTP_Participant_Remote participant = null;
                if(!m_pParticipants.TryGetValue(cname,out participant)){
                    participant = new RTP_Participant_Remote(cname);
                    participant.Removed += new EventHandler(delegate(object sender,EventArgs e){
                        m_pParticipants.Remove(participant.CNAME);
                    });
                    m_pParticipants.Add(cname,participant);

                    OnNewParticipant(participant);
                }

                return participant;
            }
        }

        /// <summary>
        /// Gets if this object is disposed.
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// Gets media sessions.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this class is Disposed and this property is accessed.</exception>
        public RTP_Session[] Sessions
        {
            get{ 
                if(IsDisposed){
                    throw new ObjectDisposedException(this.GetType().Name);
                }

                return m_pSessions.ToArray(); 
            }
        }

        /// <summary>
        /// Gets local participant.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this class is Disposed and this property is accessed.</exception>
        public RTP_Participant_Local LocalParticipant
        {
            get{ 
                if(IsDisposed){
                    throw new ObjectDisposedException(this.GetType().Name);
                }

                return m_pLocalParticipant; 
            }
        }

        /// <summary>
        /// Gets session remote participants.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this class is Disposed and this property is accessed.</exception>
        public RTP_Participant_Remote[] RemoteParticipants
        {
            get{ 
                if(IsDisposed){
                    throw new ObjectDisposedException(this.GetType().Name);
                }

                lock(m_pParticipants){
                    var retVal = new RTP_Participant_Remote[m_pParticipants.Count];
                    m_pParticipants.Values.CopyTo(retVal,0);

                    return retVal;
                }
            }
        }

        /// <summary>
        /// Is raised when new session has created.
        /// </summary>
        public event EventHandler<EventArgs<RTP_Session>> SessionCreated;

        /// <summary>
        /// Raises <b>SessionCreated</b> event.
        /// </summary>
        /// <param name="session">RTP session.</param>
        private void OnSessionCreated(RTP_Session session)
        {
            if(session == null){
                throw new ArgumentNullException("session");
            }

            if(this.SessionCreated != null){
                this.SessionCreated(this,new EventArgs<RTP_Session>(session));
            }
        }

        /// <summary>
        /// Is raised when new remote participant has joined to session.
        /// </summary>
        public event EventHandler<RTP_ParticipantEventArgs> NewParticipant;

        /// <summary>
        /// Raises <b>NewParticipant</b> event.
        /// </summary>
        /// <param name="participant">New participant.</param>
        private void OnNewParticipant(RTP_Participant_Remote participant)
        {
            if(this.NewParticipant != null){
                this.NewParticipant(this,new RTP_ParticipantEventArgs(participant));
            }
        }

        /// <summary>
        /// Is raised when unknown error has happened.
        /// </summary>
        public event EventHandler<ExceptionEventArgs> Error;

        /// <summary>
        /// Raises <b>Error</b> event.
        /// </summary>
        /// <param name="exception">Exception.</param>
        internal void OnError(Exception exception)
        {
            if(this.Error != null){
                this.Error(this,new ExceptionEventArgs(exception));
            }
        }
    }
}
