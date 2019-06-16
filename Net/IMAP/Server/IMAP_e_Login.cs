using System;

namespace LumiSoft.Net.IMAP.Server
{
    /// <summary>
    /// This class provides data for <b cref="IMAP_Session.Started">IMAP_Session.Login</b> event.
    /// </summary>
    public class IMAP_e_Login : EventArgs
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="user">User name.</param>
        /// <param name="password">Password.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>user</b> or <b>password</b> is null reference.</exception>
        internal IMAP_e_Login(string user, string password)
        {
            UserName = user ?? throw new ArgumentNullException("user");
            Password = password ?? throw new ArgumentNullException("password");
        }

        /// <summary>
        /// Gets or sets if specified user is authenticated.
        /// </summary>
        public bool IsAuthenticated { get; set; }

        /// <summary>
        /// Gets user password.
        /// </summary>
        public string Password { get; } = "";

        /// <summary>
        /// Gets user name.
        /// </summary>
        public string UserName { get; } = "";
    }
}
