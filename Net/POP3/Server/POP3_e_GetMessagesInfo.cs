using System;
using System.Collections.Generic;

namespace LumiSoft.Net.POP3.Server
{
    /// <summary>
    /// This class provides data for <see cref="POP3_Session.GetMessagesInfo"/> event.
    /// </summary>
    public class POP3_e_GetMessagesInfo : EventArgs
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        internal POP3_e_GetMessagesInfo()
        {
            Messages = new List<POP3_ServerMessage>();
        }


        /// <summary>
        /// Gets POP3 messages info collection.
        /// </summary>
        public List<POP3_ServerMessage> Messages { get; }
    }
}
