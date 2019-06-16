using System;

namespace LumiSoft.Net.RTP
{
    /// <summary>
    /// This class holds sender report info.
    /// </summary>
    public class RTCP_Report_Sender
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="sr">RTCP SR report.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>sr</b> is null reference.</exception>
        internal RTCP_Report_Sender(RTCP_Packet_SR sr)
        {
            if(sr == null){
                throw new ArgumentNullException("sr");
            }

            NtpTimestamp      = sr.NtpTimestamp;
            RtpTimestamp      = sr.RtpTimestamp;
            SenderPacketCount = sr.SenderPacketCount;
            SenderOctetCount  = sr.SenderOctetCount;
        }

        /// <summary>
        /// Gets the wallclock time (see Section 4) when this report was sent.
        /// </summary>
        public ulong NtpTimestamp { get; }

        /// <summary>
        /// Gets RTP timestamp.
        /// </summary>
        public uint RtpTimestamp { get; }

        /// <summary>
        /// Gets how many packets sender has sent.
        /// </summary>
        public uint SenderPacketCount { get; }

        /// <summary>
        /// Gets how many bytes sender has sent.
        /// </summary>
        public uint SenderOctetCount { get; }
    }
}
