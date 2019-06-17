﻿using System;
using System.Text;

namespace LumiSoft.Net.AUTH
{
    /// <summary>
    /// This class represents SASL DIGEST-MD5 authentication <b>digest-response</b>. Defined in RFC 2831.
    /// </summary>
    public class AUTH_SASL_DigestMD5_Response
    {
        private string _password;
        private readonly AUTH_SASL_DigestMD5_Challenge _challenge;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="challenge">Client challenge.</param>
        /// <param name="realm">Realm value. This must be one value of the challenge Realm.</param>
        /// <param name="userName">User name.</param>
        /// <param name="password">User password.</param>
        /// <param name="cnonce">Client nonce value.</param>
        /// <param name="nonceCount">Nonce count. One-based client authentication attempt number. Normally this value is 1.</param>
        /// <param name="qop">Indicates what "quality of protection" the client accepted. This must be one value of the challenge QopOptions.</param>
        /// <param name="digestUri">Digest URI.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>challenge</b>,<b>realm</b>,<b>password</b>,<b>nonce</b>,<b>qop</b> or <b>digestUri</b> is null reference.</exception>
        public AUTH_SASL_DigestMD5_Response(AUTH_SASL_DigestMD5_Challenge challenge, string realm, string userName, string password, string cnonce, int nonceCount, string qop, string digestUri)
        {
            _challenge = challenge ?? throw new ArgumentNullException("challenge");
            Realm = realm ?? throw new ArgumentNullException("realm");
            UserName = userName ?? throw new ArgumentNullException("userName");
            _password = password ?? throw new ArgumentNullException("password");
            Nonce = _challenge.Nonce;
            Cnonce = cnonce ?? throw new ArgumentNullException("cnonce");
            NonceCount = nonceCount;
            Qop = qop ?? throw new ArgumentNullException("qop");
            DigestUri = digestUri ?? throw new ArgumentNullException("digestUri");
            Response = CalculateResponse(userName, password);
            Charset = challenge.Charset;
        }

        /// <summary>
        /// Internal parse constructor.
        /// </summary>
        private AUTH_SASL_DigestMD5_Response()
        {
        }

        /// <summary>
        /// Gets authorization ID.
        /// </summary>
        public string Authzid { get; private set; }

        /// <summary>
        /// Gets charset value.
        /// </summary>
        public string Charset { get; private set; }

        /// <summary>
        /// Gets cipher value.
        /// </summary>
        public string Cipher { get; private set; }

        /// <summary>
        /// Gets cnonce value.
        /// </summary>
        public string Cnonce { get; private set; }

        /// <summary>
        /// Gets digest URI value.
        /// </summary>
        public string DigestUri { get; private set; }

        /// <summary>
        /// Gets nonce value.
        /// </summary>
        public string Nonce { get; private set; }

        /// <summary>
        /// Gets nonce count.
        /// </summary>
        public int NonceCount { get; private set; }

        /// <summary>
        /// Gets "quality of protection" value.
        /// </summary>
        public string Qop { get; private set; }

        /// <summary>
        /// Gets realm(domain) name.
        /// </summary>
        public string Realm { get; private set; }

        /// <summary>
        /// Gets response value.
        /// </summary>
        public string Response { get; private set; }

        /// <summary>
        /// Gets user name.
        /// </summary>
        public string UserName { get; private set; }

