using System;

namespace LumiSoft.Net.RTP
{
    /// <summary>
    /// This class holds receiver report info.
    /// </summary>
    public class RTCP_Report_Receiver
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="rr">RTCP RR report.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>rr</b> is null reference.</exception>
        internal RTCP_Report_Receiver(RTCP_Packet_ReportBlock rr)
        {
            if(rr == null){
                throw new ArgumentNullException("rr");
            }

            FractionLost          = rr.FractionLost;
            CumulativePacketsLost = rr.CumulativePacketsLost;
            ExtendedSequenceNumber    = rr.ExtendedHighestSeqNo;
            Jitter                = rr.Jitter;
            LastSR                = rr.LastSR;
            DelaySinceLastSR      = rr.DelaySinceLastSR;
        }

        /// <summary>
        /// Gets the fraction of RTP data packets from source SSRC lost since the previous SR or 
        /// RR packet was sent.
        /// </summary>
        public uint FractionLost { get; }

        /// <summary>
        /// Gets total number of RTP data packets from source SSRC that have
        /// been lost since the beginning of reception.
        /// </summary>
        public uint CumulativePacketsLost { get; }

        /// <summary>
        /// Gets extended highest sequence number received.
        /// </summary>
        public uint ExtendedSequenceNumber { get; }

        /// <summary>
        /// Gets an estimate of the statistical variance of the RTP data packet
        /// interarrival time, measured in timestamp units and expressed as an
        /// unsigned integer.
        /// </summary>
        public uint Jitter { get; }

        /// <summary>
        /// Gets when last sender report(SR) was recieved.
        /// </summary>
        public uint LastSR { get; }

        /// <summary>
        /// Gets delay since last sender report(SR) was received.
        /// </summary>
        public uint DelaySinceLastSR { get; }
    }
}
