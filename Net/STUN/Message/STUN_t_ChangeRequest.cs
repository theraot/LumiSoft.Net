namespace LumiSoft.Net.STUN.Message
{
    /// <summary>
    /// This class implements STUN CHANGE-REQUEST attribute. Defined in RFC 3489 11.2.4.
    /// </summary>
    public class STUN_t_ChangeRequest
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public STUN_t_ChangeRequest()
        {
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="changeIP">Specifies if STUN server must send response to different IP than request was received.</param>
        /// <param name="changePort">Specifies if STUN server must send response to different port than request was received.</param>
        public STUN_t_ChangeRequest(bool changeIP,bool changePort)
        {
            ChangeIP = changeIP;
            ChangePort = changePort;
        }

        /// <summary>
        /// Gets or sets if STUN server must send response to different IP than request was received.
        /// </summary>
        public bool ChangeIP { get; set; } = true;

        /// <summary>
        /// Gets or sets if STUN server must send response to different port than request was received.
        /// </summary>
        public bool ChangePort { get; set; } = true;
    }
}
