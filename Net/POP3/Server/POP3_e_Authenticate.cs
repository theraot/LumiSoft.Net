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
        internal POP3_e_Authenticate(string user,string password)
        {
            if(user == null){
                throw new ArgumentNullException("user");
            }
            if(password == null){
                throw new ArgumentNullException("password");
            }

            User     = user;
            Password = password;
        }


        #region Properties implementation

        /// <summary>
        /// Gets or sets if session is authenticated.
        /// </summary>
        public bool IsAuthenticated { get; set; }

        /// <summary>
        /// Gets user name.
        /// </summary>
        public string User { get; } = "";

        /// <summary>
        /// Gets password.
        /// </summary>
        public string Password { get; } = "";

#endregion
    }
}
