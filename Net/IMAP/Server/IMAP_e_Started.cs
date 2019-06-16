using System;

namespace LumiSoft.Net.IMAP.Server
{
    /// <summary>
    /// This class provides data for <b cref="IMAP_Session.Started">IMAP_Session.Started</b> event.
    /// </summary>
    public class IMAP_e_Started : EventArgs
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="response">IMAP server response.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>response</b> is null reference.</exception>
        internal IMAP_e_Started(IMAP_r_u_ServerStatus response)
        {
            Response = response ?? throw new ArgumentNullException("response");
        }

        /// <summary>
        /// Gets or sets IMAP server response.
        /// </summary>
        /// <remarks>Response should be OK,NO with human readable text."</remarks>
        public IMAP_r_u_ServerStatus Response { get; set; }
    }
}
