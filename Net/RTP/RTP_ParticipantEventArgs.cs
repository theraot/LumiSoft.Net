using System;

namespace LumiSoft.Net.RTP
{
    /// <summary>
    /// This claass provides data for <b>RTP_MultimediaSession.NewParticipant</b> event.
    /// </summary>
    public class RTP_ParticipantEventArgs : EventArgs
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="participant">RTP participant.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>participant</b> is null reference.</exception>
        public RTP_ParticipantEventArgs(RTP_Participant_Remote participant)
        {
            if(participant == null){
                throw new ArgumentNullException("participant");
            }

            Participant = participant;
        }


        #region Properties implementation

        /// <summary>
        /// Gets participant.
        /// </summary>
        public RTP_Participant_Remote Participant { get; }

#endregion

    }
}
