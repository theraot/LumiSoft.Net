using System;

namespace LumiSoft.Net.POP3.Server
{
    /// <summary>
    /// This class provides data for <see cref="POP3_Session.GetTopOfMessage"/> event.
    /// </summary>
    public class POP3_e_GetTopOfMessage : EventArgs
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="message">Message which top data to get.</param>
        /// <param name="lines">Number of message-body lines to get.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>message</b> is null reference.</exception>
        /// <exception cref="ArgumentException">Is raised when any of the arguments has invalid value.</exception>
        internal POP3_e_GetTopOfMessage(POP3_ServerMessage message,int lines)
        {
            if(message == null){
                throw new ArgumentNullException("message");
            }
            if(lines < 0){
                throw new ArgumentException("Argument 'lines' value must be >= 0.","lines");
            }

            Message  = message;
            LineCount = lines;
        }

        /// <summary>
        /// Gets message info.
        /// </summary>
        public POP3_ServerMessage Message { get; }

        /// <summary>
        /// Gets number message body lines should be included.
        /// </summary>
        public int LineCount { get; }

        /// <summary>
        /// Gets or sets top of message data.
        /// </summary>
        /// <remarks>This value should contain message header + number of <b>lineCount</b> body lines.</remarks>
        public byte[] Data { get; set; }
    }
}
