using System;
using System.Text;

namespace LumiSoft.Net.AUTH
{
    /// <summary>
    /// Implements http digest access authentication. Defined in RFC 2617.
    /// </summary>
    public class Auth_HttpDigest
    {
        private string m_Charset = "";
        private string m_Cnonce = "";
        private string m_Method = "";
        private string m_Nonce = "";
        private string m_Password = "";
        private string m_Realm = "";
        private string m_UserName = "";

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="digestResponse">Server/Client returned digest response.</param>
        /// <param name="requestMethod">Request method.</param>
        public Auth_HttpDigest(string digestResponse, string requestMethod)
        {
            m_Method = requestMethod;

            Parse(digestResponse);
        }

        /// <summary>
        /// Client constructor. This is used to build valid Authorization response to server.
        /// </summary>
        /// <param name="userName">User name.</param>
        /// <param name="password">Password.</param>
        /// <param name="cnonce">Client nonce value.</param>
        /// <param name="uri">Request URI.</param>
        /// <param name="digestResponse">Server authenticate resposne.</param>
        /// <param name="requestMethod">Request method.</param>
        public Auth_HttpDigest(string userName, string password, string cnonce, string uri, string digestResponse, string requestMethod)
        {
            Parse(digestResponse);

            m_UserName = userName;
            m_Password = password;
            m_Method = requestMethod;
            m_Cnonce = cnonce;
            Uri = uri;
            Qop = "auth";
            NonceCount = 1;
            Response = CalculateResponse(m_UserName, m_Password);
        }

        /// <summary>
        /// Server constructor. This is used to build valid Authenticate response to client.
        /// </summary>
        /// <param name="realm">Realm(domain).</param>
        /// <param name="nonce">Nonce value.</param>
        /// <param name="opaque">Opaque value.</param>
        public Auth_HttpDigest(string realm, string nonce, string opaque)
        {
            m_Realm = realm;
            m_Nonce = nonce;
            Opaque = opaque;
        }

        /*
        public bool Stale
        {
            get{ return false; }
        }
        */

        /// <summary>
        /// Gets or sets algorithm to use to produce the digest and a checksum.
        /// This is normally MD5 or MD5-sess.
        /// </summary>
        public string Algorithm { get; set; } = "";

        /// <summary>
        /// Gets or sets Client nonce value. This MUST be specified if a qop directive is sent (see above), and
        /// MUST NOT be specified if the server did not send a qop directive in the WWW-Authenticate header field.
        /// </summary>
        public string CNonce
        {
            get => m_Cnonce;

            set
            {
                if (value == null)
                {
                    value = "";
                }
                m_Cnonce = value;
            }
        }

        /// <summary>
        /// Gets or sets a server-specified unique data string. It is recommended that this
        /// string be base64 or hexadecimal data.
        /// Suggested value: base64(time-stamp hex(time-stamp ":" ETag ":" private-key)).
        /// </summary>
        /// <exception cref="ArgumentException">Is raised when invalid value is specified.</exception>
        public string Nonce
        {
            get => m_Nonce;

            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentException("Nonce value can't be null or empty !");
                }

