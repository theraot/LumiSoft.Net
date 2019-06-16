using System;

namespace LumiSoft.Net.RTP
{
    /// <summary>
    /// This class provides data for RTP source related evetns.
    /// </summary>
    public class RTP_SourceEventArgs : EventArgs
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="source">RTP source.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>source</b> is null reference.</exception>
        public RTP_SourceEventArgs(RTP_Source source)
        {
            if(source == null){
                throw new ArgumentNullException("source");
            }

            Source = source;
        }

        /// <summary>
        /// Gets RTP source.
        /// </summary>
        public RTP_Source Source { get; }
    }
}
