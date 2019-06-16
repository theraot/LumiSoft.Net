using System;

namespace LumiSoft.Net.POP3.Server
{
    /// <summary>
    /// This class provides data for <see cref="POP3_Session.DeleteMessage"/> event.
    /// </summary>
    public class POP3_e_DeleteMessage : EventArgs
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="message">Message to delete.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>message</b> is null reference.</exception>
        internal POP3_e_DeleteMessage(POP3_ServerMessage message)
        {
            Message = message ?? throw new ArgumentNullException("message");
        }

        /// <summary>
        /// Gets message info.
        /// </summary>
        public POP3_ServerMessage Message { get; }
    }
}
