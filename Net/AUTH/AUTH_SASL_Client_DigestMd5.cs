using System;
using System.Text;

namespace LumiSoft.Net.AUTH
{
    /// <summary>
    /// Implements "DIGEST-MD5" authenticaiton.
    /// </summary>
    public class AUTH_SASL_Client_DigestMd5 : AUTH_SASL_Client
    {
        private bool _isCompleted;
        private readonly string _password;
        private AUTH_SASL_DigestMD5_Response _response;
        private readonly string _protocol;
        private readonly string _serverName;
        private int _state;
        private readonly string _userName;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="protocol">Protocol name. For example: SMTP.</param>
        /// <param name="server">Remote server name or IP address.</param>
        /// <param name="userName">User login name.</param>
        /// <param name="password">User password.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>protocol</b>,<b>server</b>,<b>userName</b> or <b>password</b> is null reference.</exception>
        /// <exception cref="ArgumentException">Is raised when any of the arguments has invalid value.</exception>
        public AUTH_SASL_Client_DigestMd5(string protocol, string server, string userName, string password)
        {
            if (protocol == null)
            {
                throw new ArgumentNullException("protocol");
            }
            if (protocol == string.Empty)
            {
                throw new ArgumentException("Argument 'protocol' value must be specified.", "userName");
            }
            if (server == null)
            {
                throw new ArgumentNullException("protocol");
            }
            if (server == string.Empty)
            {
                throw new ArgumentException("Argument 'server' value must be specified.", "userName");
            }
            if (userName == null)
            {
                throw new ArgumentNullException("userName");
            }
            if (userName == string.Empty)
            {
                throw new ArgumentException("Argument 'username' value must be specified.", "userName");
            }

            _protocol = protocol;
            _serverName = server;
            _userName = userName;
            _password = password ?? throw new ArgumentNullException("password");
        }

        /// <summary>
        /// Gets if the authentication exchange has completed.
        /// </summary>
        public override bool IsCompleted => _isCompleted;

        /// <summary>
        /// Returns always "DIGEST-MD5".
        /// </summary>
        public override string Name => "DIGEST-MD5";

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

            /* RFC 2831.
                The base64-decoded version of the SASL exchange is:

                S: realm="elwood.innosoft.com",nonce="OA6MG9tEQGm2hh",qop="auth",
                   algorithm=md5-sess,charset=utf-8
                C: charset=utf-8,username="chris",realm="elwood.innosoft.com",
                   nonce="OA6MG9tEQGm2hh",nc=00000001,cnonce="OA6MHXh6VqTrRk",
                   digest-uri="imap/elwood.innosoft.com",
                   response=d388dad90d4bbd760a152321f2143af7,qop=auth
                S: rspauth=ea40f60335c427b5527b84dbabcdfffd
                C: 
                S: ok

                The password in this example was "secret".
            */

            if (_state == 0)
            {
                _state++;

                // Parse server challenge.
                var challenge = AUTH_SASL_DigestMD5_Challenge.Parse(Encoding.UTF8.GetString(serverResponse));

                // Construct our response to server challenge.
                _response = new AUTH_SASL_DigestMD5_Response(
                    challenge,
                    challenge.Realm[0],
                    _userName,
                    _password,
                    Guid.NewGuid().ToString().Replace("-", ""),
                    1,
                    challenge.QopOptions[0],
                    _protocol + "/" + _serverName
                );

                return Encoding.UTF8.GetBytes(_response.ToResponse());
            }

            if (_state == 1)
            {
                _state++;
                _isCompleted = true;

                // Check rspauth value.
                if (!string.Equals(Encoding.UTF8.GetString(serverResponse), _response.ToRspauthResponse(_userName, _password), StringComparison.InvariantCultureIgnoreCase))
                {
                    throw new Exception("Server server 'rspauth' value mismatch with local 'rspauth' value.");
                }

                return new byte[0];
            }
            throw new InvalidOperationException("Authentication is completed.");
        }
    }
}