        /// <summary>
        /// Parses DIGEST-MD5 response from response-string.
        /// </summary>
        /// <param name="digestResponse">Response string.</param>
        /// <returns>Returns DIGEST-MD5 response.</returns>
        /// <exception cref="ArgumentNullException">Is raised when <b>digestResponse</b> isnull reference.</exception>
        /// <exception cref="ParseException">Is raised when response parsing + validation fails.</exception>
        public static AUTH_SASL_DigestMD5_Response Parse(string digestResponse)
        {
            if (digestResponse == null)
            {
                throw new ArgumentNullException(digestResponse);
            }

            /* RFC 2831 2.1.2.
                The client makes note of the "digest-challenge" and then responds
                with a string formatted and computed according to the rules for a
                "digest-response" defined as follows:

                digest-response  = 1#( username | realm | nonce | cnonce |
                                       nonce-count | qop | digest-uri | response |
                                       maxbuf | charset | cipher | authzid |
                                       auth-param )

                username         = "username" "=" <"> username-value <">
                username-value   = qdstr-val
                cnonce           = "cnonce" "=" <"> cnonce-value <">
                cnonce-value     = qdstr-val
                nonce-count      = "nc" "=" nc-value
                nc-value         = 8LHEX
                qop              = "qop" "=" qop-value
                digest-uri       = "digest-uri" "=" <"> digest-uri-value <">
                digest-uri-value  = serv-type "/" host [ "/" serv-name ]
                serv-type        = 1*ALPHA
                host             = 1*( ALPHA | DIGIT | "-" | "." )
                serv-name        = host
                response         = "response" "=" response-value
                response-value   = 32LHEX
                LHEX             = "0" | "1" | "2" | "3" |
                                   "4" | "5" | "6" | "7" |
                                   "8" | "9" | "a" | "b" |
                                   "c" | "d" | "e" | "f"
                cipher           = "cipher" "=" cipher-value
                authzid          = "authzid" "=" <"> authzid-value <">
                authzid-value    = qdstr-val
            */

            var retVal = new AUTH_SASL_DigestMD5_Response();

            // Set default values.
            retVal.Realm = "";

            var parameters = TextUtils.SplitQuotedString(digestResponse, ',');
            foreach (string parameter in parameters)
            {
                var name_value = parameter.Split(new[] { '=' }, 2);
                var name = name_value[0].Trim();

                if (name_value.Length == 2)
                {
                    if (name.ToLower() == "username")
                    {
                        retVal.UserName = TextUtils.UnQuoteString(name_value[1]);
                    }
                    else if (name.ToLower() == "realm")
                    {
                        retVal.Realm = TextUtils.UnQuoteString(name_value[1]);
                    }
                    else if (name.ToLower() == "nonce")
                    {
                        retVal.Nonce = TextUtils.UnQuoteString(name_value[1]);
                    }
                    else if (name.ToLower() == "cnonce")
                    {
                        retVal.Cnonce = TextUtils.UnQuoteString(name_value[1]);
                    }
                    else if (name.ToLower() == "nc")
                    {
                        retVal.NonceCount = Int32.Parse(TextUtils.UnQuoteString(name_value[1]), System.Globalization.NumberStyles.HexNumber);
                    }
                    else if (name.ToLower() == "qop")
                    {
                        retVal.Qop = TextUtils.UnQuoteString(name_value[1]);
                    }
                    else if (name.ToLower() == "digest-uri")
                    {
                        retVal.DigestUri = TextUtils.UnQuoteString(name_value[1]);
                    }
                    else if (name.ToLower() == "response")
                    {
                        retVal.Response = TextUtils.UnQuoteString(name_value[1]);
                    }
                    else if (name.ToLower() == "charset")
                    {
                        retVal.Charset = TextUtils.UnQuoteString(name_value[1]);
                    }
                    else if (name.ToLower() == "cipher")
                    {
                        retVal.Cipher = TextUtils.UnQuoteString(name_value[1]);
                    }
                    else if (name.ToLower() == "authzid")
                    {
                        retVal.Authzid = TextUtils.UnQuoteString(name_value[1]);
                    }
                }
            }

            /* Validate required fields.
                Per RFC 2831 2.1.2. Only [username nonce cnonce nc response] parameters are required.
            */
            if (string.IsNullOrEmpty(retVal.UserName))
            {
                throw new ParseException("The response-string doesn't contain required parameter 'username' value.");
            }
            if (string.IsNullOrEmpty(retVal.Nonce))
            {
                throw new ParseException("The response-string doesn't contain required parameter 'nonce' value.");
            }
            if (string.IsNullOrEmpty(retVal.Cnonce))
            {
                throw new ParseException("The response-string doesn't contain required parameter 'cnonce' value.");
            }
            if (retVal.NonceCount < 1)
            {
                throw new ParseException("The response-string doesn't contain required parameter 'nc' value.");
            }
            if (string.IsNullOrEmpty(retVal.Response))
            {
                throw new ParseException("The response-string doesn't contain required parameter 'response' value.");
            }

            return retVal;
        }

