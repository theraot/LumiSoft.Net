namespace LumiSoft.Net.STUN.Message
{
    /// <summary>
    /// This class implements STUN ERROR-CODE. Defined in RFC 3489 11.2.9.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public class STUN_t_ErrorCode
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="code">Error code.</param>
        /// <param name="reasonText">Reason text.</param>
        public STUN_t_ErrorCode(int code,string reasonText)
        {
            Code = code;
            ReasonText = reasonText;
        }

        /// <summary>
        /// Gets or sets error code.
        /// </summary>
        public int Code { get; }

        /// <summary>
        /// Gets reason text.
        /// </summary>
        public string ReasonText { get; }
    }
}