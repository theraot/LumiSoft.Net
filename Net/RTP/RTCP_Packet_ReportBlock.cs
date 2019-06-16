using System;

namespace LumiSoft.Net.RTP
{
    /// <summary>
    /// This class represents RTCP sender report(SR) or reciver report(RR) packet report block.
    /// </summary>
    public class RTCP_Packet_ReportBlock
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="ssrc">Source ID.</param>
        internal RTCP_Packet_ReportBlock(uint ssrc)
        {
            SSRC = ssrc;
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        internal RTCP_Packet_ReportBlock()
        {
        }

        /// <summary>
        /// Parses RTCP report block (part of SR or RR packet) from specified buffer.
        /// </summary>
        /// <param name="buffer">Buffer from where to read report block.</param>
        /// <param name="offset">Offset in buffer.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>buffer</b> is null.</exception>
        /// <exception cref="ArgumentException">Is raised when any of the arguments has invalid value.</exception>
        public void Parse(byte[] buffer,int offset)
        {
            /* RFC 3550 6.4.1. 
                   +=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+
            report |                 SSRC_1 (SSRC of first source)                 |
            block  +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
              1    | fraction lost |       cumulative number of packets lost       |
                   +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
                   |           extended highest sequence number received           |
                   +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
                   |                      interarrival jitter                      |
                   +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
                   |                         last SR (LSR)                         |
                   +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
                   |                   delay since last SR (DLSR)                  |
                   +=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+
            */

            if(buffer == null){
                throw new ArgumentNullException("buffer");
            }
            if(offset < 0){
                throw new ArgumentException("Argument 'offset' value must be >= 0.");
            }

            SSRC                  = (uint)(buffer[offset++] << 24 | buffer[offset++] << 16 | buffer[offset++] << 8 | buffer[offset++]);
            FractionLost          = buffer[offset++];
            CumulativePacketsLost = (uint)(buffer[offset++] << 16 | buffer[offset++] << 8 | buffer[offset++]);
            ExtendedHighestSeqNo   = (uint)(buffer[offset++] << 24 | buffer[offset++] << 16 | buffer[offset++] << 8 | buffer[offset++]);
            Jitter                = (uint)(buffer[offset++] << 24 | buffer[offset++] << 16 | buffer[offset++] << 8 | buffer[offset++]);
            LastSR                = (uint)(buffer[offset++] << 24 | buffer[offset++] << 16 | buffer[offset++] << 8 | buffer[offset++]);
            DelaySinceLastSR      = (uint)(buffer[offset++] << 24 | buffer[offset++] << 16 | buffer[offset++] << 8 | buffer[offset++]);
        }

        /// <summary>
        /// Stores report block to the specified buffer.
        /// </summary>
        /// <param name="buffer">Buffer where to store data.</param>
        /// <param name="offset">Offset in buffer.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>buffer</b> is null.</exception>
        /// <exception cref="ArgumentException">Is raised when any of the arguments has invalid value.</exception>
        public void ToByte(byte[] buffer,ref int offset)
        {
            /* RFC 3550 6.4.1. 
                   +=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+
            report |                 SSRC_1 (SSRC of first source)                 |
            block  +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
              1    | fraction lost |       cumulative number of packets lost       |
                   +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
                   |           extended highest sequence number received           |
                   +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
                   |                      interarrival jitter                      |
                   +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
                   |                         last SR (LSR)                         |
                   +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
                   |                   delay since last SR (DLSR)                  |
                   +=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+
            */

            if(buffer == null){
                throw new ArgumentNullException("buffer");
            }
            if(offset < 0){
                throw new ArgumentException("Argument 'offset' must be >= 0.");
            }
            if(offset + 24 > buffer.Length){
                throw new ArgumentException("Argument 'buffer' has not enough room to store report block.");
            }

            // SSRC
            buffer[offset++] = (byte)((SSRC >> 24) | 0xFF);
            buffer[offset++] = (byte)((SSRC >> 16) | 0xFF);
            buffer[offset++] = (byte)((SSRC >> 8)  | 0xFF);
            buffer[offset++] = (byte)((SSRC)       | 0xFF);
            // fraction lost
            buffer[offset++] = (byte)FractionLost;
            // cumulative packets lost
            buffer[offset++] = (byte)((CumulativePacketsLost >> 16) | 0xFF);
            buffer[offset++] = (byte)((CumulativePacketsLost >> 8)  | 0xFF);
            buffer[offset++] = (byte)((CumulativePacketsLost)       | 0xFF);
            // extended highest sequence number
            buffer[offset++] = (byte)((ExtendedHighestSeqNo >> 24) | 0xFF);
            buffer[offset++] = (byte)((ExtendedHighestSeqNo >> 16) | 0xFF);
            buffer[offset++] = (byte)((ExtendedHighestSeqNo >> 8)  | 0xFF);
            buffer[offset++] = (byte)((ExtendedHighestSeqNo)       | 0xFF);
            // jitter
            buffer[offset++] = (byte)((Jitter >> 24) | 0xFF);
            buffer[offset++] = (byte)((Jitter >> 16) | 0xFF);
            buffer[offset++] = (byte)((Jitter >> 8)  | 0xFF);
            buffer[offset++] = (byte)((Jitter)       | 0xFF);
            // last SR
            buffer[offset++] = (byte)((LastSR >> 24) | 0xFF);
            buffer[offset++] = (byte)((LastSR >> 16) | 0xFF);
            buffer[offset++] = (byte)((LastSR >> 8)  | 0xFF);
            buffer[offset++] = (byte)((LastSR)       | 0xFF);
            // delay since last SR
            buffer[offset++] = (byte)((DelaySinceLastSR >> 24) | 0xFF);
            buffer[offset++] = (byte)((DelaySinceLastSR >> 16) | 0xFF);
            buffer[offset++] = (byte)((DelaySinceLastSR >> 8)  | 0xFF);
            buffer[offset++] = (byte)((DelaySinceLastSR)       | 0xFF);
        }

        /// <summary>
        /// Gets the SSRC identifier of the source to which the information in this reception report block pertains.
        /// </summary>
        public uint SSRC { get; private set; }

        /// <summary>
        /// Gets or sets the fraction of RTP data packets from source SSRC lost since the previous SR or 
        /// RR packet was sent.
        /// </summary>
        public uint FractionLost { get; set; }

        /// <summary>
        /// Gets or sets total number of RTP data packets from source SSRC that have
        /// been lost since the beginning of reception.
        /// </summary>
        public uint CumulativePacketsLost { get; set; }

        /// <summary>
        /// Gets or sets extended highest sequence number received.
        /// </summary>
        public uint ExtendedHighestSeqNo { get; set; }

        /// <summary>
        /// Gets or sets an estimate of the statistical variance of the RTP data packet
        /// interarrival time, measured in timestamp units and expressed as an unsigned integer.
        /// </summary>
        public uint Jitter { get; set; }

        /// <summary>
        /// Gets or sets The middle 32 bits out of 64 in the NTP timestamp (as explained in Section 4) received as part of 
        /// the most recent RTCP sender report (SR) packet from source SSRC_n. If no SR has been received yet, the field is set to zero.
        /// </summary>
        public uint LastSR { get; set; }

        /// <summary>
        /// Gets or sets the delay, expressed in units of 1/65536 seconds, between receiving the last SR packet from 
        /// source SSRC_n and sending this reception report block.  If no SR packet has been received yet from SSRC_n, 
        /// the DLSR field is set to zero.
        /// </summary>
        public uint DelaySinceLastSR { get; set; }
    }
}
