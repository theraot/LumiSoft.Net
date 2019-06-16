using System;
using System.Collections.Generic;

using LumiSoft.Net.RTP;
using LumiSoft.Net.Media.Codec.Audio;

namespace LumiSoft.Net.Media
{
    /// <summary>
    /// This class implements audio-in (eg. microphone,line-in device) device RTP audio sending.
    /// </summary>
    public class AudioIn_RTP : IDisposable
    {
        private bool                       m_IsRunning;
        private AudioInDevice              m_pAudioInDevice;
        private readonly int                        m_AudioFrameSize = 20;
        private Dictionary<int,AudioCodec> m_pAudioCodecs;
        private RTP_SendStream             m_pRTP_Stream;
        private AudioCodec                 m_pActiveCodec;
        private _WaveIn                    m_pWaveIn;
        private uint                       m_RtpTimeStamp;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="audioInDevice">Audio-in device to capture.</param>
        /// <param name="audioFrameSize">Audio frame size in milliseconds.</param>
        /// <param name="codecs">Audio codecs with RTP payload number. For example: 0-PCMU,8-PCMA.</param>
        /// <param name="stream">RTP stream to use for audio sending.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>audioInDevice</b>,<b>codecs</b> or <b>stream</b> is null reference.</exception>
        public AudioIn_RTP(AudioInDevice audioInDevice,int audioFrameSize,Dictionary<int,AudioCodec> codecs,RTP_SendStream stream)
        {
            m_pAudioInDevice = audioInDevice ?? throw new ArgumentNullException("audioInDevice");
            m_AudioFrameSize = audioFrameSize;
            m_pAudioCodecs   = codecs ?? throw new ArgumentNullException("codecs");
            m_pRTP_Stream    = stream ?? throw new ArgumentNullException("stream");

            m_pRTP_Stream.Session.PayloadChanged += new EventHandler(m_pRTP_Stream_PayloadChanged);
            m_pAudioCodecs.TryGetValue(m_pRTP_Stream.Session.Payload,out m_pActiveCodec);
        }

        /// <summary>
        /// Cleans up any resources being used.
        /// </summary>
        public void Dispose()
        {
            if(IsDisposed){
                return;
            }

            Stop();

            IsDisposed = true;

            Error        = null;
            m_pAudioInDevice  = null;
            m_pAudioCodecs    = null;
            m_pRTP_Stream.Session.PayloadChanged -= new EventHandler(m_pRTP_Stream_PayloadChanged);
            m_pRTP_Stream     = null;
            m_pActiveCodec    = null;
        }

        /// <summary>
        /// Is called when RTP session sending payload has changed.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">Event data.</param>
        private void m_pRTP_Stream_PayloadChanged(object sender,EventArgs e)
        {
            if(m_IsRunning){
                Stop();

                m_pActiveCodec = null;
                m_pAudioCodecs.TryGetValue(m_pRTP_Stream.Session.Payload,out m_pActiveCodec);

                Start();
            }
        }

        /// <summary>
        /// Is called when wave-in has received new audio frame.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">Event data.</param>
        private void m_pWaveIn_AudioFrameReceived(object sender,EventArgs<byte[]> e)
        {
            try{                
                // We don't have RTP timestamp base or time stamp recycled.
                if(m_RtpTimeStamp == 0 || m_RtpTimeStamp > m_pRTP_Stream.Session.RtpClock.RtpTimestamp){
                    m_RtpTimeStamp = m_pRTP_Stream.Session.RtpClock.RtpTimestamp;
                }
                // Some sample block missing or silence suppression.
                // Don't work ... need some more investigation.
                //else if((m_pRTP_Stream.Session.RtpClock.RtpTimestamp - m_RtpTimeStamp) > 2 * m_pRTP_Stream.Session.RtpClock.MillisecondsToRtpTicks(m_AudioFrameSize)){
                //    m_RtpTimeStamp = m_pRTP_Stream.Session.RtpClock.RtpTimestamp;
                //}
                else{
                    m_RtpTimeStamp += (uint)m_pRTP_Stream.Session.RtpClock.MillisecondsToRtpTicks(m_AudioFrameSize);
                }

                if(m_pActiveCodec != null){
                    var rtpPacket = new RTP_Packet();
                    rtpPacket.Data = m_pActiveCodec.Encode(e.Value,0,e.Value.Length);
                    rtpPacket.Timestamp = m_RtpTimeStamp;
 	        
                    m_pRTP_Stream.Send(rtpPacket);
                }
            }
            catch(Exception x){
                if(!IsDisposed){
                    // Raise error event(We can't throw expection directly, we are on threadpool thread).
                    OnError(x);
                }
            }
        }

