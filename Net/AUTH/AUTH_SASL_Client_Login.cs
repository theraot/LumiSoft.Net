using System;
using System.Text;

namespace LumiSoft.Net.AUTH
{
    /// <summary>
    /// Implements "LOGIN" authenticaiton.
    /// </summary>
    public class AUTH_SASL_Client_Login : AUTH_SASL_Client
    {
        private bool _isCompleted;
        private readonly string _password;
        private int _state;
        private readonly string _userName;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="userName">User login name.</param>
        /// <param name="password">User password.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>userName</b> or <b>password</b> is null reference.</exception>
        /// <exception cref="ArgumentException">Is raised when any of the arguments has invalid value.</exception>
        public AUTH_SASL_Client_Login(string userName, string password)
        {
            if (userName == null)
            {
                throw new ArgumentNullException("userName");
            }
            if (userName == string.Empty)
            {
                throw new ArgumentException("Argument 'username' value must be specified.", "userName");
            }

            _userName = userName;
            _password = password ?? throw new ArgumentNullException("password");
        }

        /// <summary>
        /// Gets if the authentication exchange has completed.
        /// </summary>
        public override bool IsCompleted => _isCompleted;

        /// <summary>
        /// Returns always "LOGIN".
        /// </summary>
        public override string Name => "LOGIN";

        /// <summary>
        /// Gets user login name.
        /// </summary>
        public override string UserName => _userName;

        /// <summary>
        /// Continues authentication process.
        /// </summary>
        /// <param name="serverResponse">Server sent SASL response.</param>
        /// <returns>Returns challange request what must be sent to server or null if authentication has completed.</returns>
        /// <exception cref="ArgumentNullException">Is raised when <b>serverResponse</b> is null reference.</exception>
        /// <exception cref="InvalidOperationException">Is raised when this method is called when authentication is completed.</exception>
        public override byte[] Continue(byte[] serverResponse)
        {
            if (serverResponse == null)
            {
                throw new ArgumentNullException("serverResponse");
            }
            if (_isCompleted)
            {
                throw new InvalidOperationException("Authentication is completed.");
            }

            /* RFC none.
                S: "Username:"
                C: userName
                S: "Password:"
                C: password
             
                NOTE: UserName may be included in initial client response.
            */

            if (_state == 0)
            {
                _state++;

                return Encoding.UTF8.GetBytes(_userName);
            }

            if (_state == 1)
            {
                _state++;
                _isCompleted = true;

                return Encoding.UTF8.GetBytes(_password);
            }
            throw new InvalidOperationException("Authentication is completed.");
        }
    }
}
