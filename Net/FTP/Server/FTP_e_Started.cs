using System;

namespace LumiSoft.Net.FTP.Server
{
    /// <summary>
    /// This class provides data for <b cref="FTP_Session.Started">FTP_Session.Started</b> event.
    /// </summary>
    public class FTP_e_Started : EventArgs
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="response">POP3 server response.</param>
        internal FTP_e_Started(string response)
        {
            Response = response;
        }


        /// <summary>
        /// Gets or sets FTP server response.
        /// </summary>
        /// <remarks>Response also MUST contain response code(220 / 500). For example: "500 Session rejected."</remarks>
        public string Response { get; set; }
    }
}
