using System;
using System.Collections.Generic;

using LumiSoft.Net.RTP;
using LumiSoft.Net.Media.Codec.Audio;

namespace LumiSoft.Net.Media
{
    /// <summary>
    /// This class implements audio-out (eg. speaker,headphones) device RTP audio player.
    /// </summary>
    public class AudioOut_RTP : IDisposable
    {
        private bool m_IsRunning;
        private AudioCodec m_pActiveCodec;
        private Dictionary<int, AudioCodec> m_pAudioCodecs;
        private AudioOut m_pAudioOut;
        private AudioOutDevice m_pAudioOutDevice;
        private RTP_ReceiveStream m_pRTP_Stream;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="audioOutDevice">Audio-out device used to play out RTP audio.</param>
        /// <param name="stream">RTP receive stream which audio to play.</param>
        /// <param name="codecs">Audio codecs with RTP payload number. For example: 0-PCMU,8-PCMA.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>audioOutDevice</b>,<b>stream</b> or <b>codecs</b> is null reference.</exception>
        public AudioOut_RTP(AudioOutDevice audioOutDevice, RTP_ReceiveStream stream, Dictionary<int, AudioCodec> codecs)
        {
            m_pAudioOutDevice = audioOutDevice ?? throw new ArgumentNullException("audioOutDevice");
            m_pRTP_Stream = stream ?? throw new ArgumentNullException("stream");
            m_pAudioCodecs = codecs ?? throw new ArgumentNullException("codecs");
        }

        /// <summary>
        /// This method is raised when asynchronous thread Exception happens.
        /// </summary>
        public event EventHandler<ExceptionEventArgs> Error;

        /// <summary>
        /// Gets active audio codec. This value may be null if yet no data received from RTP.
        /// </summary>
        /// <remarks>Audio codec may change during RTP session, if remote-party(sender) changes it.</remarks>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this property is accessed.</exception>
        public AudioCodec ActiveCodec
        {
            get
            {
                if (IsDisposed)
                {
                    throw new ObjectDisposedException(GetType().Name);
                }

                return m_pActiveCodec;
            }
        }

        /// <summary>
        /// Gets audio-out device is used to play out sound.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this property is accessed.</exception>
        /// <exception cref="ArgumentNullException">Is raised when null reference passed.</exception>
        public AudioOutDevice AudioOutDevice
        {
            get
            {
                if (IsDisposed)
                {
                    throw new ObjectDisposedException(GetType().Name);
                }

                return m_pAudioOutDevice;
            }

            set
            {
                if (IsDisposed)
                {
                    throw new ObjectDisposedException(GetType().Name);
                }

                m_pAudioOutDevice = value ?? throw new ArgumentNullException("AudioOutDevice");

                if (IsRunning)
                {
                    Stop();
                    Start();
                }
            }
        }

        /// <summary>
        /// Audio codecs with RTP payload number. For example: 0-PCMU,8-PCMA.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this property is accessed.</exception>
        public Dictionary<int, AudioCodec> Codecs
        {
            get
            {
                if (IsDisposed)
                {
                    throw new ObjectDisposedException(GetType().Name);
                }

                return m_pAudioCodecs;
            }
        }

        /// <summary>
        /// Gets if this object is disposed.
        /// </summary>
        public bool IsDisposed { get; } = false;

        /// <summary>
        /// Gets if audio player is running.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this property is accessed.</exception>
        public bool IsRunning
        {
            get
            {
                if (IsDisposed)
                {
                    throw new ObjectDisposedException(GetType().Name);
                }

                return m_IsRunning;
            }
        }

        /// <summary>
        /// Cleans up any resource being used.
        /// </summary>
        public void Dispose()
        {
            if (IsDisposed)
            {
                return;
            }

            Stop();

            Error = null;
            m_pAudioOutDevice = null;
            m_pRTP_Stream = null;
            m_pAudioCodecs = null;
            m_pActiveCodec = null;
        }

        /// <summary>
        /// Starts receiving RTP audio and palying it out.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this method is accessed.</exception>
        public void Start()
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
            if (m_IsRunning)
            {
                return;
            }

            m_IsRunning = true;

            m_pRTP_Stream.PacketReceived += new EventHandler<RTP_PacketEventArgs>(m_pRTP_Stream_PacketReceived);
        }

        /// <summary>
        /// Stops receiving RTP audio and palying it out.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this method is accessed.</exception>
        public void Stop()
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
            if (!m_IsRunning)
            {
                return;
            }

            m_IsRunning = false;

            m_pRTP_Stream.PacketReceived -= new EventHandler<RTP_PacketEventArgs>(m_pRTP_Stream_PacketReceived);

            if (m_pAudioOut != null)
            {
                m_pAudioOut.Dispose();
                m_pAudioOut = null;
            }
        }

        /// <summary>
        /// This method is called when new RTP packet received.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">Event data.</param>
        private void m_pRTP_Stream_PacketReceived(object sender, RTP_PacketEventArgs e)
        {
            if (IsDisposed)
            {
                return;
            }

            try
            {
                AudioCodec codec = null;
                if (!m_pAudioCodecs.TryGetValue(e.Packet.PayloadType, out codec))
                {
                    // Unknown codec(payload value), skip it.

                    return;
                }
                m_pActiveCodec = codec;

                // Audio-out not created yet, create it.
                if (m_pAudioOut == null)
                {
                    m_pAudioOut = new AudioOut(m_pAudioOutDevice, codec.AudioFormat);
                }
                // Audio-out audio format not compatible to codec, recreate it.
                else if (!m_pAudioOut.AudioFormat.Equals(codec.AudioFormat))
                {
                    m_pAudioOut.Dispose();
                    m_pAudioOut = new AudioOut(m_pAudioOutDevice, codec.AudioFormat);
                }

                // Decode RTP audio frame and queue it for play out.
                var decodedData = codec.Decode(e.Packet.Data, 0, e.Packet.Data.Length);
                m_pAudioOut.Write(decodedData, 0, decodedData.Length);
            }
            catch (Exception x)
            {
                if (!IsDisposed)
                {
                    // Raise error event(We can't throw expection directly, we are on threadpool thread).
                    OnError(x);
                }
            }
        }

        /// <summary>
        /// Raises <b>Error</b> event.
        /// </summary>
        /// <param name="x">Error what happened.</param>
        private void OnError(Exception x)
        {
            if (Error != null)
            {
                Error(this, new ExceptionEventArgs(x));
            }
        }
    }
}