        /// <summary>
        /// Authenticates user.
        /// </summary>
        /// <param name="userName">User name.</param>
        /// <param name="password">Password.</param>
        /// <returns>Returns true if user authenticated, otherwise false.</returns>
        /// <exception cref="ArgumentNullException">Is raised when <b>userName</b> or <b>password</b> is null reference.</exception>
        public bool Authenticate(string userName, string password)
        {
            if (userName == null)
            {
                throw new ArgumentNullException("userName");
            }
            if (password == null)
            {
                throw new ArgumentNullException("password");
            }

            if (Response == CalculateResponse(userName, password))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Creates digest response for challenge.
        /// </summary>
        /// <returns>Returns digest response.</returns>
        public string ToResponse()
        {
            /* RFC 2831 2.1.2.
                The client makes note of the "digest-challenge" and then responds
                with a string formatted and computed according to the rules for a
                "digest-response" defined as follows:

                digest-response  = 1#( username | realm | nonce | cnonce |
                                       nonce-count | qop | digest-uri | response |
                                       maxbuf | charset | cipher | authzid |
                                       auth-param )

                username         = "username" "=" <"> username-value <">
                username-value   = qdstr-val
                cnonce           = "cnonce" "=" <"> cnonce-value <">
                cnonce-value     = qdstr-val
                nonce-count      = "nc" "=" nc-value
                nc-value         = 8LHEX
                qop              = "qop" "=" qop-value
                digest-uri       = "digest-uri" "=" <"> digest-uri-value <">
                digest-uri-value  = serv-type "/" host [ "/" serv-name ]
                serv-type        = 1*ALPHA
                host             = 1*( ALPHA | DIGIT | "-" | "." )
                serv-name        = host
                response         = "response" "=" response-value
                response-value   = 32LHEX
                LHEX             = "0" | "1" | "2" | "3" |
                                   "4" | "5" | "6" | "7" |
                                   "8" | "9" | "a" | "b" |
                                   "c" | "d" | "e" | "f"
                cipher           = "cipher" "=" cipher-value
                authzid          = "authzid" "=" <"> authzid-value <">
                authzid-value    = qdstr-val
            */

            var retVal = new StringBuilder();
            retVal.Append("username=\"" + UserName + "\"");
            retVal.Append(",realm=\"" + Realm + "\"");
            retVal.Append(",nonce=\"" + Nonce + "\"");
            retVal.Append(",cnonce=\"" + Cnonce + "\"");
            retVal.Append(",nc=" + NonceCount.ToString("x8"));
            retVal.Append(",qop=" + Qop);
            retVal.Append(",digest-uri=\"" + DigestUri + "\"");
            retVal.Append(",response=" + Response);
            if (!string.IsNullOrEmpty(Charset))
            {
                retVal.Append(",charset=" + Charset);
            }
            if (!string.IsNullOrEmpty(Cipher))
            {
                retVal.Append(",cipher=\"" + Cipher + "\"");
            }
            if (!string.IsNullOrEmpty(Authzid))
            {
                retVal.Append(",authzid=\"" + Authzid + "\"");
            }
            // auth-param

            return retVal.ToString();
        }

        /// <summary>
        /// Creates <b>response-auth</b> response for client.
        /// </summary>
        /// <returns>Returns <b>response-auth</b> response.</returns>
        public string ToRspauthResponse(string userName, string password)
        {
            /* RFC 2831 2.1.3.
                The server receives and validates the "digest-response". The server
                checks that the nonce-count is "00000001". If it supports subsequent
                authentication (see section 2.2), it saves the value of the nonce and
                the nonce-count. It sends a message formatted as follows:

                    response-auth = "rspauth" "=" response-value

                where response-value is calculated as above, using the values sent in
                step two, except that if qop is "auth", then A2 is

                    A2 = { ":", digest-uri-value }

                And if qop is "auth-int" or "auth-conf" then A2 is

                    A2 = { ":", digest-uri-value, ":00000000000000000000000000000000" }

                Compared to its use in HTTP, the following Digest directives in the
                "digest-response" are unused:

                    nextnonce
                    qop
                    cnonce
                    nonce-count
             
                response-value  =
                    HEX( KD ( HEX(H(A1)),
                        { nonce-value, ":" nc-value, ":", cnonce-value, ":", qop-value, ":", HEX(H(A2)) }))
            */

            byte[] a2 = null;
            if (string.IsNullOrEmpty(Qop) || Qop.ToLower() == "auth")
            {
                a2 = Encoding.UTF8.GetBytes(":" + DigestUri);
            }
            else if (Qop.ToLower() == "auth-int" || Qop.ToLower() == "auth-conf")
            {
                a2 = Encoding.UTF8.GetBytes(":" + DigestUri + ":00000000000000000000000000000000");
            }

            if (Qop.ToLower() == "auth")
            {
                // RFC 2831 2.1.2.1.
                // response-value = HEX(KD(HEX(H(A1)),{nonce-value,":" nc-value,":",cnonce-value,":",qop-value,":",HEX(H(A2))}))

                return "rspauth=" + Hex(KD(Hex(H(A1(userName, password))), Nonce + ":" + NonceCount.ToString("x8") + ":" + Cnonce + ":" + Qop + ":" + Hex(H(a2))));
            }

            throw new ArgumentException("Invalid 'qop' value '" + Qop + "'.");
        }

        /// <summary>
        /// Calculates A1 value.
        /// </summary>
        /// <param name="userName">User name.</param>
        /// <param name="password">Password.</param>
        /// <returns>Returns A1 value.</returns>
        private byte[] A1(string userName, string password)
        {
            /* RFC 2831 2.1.2.1.
                If authzid is specified, then A1 is

                A1 = { H( { username-value, ":", realm-value, ":", passwd } ),
                      ":", nonce-value, ":", cnonce-value, ":", authzid-value }

                If authzid is not specified, then A1 is

                A1 = { H( { username-value, ":", realm-value, ":", passwd } ),
                      ":", nonce-value, ":", cnonce-value
             
                NOTE: HTTP MD5 RFC 2617 supports more algorithms. SASL requires md5-sess.
            */

            if (string.IsNullOrEmpty(Authzid))
            {
                var user_realm_pwd = H(Encoding.UTF8.GetBytes(userName + ":" + Realm + ":" + password));
                var nonce_cnonce = Encoding.UTF8.GetBytes(":" + Nonce + ":" + Cnonce);

                var retVal = new byte[user_realm_pwd.Length + nonce_cnonce.Length];
                Array.Copy(user_realm_pwd, 0, retVal, 0, user_realm_pwd.Length);
                Array.Copy(nonce_cnonce, 0, retVal, user_realm_pwd.Length, nonce_cnonce.Length);

                return retVal;
            }
            else
            {
                var user_realm_pwd = H(Encoding.UTF8.GetBytes(userName + ":" + Realm + ":" + password));
                var nonce_cnonce_authzid = Encoding.UTF8.GetBytes(":" + Nonce + ":" + Cnonce + ":" + Authzid);

                var retVal = new byte[user_realm_pwd.Length + nonce_cnonce_authzid.Length];
                Array.Copy(user_realm_pwd, 0, retVal, 0, user_realm_pwd.Length);
                Array.Copy(nonce_cnonce_authzid, 0, retVal, user_realm_pwd.Length, nonce_cnonce_authzid.Length);

                return retVal;
            }
        }

        /// <summary>
        /// Calculates A2 value.
        /// </summary>
        /// <returns>Returns A2 value.</returns>
        private byte[] A2()
        {
            /* RFC 2831 2.1.2.1.
                If the "qop" directive's value is "auth", then A2 is:

                    A2       = { "AUTHENTICATE:", digest-uri-value }

                If the "qop" value is "auth-int" or "auth-conf" then A2 is:

                    A2       = { "AUTHENTICATE:", digest-uri-value, ":00000000000000000000000000000000" }

                Note that "AUTHENTICATE:" must be in upper case, and the second
                string constant is a string with a colon followed by 32 zeros.
             
                RFC 2617(HTTP MD5) 3.2.2.3.
                    A2       = Method ":" digest-uri-value ":" H(entity-body)

                NOTE: In SASL entity-body hash always "00000000000000000000000000000000".
            */

            if (string.IsNullOrEmpty(Qop) || Qop.ToLower() == "auth")
            {
                return Encoding.UTF8.GetBytes("AUTHENTICATE:" + DigestUri);
            }

            if (Qop.ToLower() == "auth-int" || Qop.ToLower() == "auth-conf")
            {
                return Encoding.UTF8.GetBytes("AUTHENTICATE:" + DigestUri + ":00000000000000000000000000000000");
            }
            throw new ArgumentException("Invalid 'qop' value '" + Qop + "'.");
        }

        /// <summary>
        /// Calculates digest response.
        /// </summary>
        /// <param name="userName">User name.</param>
        /// <param name="password">Password.</param>
        /// <returns>Returns digest response.</returns>
        private string CalculateResponse(string userName, string password)
        {
            /* RFC 2831.2.1.2.1.
                The definition of "response-value" above indicates the encoding for
                its value -- 32 lower case hex characters. The following definitions
                show how the value is computed.

                Although qop-value and components of digest-uri-value may be
                case-insensitive, the case which the client supplies in step two is
                preserved for the purpose of computing and verifying the
                response-value.

                response-value  =
                    HEX( KD ( HEX(H(A1)),
                        { nonce-value, ":" nc-value, ":", cnonce-value, ":", qop-value, ":", HEX(H(A2)) }))

                If authzid is specified, then A1 is

                    A1 = { H( { username-value, ":", realm-value, ":", passwd } ),
                        ":", nonce-value, ":", cnonce-value, ":", authzid-value }

                If authzid is not specified, then A1 is

                    A1 = { H( { username-value, ":", realm-value, ":", passwd } ),
                        ":", nonce-value, ":", cnonce-value }

                The "username-value", "realm-value" and "passwd" are encoded
                according to the value of the "charset" directive. If "charset=UTF-8"
                is present, and all the characters of either "username-value" or
                "passwd" are in the ISO 8859-1 character set, then it must be
                converted to ISO 8859-1 before being hashed. This is so that
                authentication databases that store the hashed username, realm and
                password (which is common) can be shared compatibly with HTTP, which
                specifies ISO 8859-1. A sample implementation of this conversion is
                in section 8.

                If the "qop" directive's value is "auth", then A2 is:

                    A2       = { "AUTHENTICATE:", digest-uri-value }

                If the "qop" value is "auth-int" or "auth-conf" then A2 is:

                    A2       = { "AUTHENTICATE:", digest-uri-value,
                                ":00000000000000000000000000000000" }

                Note that "AUTHENTICATE:" must be in upper case, and the second
                string constant is a string with a colon followed by 32 zeros.

                These apparently strange values of A2 are for compatibility with
                HTTP; they were arrived at by setting "Method" to "AUTHENTICATE" and
                the hash of the entity body to zero in the HTTP digest calculation of
                A2.

                Also, in the HTTP usage of Digest, several directives in the

                "digest-challenge" sent by the server have to be returned by the
                client in the "digest-response". These are:

                    opaque
                    algorithm

                These directives are not needed when Digest is used as a SASL
                mechanism (i.e., MUST NOT be sent, and MUST be ignored if received).
            */

            if (string.IsNullOrEmpty(Qop) || Qop.ToLower() == "auth")
            {
                // RFC 2831 2.1.2.1.
                // response-value = HEX(KD(HEX(H(A1)),{nonce-value,":" nc-value,":",cnonce-value,":",qop-value,":",HEX(H(A2))}))

                return Hex(KD(Hex(H(A1(userName, password))), Nonce + ":" + NonceCount.ToString("x8") + ":" + Cnonce + ":" + Qop + ":" + Hex(H(A2()))));
            }

            throw new ArgumentException("Invalid 'qop' value '" + Qop + "'.");
        }

        /// <summary>
        /// Computes MD5 hash.
        /// </summary>
        /// <param name="value">Value to process.</param>
        /// <returns>Return MD5 hash.</returns>
        private byte[] H(byte[] value)
        {
            System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();

            return md5.ComputeHash(value);
        }

        /// <summary>
        /// Converts value to hex string.
        /// </summary>
        /// <param name="value">Value to convert.</param>
        /// <returns>Returns hex string.</returns>
        private string Hex(byte[] value)
        {
            return Net_Utils.ToHex(value);
        }

        private byte[] KD(string secret, string data)
        {
            // KD(secret, data) = H(concat(secret, ":", data))

            return H(Encoding.UTF8.GetBytes(secret + ":" + data));
        }
    }
}
