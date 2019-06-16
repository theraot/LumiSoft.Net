using System;

namespace LumiSoft.Net.POP3.Server
{
    /// <summary>
    /// This class provides data for <see cref="POP3_Session.Authenticate"/> event.
    /// </summary>
    public class POP3_e_Authenticate : EventArgs
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="user">User name.</param>
        /// <param name="password">Password.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>user</b> or <b>password</b> is null reference.</exception>
        internal POP3_e_Authenticate(string user, string password)
        {
            User = user ?? throw new ArgumentNullException("user");
            Password = password ?? throw new ArgumentNullException("password");
        }

        /// <summary>
        /// Gets or sets if session is authenticated.
        /// </summary>
        public bool IsAuthenticated { get; set; }

        /// <summary>
        /// Gets password.
        /// </summary>
        public string Password { get; } = "";

        /// <summary>
        /// Gets user name.
        /// </summary>
        public string User { get; } = "";
    }
}
