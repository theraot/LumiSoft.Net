using System;

namespace LumiSoft.Net.POP3.Server
{
    /// <summary>
    /// This class provides data for <b cref="POP3_Session.Started">POP3_Session.Started</b> event.
    /// </summary>
    public class POP3_e_Started : EventArgs
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="response">POP3 server response.</param>
        internal POP3_e_Started(string response)
        {
            Response = response;
        }

        /// <summary>
        /// Gets or sets POP3 server response.
        /// </summary>
        /// <remarks>Response also MUST contain response code(+OK / -ERR). For example: "-ERR Session rejected."</remarks>
        public string Response { get; set; }
    }
}
