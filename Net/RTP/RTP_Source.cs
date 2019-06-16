﻿using System;
using System.Net;

namespace LumiSoft.Net.RTP
{
    /// <summary>
    /// This class represents RTP source.
    /// </summary>
    /// <remarks>Source indicates an entity sending packets, either RTP and/or RTCP.
    /// Sources what send RTP packets are called "active", only RTCP sending ones are "passive".
    /// Source can be local(we send RTP and/or RTCP remote party) or remote(remote party sends RTP and/or RTCP to us).
    /// </remarks>
    public abstract class RTP_Source
    {
        private RTP_Session          m_pSession;
        private uint                 m_SSRC;
        private IPEndPoint           m_pRtcpEP;
        private IPEndPoint           m_pRtpEP;
        private DateTime             m_LastRtcpPacket = DateTime.MinValue;
        private DateTime             m_LastRtpPacket  = DateTime.MinValue;
        private DateTime             m_LastActivity   = DateTime.Now;
        private readonly DateTime             m_LastRRTime     = DateTime.MinValue;
        private string               m_CloseReason;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="session">Owner RTP session.</param>
        /// <param name="ssrc">Synchronization source ID.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>session</b> is null reference.</exception>
        internal RTP_Source(RTP_Session session,uint ssrc)
        {
            if(session == null){
                throw new ArgumentNullException("session");
            }

            m_pSession = session;
            m_SSRC     = ssrc;
        }

        /// <summary>
        /// Cleans up any resources being used.
        /// </summary>
        internal virtual void Dispose()
        {   
            if(State == RTP_SourceState.Disposed){
                return;
            }
            OnDisposing();
            SetState(RTP_SourceState.Disposed);

            m_pSession = null;
            m_pRtcpEP = null;
            m_pRtpEP = null;

            this.Closed = null;
            this.Disposing = null;
            this.StateChanged = null;
        }

        /// <summary>
        /// Closes specified source.
        /// </summary>
        /// <param name="closeReason">Closing reason. Value null means not specified.</param>
        internal virtual void Close(string closeReason)
        {
            m_CloseReason = closeReason;

            OnClosed();
            Dispose();
        }

        /// <summary>
        /// Sets property <b>RtcpEP</b> value.
        /// </summary>
        /// <param name="ep">IP end point.</param>
        internal void SetRtcpEP(IPEndPoint ep)
        {
            m_pRtcpEP = ep;
        }

        /// <summary>
        /// Sets property <b>RtpEP</b> value.
        /// </summary>
        /// <param name="ep">IP end point.</param>
        internal void SetRtpEP(IPEndPoint ep)
        {
            m_pRtpEP = ep;
        }

        /// <summary>
        /// Sets source active/passive state.
        /// </summary>
        /// <param name="active">If true, source switches to active, otherwise to passive.</param>
        internal void SetActivePassive(bool active)
        {            
            if(active){
            }
            else{
            }

            // TODO:
        }

        /// <summary>
        /// Sets <b>LastRtcpPacket</b> property value.
        /// </summary>
        /// <param name="time">Time.</param>
        internal void SetLastRtcpPacket(DateTime time)
        {
            m_LastRtcpPacket = time;
            m_LastActivity = time;
        }

        /// <summary>
        /// Sets <b>LastRtpPacket</b> property value.
        /// </summary>
        /// <param name="time">Time.</param>
        internal void SetLastRtpPacket(DateTime time)
        {
            m_LastRtpPacket = time;
            m_LastActivity = time;
        }

        /// <summary>
        /// Sets property LastRR value.
        /// </summary>
        /// <param name="rr">RTCP RR report.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>rr</b> is null reference.</exception>
        internal void SetRR(RTCP_Packet_ReportBlock rr)
        {
            if(rr == null){
                throw new ArgumentNullException("rr");
            }
        }

        /// <summary>
        /// Generates new SSRC value. This must be called only if SSRC collision of local source.
        /// </summary>
        internal void GenerateNewSSRC()
        {
            m_SSRC = RTP_Utils.GenerateSSRC();
        }

        /// <summary>
        /// Sets source state.
        /// </summary>
        /// <param name="state">New source state.</param>
        protected void SetState(RTP_SourceState state)
        {
            if(State == RTP_SourceState.Disposed){
                return;
            }

            if(State != state){
                State = state;

                OnStateChaged();
            }
        }

        /// <summary>
        /// Gets source state.
        /// </summary>
        public RTP_SourceState State { get; private set; } = RTP_SourceState.Passive;

