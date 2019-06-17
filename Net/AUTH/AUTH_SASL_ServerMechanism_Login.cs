using System;
using System.Text;

namespace LumiSoft.Net.AUTH
{
    /// <summary>
    /// Implements "LOGIN" authenticaiton.
    /// </summary>
    public class AUTH_SASL_ServerMechanism_Login : AUTH_SASL_ServerMechanism
    {
        private bool _isAuthenticated;
        private bool _isCompleted;
        private string _password;
        private int _state;
        private string _userName;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="requireSSL">Specifies if this mechanism is available to SSL connections only.</param>
        public AUTH_SASL_ServerMechanism_Login(bool requireSSL)
        {
            RequireSSL = requireSSL;
        }

        /// <summary>
        /// Is called when authentication mechanism needs to authenticate specified user.
        /// </summary>
        public event EventHandler<AUTH_e_Authenticate> Authenticate;

        /// <summary>
        /// Gets if user has authenticated sucessfully.
        /// </summary>
        public override bool IsAuthenticated => _isAuthenticated;

        /// <summary>
        /// Gets if the authentication exchange has completed.
        /// </summary>
        public override bool IsCompleted => _isCompleted;

        /// <summary>
        /// Returns always "LOGIN".
        /// </summary>
        public override string Name => "LOGIN";

        /// <summary>
        /// Gets if specified SASL mechanism is available only to SSL connection.
        /// </summary>
        public override bool RequireSSL { get; }

        /// <summary>
        /// Gets user login name.
        /// </summary>
        public override string UserName => _userName;

        /// <summary>
        /// Continues authentication process.
        /// </summary>
        /// <param name="clientResponse">Client sent SASL response.</param>
        /// <returns>Retunrns challange response what must be sent to client or null if authentication has completed.</returns>
        /// <exception cref="ArgumentNullException">Is raised when <b>clientResponse</b> is null reference.</exception>
        public override byte[] Continue(byte[] clientResponse)
        {
            if (clientResponse == null)
            {
                throw new ArgumentNullException("clientResponse");
            }

            /* RFC none.
                S: "Username:"
                C: userName
                S: "Password:"
                C: password
             
                NOTE: UserName may be included in initial client response.
            */

            // User name provided, so skip that state.
            if (_state == 0 && clientResponse.Length > 0)
            {
                _state++;
            }

            if (_state == 0)
            {
                _state++;

                return Encoding.ASCII.GetBytes("UserName:");
            }

            if (_state == 1)
            {
                _state++;
                _userName = Encoding.UTF8.GetString(clientResponse);

                return Encoding.ASCII.GetBytes("Password:");
            }
            _password = Encoding.UTF8.GetString(clientResponse);

            var result = OnAuthenticate("", _userName, _password);
            _isAuthenticated = result.IsAuthenticated;
            _isCompleted = true;

            return null;
        }

        /// <summary>
        /// Resets any authentication state data.
        /// </summary>
        public override void Reset()
        {
            _isCompleted = false;
            _isAuthenticated = false;
            _userName = null;
            _password = null;
            _state = 0;
        }

        /// <summary>
        /// Raises <b>Authenticate</b> event.
        /// </summary>
        /// <param name="authorizationID">Authorization ID.</param>
        /// <param name="userName">User name.</param>
        /// <param name="password">Password.</param>
        /// <returns>Returns authentication result.</returns>
        private AUTH_e_Authenticate OnAuthenticate(string authorizationID, string userName, string password)
        {
            var retVal = new AUTH_e_Authenticate(authorizationID, userName, password);

            Authenticate?.Invoke(this, retVal);

            return retVal;
        }
    }
}