                m_Nonce = value;
            }
        }

        /// <summary>
        /// Gets or stets nonce count. This MUST be specified if a qop directive is sent (see above), and
        /// MUST NOT be specified if the server did not send a qop directive in the WWW-Authenticate
        /// header field.  The nc-value is the hexadecimal count of the number of requests.
        /// </summary>
        public int NonceCount { get; set; } = 1;

        /// <summary>
        /// Gets or sets string of data, specified by the server, which should be returned by the client unchanged.
        /// It is recommended that this string be base64 or hexadecimal data.
        /// </summary>
        /// <exception cref="ArgumentException">Is raised when invalid value is specified.</exception>
        public string Opaque { get; set; } = "";

        /// <summary>
        /// Gets or sets password.
        /// </summary>
        public string Password
        {
            get => m_Password;

            set
            {
                if (value == null)
                {
                    value = "";
                }
                m_Password = value;
            }
        }

        /// <summary>
        /// Gets or sets value what indicates "quality of protection" the client has applied to
        /// the message. If present, its value MUST be one of the alternatives the server indicated
        /// it supports in the WWW-Authenticate header. This directive is optional in order to preserve
        /// backward compatibility.
        /// </summary>
        public string Qop { get; set; } = "";

        /// <summary>
        /// Gets or sets a string to be displayed to users so they know which username and password
        /// to use. This string should contain at least the name of the host performing the
        /// authentication and might additionally indicate the collection of users who might have access.
        /// An example might be "registered_users@gotham.news.com".
        /// </summary>
        public string Realm
        {
            get => m_Realm;

            set
            {
                if (value == null)
                {
                    value = "";
                }
                m_Realm = value;
            }
        }

        /// <summary>
        /// Gets or sets request method.
        /// </summary>
        public string RequestMethod
        {
            get => m_Method;

            set
            {
                if (value == null)
                {
                    value = "";
                }
                m_Method = value;
            }
        }

        /// <summary>
        /// Gets a string of 32 hex digits computed by HTTP digest algorithm,
        /// which proves that the user knows a password.
        /// </summary>
        public string Response { get; private set; } = "";

        /// <summary>
        /// Gets the URI from Request-URI.
        /// </summary>
        public string Uri { get; set; } = "";

        /// <summary>
        /// Gets or sets user name.
        /// </summary>
        public string UserName
        {
            get => m_UserName;

            set
            {
                if (value == null)
                {
                    value = "";
                }
                m_UserName = value;
            }
        }

        /// <summary>
        /// Creates valid nonce value.
        /// </summary>
        /// <returns>Returns nonce value.</returns>
        public static string CreateNonce()
        {
            return Guid.NewGuid().ToString().Replace("-", "");
        }

        /// <summary>
        /// Creates valid opaque value.
        /// </summary>
        /// <returns>Renturn opaque value.</returns>
        public static string CreateOpaque()
        {
            return Guid.NewGuid().ToString().Replace("-", "");
        }

        /// <summary>
        /// Authenticates specified user and password using this class parameters.
        /// </summary>
        /// <param name="userName">User name.</param>
        /// <param name="password">Password.</param>
        /// <returns>Returns true if authenticated, otherwise false.</returns>
        public bool Authenticate(string userName, string password)
        {
            // Check that our computed digest is same as client provided.
            if (Response == CalculateResponse(userName, password))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Calculates response value.
        /// </summary>
        /// <param name="userName">User name.</param>
        /// <param name="password">User password.</param>
        /// <returns>Returns calculated rsponse value.</returns>
        public string CalculateResponse(string userName, string password)
        {
            /* RFC 2617.
             
                3.2.2.1 Request-Digest
            
                    If the "qop" value is "auth" or "auth-int":

                        request-digest  = <"> < KD ( H(A1),unq(nonce-value) ":" nc-value ":" unq(cnonce-value) ":" unq(qop-value) ":" H(A2) )> <">

                    If the "qop" directive is not present (this construction is for
                    compatibility with RFC 2069):

                        request-digest = <"> < KD ( H(A1), unq(nonce-value) ":" H(A2) ) > <">

                3.2.2.2 A1

                    If the "algorithm" directive's value is "MD5" or is unspecified, then A1 is:

                        A1 = unq(username-value) ":" unq(realm-value) ":" passwd

                    If the "algorithm" directive's value is "MD5-sess", then A1 is
                    calculated only once - on the first request by the client following
                    receipt of a WWW-Authenticate challenge from the server.  It uses the
                    server nonce from that challenge, and the first client nonce value to
                    construct A1 as follows:

                        A1 = H( unq(username-value) ":" unq(realm-value) ":" passwd ) ":" unq(nonce-value) ":" unq(cnonce-value)

                    This creates a 'session key' for the authentication of subsequent
                    requests and responses which is different for each "authentication
                    session", thus limiting the amount of material hashed with any one
                    key.  (Note: see further discussion of the authentication session in
                    section 3.3.) Because the server need only use the hash of the user
                    credentials in order to create the A1 value, this construction could
                    be used in conjunction with a third party authentication service so
                    that the web server would not need the actual password value.  The
                    specification of such a protocol is beyond the scope of this
                    specification.
            
                3.2.2.3 A2

                    If the "qop" directive's value is "auth" or is unspecified, then A2 is:

                        A2 = Method ":" digest-uri-value

                    If the "qop" value is "auth-int", then A2 is:

                        A2 = Method ":" digest-uri-value ":" H(entity-body)
              
            
                H(data) = MD5(data)
                KD(secret, data) = H(concat(secret, ":", data))
                unc = unqoute string
            */

            var A1 = "";
            if (string.IsNullOrEmpty(Algorithm) || Algorithm.ToLower() == "md5")
            {
                A1 = userName + ":" + Realm + ":" + password;
            }
            else if (Algorithm.ToLower() == "md5-sess")
            {
                A1 = H(userName + ":" + Realm + ":" + password) + ":" + Nonce + ":" + CNonce;
            }
            else
            {
                throw new ArgumentException("Invalid 'algorithm' value '" + Algorithm + "'.");
            }

            var A2 = "";
            if (string.IsNullOrEmpty(Qop) || Qop.ToLower() == "auth")
            {
                A2 = RequestMethod + ":" + Uri;
            }
            else
            {
                throw new ArgumentException("Invalid 'qop' value '" + Qop + "'.");
            }

            if (Qop.ToLower() == "auth" || Qop.ToLower() == "auth-int")
            {
                // request-digest  = <"> < KD ( H(A1),unq(nonce-value) ":" nc-value ":" unq(cnonce-value) ":" unq(qop-value) ":" H(A2) )> <">
                // We don't add quoutes here.

                return KD(H(A1), Nonce + ":" + NonceCount.ToString("x8") + ":" + CNonce + ":" + Qop + ":" + H(A2));
            }

            if (string.IsNullOrEmpty(Qop))
            {
                // request-digest = <"> < KD ( H(A1), unq(nonce-value) ":" H(A2) ) > <">
                // We don't add quoutes here.

                return KD(H(A1), Nonce + ":" + H(A2));
            }
            throw new ArgumentException("Invalid 'qop' value '" + Qop + "'.");
        }

        /// <summary>
        /// Calculates 'rspauth' value.
        /// </summary>
        /// <param name="userName">User name.</param>
        /// <param name="password">Password.</param>
        /// <returns>Returns 'rspauth' value.</returns>
        public string CalculateRspAuth(string userName, string password)
        {
            /* RFC 2617 3.2.3.
                The optional response digest in the "response-auth" directive
                supports mutual authentication -- the server proves that it knows the
                user's secret, and with qop=auth-int also provides limited integrity
                protection of the response. The "response-digest" value is calculated
                as for the "request-digest" in the Authorization header, except that
                if "qop=auth" or is not specified in the Authorization header for the
                request, A2 is

                    A2 = ":" digest-uri-value

                and if "qop=auth-int", then A2 is

                    A2 = ":" digest-uri-value ":" H(entity-body) 
             
                where "digest-uri-value" is the value of the "uri" directive on the
                Authorization header in the request. The "cnonce-value" and "nc-
                value" MUST be the ones for the client request to which this message
                is the response. The "response-auth", "cnonce", and "nonce-count"
                directives MUST BE present if "qop=auth" or "qop=auth-int" is
                specified.
            */

            var a1 = "";
            var a2 = "";
            // Create A1
            if (Algorithm == "" || Algorithm.ToLower() == "md5")
            {
                a1 = userName + ":" + Realm + ":" + password;
            }
            else if (Algorithm.ToLower() == "md5-sess")
            {
                a1 = Net_Utils.ComputeMd5(userName + ":" + Realm + ":" + password, false) + ":" + Nonce + ":" + CNonce;
            }
            else
            {
                throw new ArgumentException("Invalid Algorithm value '" + Algorithm + "' !");
            }
            // Create A2
            if (Qop == "" || Qop.ToLower() == "auth")
            {
                a2 = ":" + Uri;
            }
            else
            {
                throw new ArgumentException("Invalid qop value '" + Qop + "' !");
            }

            // Calculate response value.
            // qop present
            if (!string.IsNullOrEmpty(Qop))
            {
                return Net_Utils.ComputeMd5(Net_Utils.ComputeMd5(a1, true) + ":" + Nonce + ":" + NonceCount.ToString("x8") + ":" + CNonce + ":" + Qop + ":" + Net_Utils.ComputeMd5(a2, true), true);
            }
            // qop not present

            return Net_Utils.ComputeMd5(Net_Utils.ComputeMd5(a1, true) + ":" + Nonce + ":" + Net_Utils.ComputeMd5(a2, true), true);
        }

        /// <summary>
        /// Creates 'Authorization' data using this class info.
        /// </summary>
        /// <returns>Return Authorization data.</returns>
        public string ToAuthorization()
        {
            return ToAuthorization(true);
        }

        /// <summary>
        /// Creates 'Authorization' data using this class info.
        /// </summary>
        /// <param name="addAuthMethod">Specifies if 'digest ' authe method string constant is added.</param>
        /// <returns>Return Authorization data.</returns>
        public string ToAuthorization(bool addAuthMethod)
        {
            /* RFC 2831 2.1.2.
                digest-response  = 1#( username | realm | nonce | cnonce | nonce-count | qop | digest-uri | response |
                          maxbuf | charset | cipher | authzid | auth-param )
            */

            var response = "";
            if (string.IsNullOrEmpty(m_Password))
            {
                response = Response;
            }
            else
            {
                response = CalculateResponse(m_UserName, m_Password);
            }

            var authData = new StringBuilder();
            if (addAuthMethod)
            {
                authData.Append("digest ");
            }
            authData.Append("realm=\"" + m_Realm + "\",");
            authData.Append("username=\"" + m_UserName + "\",");
            authData.Append("nonce=\"" + m_Nonce + "\",");
            if (!string.IsNullOrEmpty(Uri))
            {
                authData.Append("uri=\"" + Uri + "\",");
            }
            if (!string.IsNullOrEmpty(Qop))
            {
                authData.Append("qop=\"" + Qop + "\",");
            }
            // nc value must be specified only if qop is present.
            if (!string.IsNullOrEmpty(Qop))
            {
                authData.Append("nc=" + NonceCount.ToString("x8") + ",");
            }
            if (!string.IsNullOrEmpty(m_Cnonce))
            {
                authData.Append("cnonce=\"" + m_Cnonce + "\",");
            }
            authData.Append("response=\"" + response + "\",");
            if (!string.IsNullOrEmpty(Opaque))
            {
                authData.Append("opaque=\"" + Opaque + "\",");
            }
            if (!string.IsNullOrEmpty(m_Charset))
            {
                authData.Append("charset=" + m_Charset + ",");
            }

            var retVal = authData.ToString().Trim();
            if (retVal.EndsWith(","))
            {
                retVal = retVal.Substring(0, retVal.Length - 1);
            }

            return retVal;
        }

        /// <summary>
        /// Creates 'Challange' data using this class info.
        /// </summary>
        /// <returns>Returns Challange data.</returns>
        public string ToChallange()
        {
            return ToChallange(true);
        }

        /// <summary>
        /// Creates 'Challange' data using this class info.
        /// </summary>
        /// <param name="addAuthMethod">Specifies if 'digest ' authe method string constant is added.</param>
        /// <returns>Returns Challange data.</returns>
        public string ToChallange(bool addAuthMethod)
        {
            // digest realm="",qop="",nonce="",opaque=""

            var retVal = new StringBuilder();
            if (addAuthMethod)
            {
                retVal.Append("digest ");
            }
            retVal.Append("realm=" + TextUtils.QuoteString(m_Realm) + ",");
            if (!string.IsNullOrEmpty(Qop))
            {
                retVal.Append("qop=" + TextUtils.QuoteString(Qop) + ",");
            }
            retVal.Append("nonce=" + TextUtils.QuoteString(m_Nonce) + ",");
            retVal.Append("opaque=" + TextUtils.QuoteString(Opaque));

            return retVal.ToString();
        }

        /// <summary>
        /// Converts this to valid digest string.
        /// </summary>
        /// <returns>Returns digest string.</returns>
        public override string ToString()
        {
            var retVal = new StringBuilder();
            retVal.Append("realm=\"" + m_Realm + "\",");
            retVal.Append("username=\"" + m_UserName + "\",");
            if (!string.IsNullOrEmpty(Qop))
            {
                retVal.Append("qop=\"" + Qop + "\",");
            }
            retVal.Append("nonce=\"" + m_Nonce + "\",");
            retVal.Append("nc=\"" + NonceCount + "\",");
            retVal.Append("cnonce=\"" + m_Cnonce + "\",");
            retVal.Append("response=\"" + Response + "\",");
            retVal.Append("opaque=\"" + Opaque + "\",");
            retVal.Append("uri=\"" + Uri + "\"");

            return retVal.ToString();
        }

        private string H(string value)
        {
            return Net_Utils.ComputeMd5(value, true);
        }

        private string KD(string key, string data)
        {
            // KD(secret, data) = H(concat(secret, ":", data))

            return H(key + ":" + data);
        }

        /// <summary>
        /// Parses authetication info from client digest response.
        /// </summary>
        /// <param name="digestResponse">Client returned digest response.</param>
        private void Parse(string digestResponse)
        {
            var parameters = TextUtils.SplitQuotedString(digestResponse, ',');
            foreach (string parameter in parameters)
            {
                var name_value = parameter.Split(new[] { '=' }, 2);
                var name = name_value[0].Trim();

                if (name_value.Length == 2)
                {
                    if (name.ToLower() == "realm")
                    {
                        m_Realm = TextUtils.UnQuoteString(name_value[1]);
                    }
                    else if (name.ToLower() == "nonce")
                    {
                        m_Nonce = TextUtils.UnQuoteString(name_value[1]);
                    }
                    // RFC bug ?: RFC 2831. digest-uri = "digest-uri" "=" <"> digest-uri-value <">
                    //            RFC 2617  digest-uri        = "uri" "=" digest-uri-value
                    else if (name.ToLower() == "uri" || name.ToLower() == "digest-uri")
                    {
                        Uri = TextUtils.UnQuoteString(name_value[1]);
                    }
                    else if (name.ToLower() == "qop")
                    {
                        Qop = TextUtils.UnQuoteString(name_value[1]);
                    }
                    else if (name.ToLower() == "nc")
                    {
                        NonceCount = Convert.ToInt32(TextUtils.UnQuoteString(name_value[1]));
                    }
                    else if (name.ToLower() == "cnonce")
                    {
                        m_Cnonce = TextUtils.UnQuoteString(name_value[1]);
                    }
                    else if (name.ToLower() == "response")
                    {
                        Response = TextUtils.UnQuoteString(name_value[1]);
                    }
                    else if (name.ToLower() == "opaque")
                    {
                        Opaque = TextUtils.UnQuoteString(name_value[1]);
                    }
                    else if (name.ToLower() == "username")
                    {
                        m_UserName = TextUtils.UnQuoteString(name_value[1]);
                    }
                    else if (name.ToLower() == "algorithm")
                    {
                        Algorithm = TextUtils.UnQuoteString(name_value[1]);
                    }
                    else if (name.ToLower() == "charset")
                    {
                        m_Charset = TextUtils.UnQuoteString(name_value[1]);
                    }
                }
            }
        }
    }
}