        /// <summary>
        /// Gets owner RTP session.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this class is Disposed and this property is accessed.</exception>
        public RTP_Session Session
        {
            get{
                if(State == RTP_SourceState.Disposed){
                    throw new ObjectDisposedException(this.GetType().Name);
                }

                return m_pSession; 
            }
        }

        /// <summary>
        /// Gets synchronization source ID.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this class is Disposed and this property is accessed.</exception>
        public uint SSRC
        {
            get{
                if(State == RTP_SourceState.Disposed){
                    throw new ObjectDisposedException(this.GetType().Name);
                }

                return m_SSRC; 
            }
        }

        /// <summary>
        /// Gets source RTCP end point. Value null means source haven't sent any RTCP packet.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this class is Disposed and this property is accessed.</exception>
        public IPEndPoint RtcpEP
        {
            get{
                if(State == RTP_SourceState.Disposed){
                    throw new ObjectDisposedException(this.GetType().Name);
                }

                return m_pRtcpEP; 
            }
        }

        /// <summary>
        /// Gets source RTP end point. Value null means source haven't sent any RTCP packet.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this class is Disposed and this property is accessed.</exception>
        public IPEndPoint RtpEP
        {
            get{
                if(State == RTP_SourceState.Disposed){
                    throw new ObjectDisposedException(this.GetType().Name);
                }

                return m_pRtpEP; 
            }
        }

        /// <summary>
        /// Gets if source is local or remote source.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this class is Disposed and this property is accessed.</exception>
        public abstract bool IsLocal
        {
            get;
        }

        /// <summary>
        /// Gets last time when source sent RTP or RCTP packet.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this class is Disposed and this property is accessed.</exception>
        public DateTime LastActivity
        {
            get{
                if(State == RTP_SourceState.Disposed){
                    throw new ObjectDisposedException(this.GetType().Name);
                }

                return m_LastActivity; 
            }
        }

        /// <summary>
        /// Gets last time when source sent RTCP packet.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this class is Disposed and this property is accessed.</exception>
        public DateTime LastRtcpPacket
        {
            get{
                if(State == RTP_SourceState.Disposed){
                    throw new ObjectDisposedException(this.GetType().Name);
                }

                return m_LastRtcpPacket; 
            }
        }

        /// <summary>
        /// Gets last time when source sent RTP packet.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this class is Disposed and this property is accessed.</exception>
        public DateTime LastRtpPacket
        {
            get{
                if(State == RTP_SourceState.Disposed){
                    throw new ObjectDisposedException(this.GetType().Name);
                }

                return m_LastRtpPacket; 
            }
        }

        /// <summary>
        /// Gets last time when source sent RTCP RR report.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this class is Disposed and this property is accessed.</exception>
        public DateTime LastRRTime
        {
            get{
                if(State == RTP_SourceState.Disposed){
                    throw new ObjectDisposedException(this.GetType().Name);
                }

                return m_LastRRTime; 
            }
        }

        /// <summary>
        /// Gets source closing reason. Value null means not specified.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this class is Disposed and this property is accessed.</exception>
        public string CloseReason
        {
            get{ 
                if(State == RTP_SourceState.Disposed){
                    throw new ObjectDisposedException(this.GetType().Name);
                }

                return m_CloseReason; 
            }
        }

        /// <summary>
        /// Gets or sets user data.
        /// </summary>
        public object Tag { get; set; }

        /// <summary>
        /// Gets source CNAME. Value null means that source not binded to participant.
        /// </summary>
        internal abstract string CName
        {
            get;
        }

        /// <summary>
        /// Is raised when source is closed (by BYE).
        /// </summary>
        public event EventHandler Closed;

        /// <summary>
        /// Raises <b>Closed</b> event.
        /// </summary>
        private void OnClosed()
        {
            if(this.Closed != null){
                this.Closed(this,new EventArgs());
            }
        }

        /// <summary>
        /// Is raised when source is disposing.
        /// </summary>
        public event EventHandler Disposing;

        /// <summary>
        /// Raises <b>Disposing</b> event.
        /// </summary>
        private void OnDisposing()
        {
            if(this.Disposing != null){
                this.Disposing(this,new EventArgs());
            }
        }

        /// <summary>
        /// Is raised when source state has changed.
        /// </summary>
        public event EventHandler StateChanged;

        /// <summary>
        /// Raises <b>StateChanged</b> event.
        /// </summary>
        private void OnStateChaged()
        {
            if(this.StateChanged != null){
                this.StateChanged(this,new EventArgs());
            }
        }
    }
}
