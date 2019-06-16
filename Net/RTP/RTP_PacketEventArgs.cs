using System;

namespace LumiSoft.Net.RTP
{
    /// <summary>
    /// This class provides data for RTP packet related events/methods.
    /// </summary>
    public class RTP_PacketEventArgs : EventArgs
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="packet">RTP packet.</param>
        public RTP_PacketEventArgs(RTP_Packet packet)
        {
            if(packet == null){
                throw new ArgumentNullException("packet");
            }

            Packet = packet;
        }


        #region Properties implementation

        /// <summary>
        /// Gets RTP packet.
        /// </summary>
        public RTP_Packet Packet { get; }

#endregion

    }
}
