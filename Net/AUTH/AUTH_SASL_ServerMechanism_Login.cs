using System;
using System.Text;

namespace LumiSoft.Net.AUTH
{
    /// <summary>
    /// Implements "LOGIN" authenticaiton.
    /// </summary>
    public class AUTH_SASL_ServerMechanism_Login : AUTH_SASL_ServerMechanism
    {
        private bool   m_IsCompleted;
        private bool   m_IsAuthenticated;
        private string m_UserName;
        private string m_Password;
        private int    m_State;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="requireSSL">Specifies if this mechanism is available to SSL connections only.</param>
        public AUTH_SASL_ServerMechanism_Login(bool requireSSL)
        {
            RequireSSL = requireSSL;
        }

        /// <summary>
        /// Resets any authentication state data.
        /// </summary>
        public override void Reset()
        {
            m_IsCompleted     = false;
            m_IsAuthenticated = false;
            m_UserName        = null;
            m_Password        = null;
            m_State           = 0;
        }

        /// <summary>
        /// Continues authentication process.
        /// </summary>
        /// <param name="clientResponse">Client sent SASL response.</param>
        /// <returns>Retunrns challange response what must be sent to client or null if authentication has completed.</returns>
        /// <exception cref="ArgumentNullException">Is raised when <b>clientResponse</b> is null reference.</exception>
        public override byte[] Continue(byte[] clientResponse)
        {
            if(clientResponse == null){
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
            if(m_State == 0 && clientResponse.Length > 0){
                m_State++;
            }

            if(m_State == 0){
                m_State++;

                return Encoding.ASCII.GetBytes("UserName:");
            }

            if(m_State == 1){
                m_State++;
                m_UserName = Encoding.UTF8.GetString(clientResponse);
            
                return Encoding.ASCII.GetBytes("Password:");
            }
            m_Password = Encoding.UTF8.GetString(clientResponse);

            var result = OnAuthenticate("",m_UserName,m_Password);
            m_IsAuthenticated = result.IsAuthenticated;
            m_IsCompleted = true;

            return null;
        }

        /// <summary>
        /// Gets if the authentication exchange has completed.
        /// </summary>
        public override bool IsCompleted
        {
            get{ return m_IsCompleted; }
        }

        /// <summary>
        /// Gets if user has authenticated sucessfully.
        /// </summary>
        public override bool IsAuthenticated
        {
            get{ return m_IsAuthenticated; }
        }

        /// <summary>
        /// Returns always "LOGIN".
        /// </summary>
        public override string Name
        {
            get { return "LOGIN"; }
        }

        /// <summary>
        /// Gets if specified SASL mechanism is available only to SSL connection.
        /// </summary>
        public override bool RequireSSL { get; }

        /// <summary>
        /// Gets user login name.
        /// </summary>
        public override string UserName
        {
            get{ return m_UserName; }
        }

        /// <summary>
        /// Is called when authentication mechanism needs to authenticate specified user.
        /// </summary>
        public event EventHandler<AUTH_e_Authenticate> Authenticate;

        /// <summary>
        /// Raises <b>Authenticate</b> event.
        /// </summary>
        /// <param name="authorizationID">Authorization ID.</param>
        /// <param name="userName">User name.</param>
        /// <param name="password">Password.</param>
        /// <returns>Returns authentication result.</returns>
        private AUTH_e_Authenticate OnAuthenticate(string authorizationID,string userName,string password)
        {
            var retVal = new AUTH_e_Authenticate(authorizationID,userName,password);

            if (Authenticate != null){
                Authenticate(this,retVal);
            }

            return retVal;
        }
    }
}