        /// <summary>
        /// Starts capturing from audio-in device and sending it to RTP stream.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this method is accessed.</exception>
        public void Start()
        {
            if(IsDisposed){
                throw new ObjectDisposedException(GetType().Name);
            }
            if(m_IsRunning){
                return;
            }

            m_IsRunning = true;

            if(m_pActiveCodec != null){
                // Calculate buffer size.
                int bufferSize = (m_pActiveCodec.AudioFormat.SamplesPerSecond / (1000 / m_AudioFrameSize)) * (m_pActiveCodec.AudioFormat.BitsPerSample / 8);

                m_pWaveIn = new _WaveIn(m_pAudioInDevice,m_pActiveCodec.AudioFormat.SamplesPerSecond,m_pActiveCodec.AudioFormat.BitsPerSample,1,bufferSize);
                m_pWaveIn.AudioFrameReceived += new EventHandler<EventArgs<byte[]>>(m_pWaveIn_AudioFrameReceived);
                m_pWaveIn.Start();
            }
        }

        /// <summary>
        /// Stops capturing from audio-in device and sending it to RTP stream.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this method is accessed.</exception>
        public void Stop()
        {
            if(IsDisposed){
                throw new ObjectDisposedException(GetType().Name);
            }
            if(!m_IsRunning){
                return;
            }

            if(m_pWaveIn != null){
                m_pWaveIn.Dispose();
                m_pWaveIn = null;
            }
        }

        /// <summary>
        /// Gets if this object is disposed.
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// Gets if currently audio is sent.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this property is accessed.</exception>
        public bool IsRunning
        {
            get{ 
                if(IsDisposed){
                    throw new ObjectDisposedException(GetType().Name);
                }
                
                return m_IsRunning; 
            }
        }

        /// <summary>
        /// Gets audio-in device is used to capture sound.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this property is accessed.</exception>
        /// <exception cref="ArgumentNullException">Is raised when null reference is passed.</exception>
        public AudioInDevice AudioInDevice
        {
            get{   
                if(IsDisposed){
                    throw new ObjectDisposedException(GetType().Name);
                }
                
                return m_pAudioInDevice; 
            }

            set{
                if(IsDisposed){
                    throw new ObjectDisposedException(GetType().Name);
                }

                m_pAudioInDevice = value ?? throw new ArgumentNullException("AudioInDevice");

                if(IsRunning){
                    Stop();
                    Start();
                }
            }
        }

        // TODO:
        // public int Volume ?

        /// <summary>
        /// Gets RTP stream used for audio sending.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this property is accessed.</exception>
        public RTP_SendStream RTP_Stream
        {
            get{  
                if(IsDisposed){
                    throw new ObjectDisposedException(GetType().Name);
                }
                
                return m_pRTP_Stream; 
            }
        }

        /// <summary>
        /// Gets current audio codec what is used for sending.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this property is accessed.</exception>
        public AudioCodec AudioCodec
        {
            get{  
                if(IsDisposed){
                    throw new ObjectDisposedException(GetType().Name);
                }
                
                return m_pActiveCodec; 
            }
        }

        /// <summary>
        /// Gets or sets audio codecs.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this property is accessed.</exception>
        /// <exception cref="ArgumentNullException">Is raised when null reference passed.</exception>
        public Dictionary<int,AudioCodec> AudioCodecs
        {
            get{ 
                if(IsDisposed){
                    throw new ObjectDisposedException(GetType().Name);
                }

                return m_pAudioCodecs; 
            }

            set{
                if(IsDisposed){
                    throw new ObjectDisposedException(GetType().Name);
                }

                m_pAudioCodecs = value ?? throw new ArgumentNullException("AudioCodecs");
            }
        }

        /// <summary>
        /// This method is raised when asynchronous thread Exception happens.
        /// </summary>
        public event EventHandler<ExceptionEventArgs> Error;

        /// <summary>
        /// Raises <b>Error</b> event.
        /// </summary>
        /// <param name="x">Error what happened.</param>
        private void OnError(Exception x)
        {
            if(Error != null){
                Error(this,new ExceptionEventArgs(x));
            }
        }
    }
}
