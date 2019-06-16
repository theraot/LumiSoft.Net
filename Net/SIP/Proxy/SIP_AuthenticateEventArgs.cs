
using LumiSoft.Net.AUTH;

namespace LumiSoft.Net.SIP.Proxy
{
    /// <summary>
    /// This class provides data for SIP_ProxyCore.Authenticate event.
    /// </summary>
    public class SIP_AuthenticateEventArgs
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="auth">Authentication context.</param>
        public SIP_AuthenticateEventArgs(Auth_HttpDigest auth)
        {
            AuthContext = auth;
        }

        /// <summary>
        /// Gets authentication context.
        /// </summary>
        public Auth_HttpDigest AuthContext { get; }

        /// <summary>
        /// Gets or sets if specified request is authenticated.
        /// </summary>
        public bool Authenticated { get; set; }
    }
}
