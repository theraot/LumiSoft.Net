using System;

namespace LumiSoft.Net.RTP
{
    /// <summary>
    /// This method provides data for RTP receive stream related events and methods.
    /// </summary>
    public class RTP_ReceiveStreamEventArgs : EventArgs
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="stream">RTP stream.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>stream</b> is null reference.</exception>
        public RTP_ReceiveStreamEventArgs(RTP_ReceiveStream stream)
        {
            Stream = stream ?? throw new ArgumentNullException("stream");
        }

        /// <summary>
        /// Gets RTP stream.
        /// </summary>
        public RTP_ReceiveStream Stream { get; }
    }
}
