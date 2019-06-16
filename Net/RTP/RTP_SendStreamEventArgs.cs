using System;

namespace LumiSoft.Net.RTP
{
    /// <summary>
    /// This method provides data for RTP send stream related events and methods.
    /// </summary>
    public class RTP_SendStreamEventArgs : EventArgs
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="stream">RTP send stream.</param>
        public RTP_SendStreamEventArgs(RTP_SendStream stream)
        {
            Stream = stream ?? throw new ArgumentNullException("stream");
        }

        /// <summary>
        /// Gets RTP stream.
        /// </summary>
        public RTP_SendStream Stream { get; }
    }
}
