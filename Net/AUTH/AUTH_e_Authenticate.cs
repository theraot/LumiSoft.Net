﻿using System;

namespace LumiSoft.Net.AUTH
{
    /// <summary>
    /// This class provides data for server userName/password authentications.
    /// </summary>
    public class AUTH_e_Authenticate : EventArgs
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="authorizationID">Authorization ID.</param>
        /// <param name="userName">User name.</param>
        /// <param name="password">Password.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>userName</b> is null reference.</exception>
        /// <exception cref="ArgumentException">Is raised when any of the argumnets has invalid value.</exception>
        public AUTH_e_Authenticate(string authorizationID,string userName,string password)
        {
            if(userName == null){
                throw new ArgumentNullException("userName");
            }
            if(userName == string.Empty){
                throw new ArgumentException("Argument 'userName' value must be specified.","userName");
            }

            AuthorizationID = authorizationID;
            UserName        = userName;
            Password        = password;
        }


        /// <summary>
        /// Gets or sets if specified user is authenticated.
        /// </summary>
        public bool IsAuthenticated { get; set; }

        /// <summary>
        /// Gets authorization ID.
        /// </summary>
        public string AuthorizationID { get; } = "";

        /// <summary>
        /// Gets user name.
        /// </summary>
        public string UserName { get; } = "";

        /// <summary>
        /// Gets password.
        /// </summary>
        public string Password { get; } = "";
    }
}
