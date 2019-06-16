﻿using System;
using System.Text;

namespace LumiSoft.Net.IMAP
{
    /// <summary>
    /// This class represents IMAP server untagged status(OK,NO,BAD,PREAUTH and BYE) response. Defined in RFC 3501 7.1.
    /// </summary>
    public class IMAP_r_u_ServerStatus : IMAP_r_u
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="responseCode">Response code.</param>
        /// <param name="responseText">Response text after response-code.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>responseCode</b> is null reference.</exception>
        /// <exception cref="ArgumentException">Is raised when any of the arguments has invalid value.</exception>
        public IMAP_r_u_ServerStatus(string responseCode, string responseText) : this(responseCode, null, responseText)
        {
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="responseCode">Response code.</param>
        /// <param name="optionalResponse">Optional response. Value null means not specified.</param>
        /// <param name="responseText">Response text after response-code.</param>
        /// <exception cref="ArgumentNullException">Is raised when<b>responseCode</b> or <b>responseText</b> is null reference.</exception>
        /// <exception cref="ArgumentException">Is raised when any of the arguments has invalid value.</exception>
        public IMAP_r_u_ServerStatus(string responseCode, IMAP_t_orc optionalResponse, string responseText)
        {
            if (responseCode == null)
            {
                throw new ArgumentNullException("responseCode");
            }
            if (responseCode == string.Empty)
            {
                throw new ArgumentException("The argument 'responseCode' value must be specified.", "responseCode");
            }

            ResponseCode = responseCode;
            OptionalResponse = optionalResponse;
            ResponseText = responseText;
        }

        /// <summary>
        /// Gets if this response is error response.
        /// </summary>
        public bool IsError
        {
            get { return !ResponseCode.Equals("OK", StringComparison.InvariantCultureIgnoreCase); }
        }

        /// <summary>
        /// Gets IMAP server otional response-code. Value null means no optional response.
        /// </summary>
        public IMAP_t_orc OptionalResponse { get; }

        /// <summary>
        /// Gets optional response aruments string. Value null means not specified. For more info see RFC 3501 7.1.
        /// </summary>
        [Obsolete("Use property OptionalResponse instead.")]
        public string OptionalResponseArgs
        {
            get
            {
                if (OptionalResponse == null)
                {
                    return null;
                }

                var code_args = OptionalResponse.ToString().Split(new[] { ' ' }, 2);

                return code_args.Length == 2 ? code_args[1] : "";
            }
        }

        /// <summary>
        /// Gets IMAP server status response optiona response-code(ALERT,BADCHARSET,CAPABILITY,PARSE,PERMANENTFLAGS,
        /// READ-ONLY,READ-WRITE,TRYCREATE,UIDNEXT,UIDVALIDITY,UNSEEN).
        /// Value null means not specified. For more info see RFC 3501 7.1.
        /// </summary>
        [Obsolete("Use property OptionalResponse instead.")]
        public string OptionalResponseCode
        {
            get
            {
                return OptionalResponse?.ToString().Split(' ')[0];
            }
        }

        /// <summary>
        /// Gets IMAP server status response code(OK,NO,BAD,PREAUTH,BYE).
        /// </summary>
        public string ResponseCode { get; } = "";

        /// <summary>
        /// Gets response human readable text after response-code.
        /// </summary>
        public string ResponseText { get; } = "";

        /// <summary>
        /// Parses IMAP command completion status response from response line.
        /// </summary>
        /// <param name="responseLine">Response line.</param>
        /// <returns>Returns parsed IMAP command completion status response.</returns>
        /// <exception cref="ArgumentNullException">Is raised when <b>responseLine</b> is null reference value.</exception>
        public static IMAP_r_u_ServerStatus Parse(string responseLine)
        {
            if (responseLine == null)
            {
                throw new ArgumentNullException("responseLine");
            }

            var parts = responseLine.Split(new[] { ' ' }, 3);
            var commandTag = parts[0];
            var responseCode = parts[1];
            IMAP_t_orc optResponse = null;
            var responseText = parts[2];

            // Optional status code.
            if (parts[2].StartsWith("["))
            {
                var r = new StringReader(parts[2]);
                optResponse = IMAP_t_orc.Parse(r.ReadParenthesized());
                responseText = r.ReadToEnd();
            }

            return new IMAP_r_u_ServerStatus(responseCode, optResponse, responseText);
        }

        /// <summary>
        /// Returns this as string.
        /// </summary>
        /// <returns>Returns this as string.</returns>
        public override string ToString()
        {
            var retVal = new StringBuilder();
            retVal.Append("* " + ResponseCode + " ");
            if (OptionalResponse != null)
            {
                retVal.Append("[" + OptionalResponse.ToString() + "] ");
            }
            retVal.Append(ResponseText + "\r\n");

            return retVal.ToString();
        }
    }
}
