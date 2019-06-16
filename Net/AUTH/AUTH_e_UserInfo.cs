using System;

namespace LumiSoft.Net.AUTH
{
    /// <summary>
    /// This class provides data for server authentication mechanisms <b>GetUserInfo</b> event.
    /// </summary>
    public class AUTH_e_UserInfo : EventArgs
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="userName">User name.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>userName</b> is null reference.</exception>
        /// <exception cref="ArgumentException">Is raised when any of the arguments has invalid value.</exception>
        public AUTH_e_UserInfo(string userName)
        {
            if(userName == null){
                throw new ArgumentNullException("userName");
            }
            if(userName == string.Empty){
                throw new ArgumentException("Argument 'userName' value must be specified.","userName");
            }

            UserName = userName;
        }

        /// <summary>
        /// Gets or sets if specified user exists.
        /// </summary>
        public bool UserExists { get; set; }

        /// <summary>
        /// Gets user name.
        /// </summary>
        public string UserName { get; } = "";

        /// <summary>
        /// Gets or sets user password.
        /// </summary>
        public string Password { get; set; } = "";
    }
}
