using System;
using System.Text;

namespace LumiSoft.Net.IMAP
{
    /// <summary>
    /// This class represents IMAP server status(OK,NO,BAD) response. Defined in RFC 3501 7.1.
    /// </summary>
    public class IMAP_r_ServerStatus : IMAP_r
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="commandTag">Command tag.</param>
        /// <param name="responseCode">Response code.</param>
        /// <param name="responseText">Response text after response-code.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>commandTag</b>,<b>responseCode</b> or <b>responseText</b> is null reference.</exception>
        /// <exception cref="ArgumentException">Is raised when any of the arguments has invalid value.</exception>
        public IMAP_r_ServerStatus(string commandTag,string responseCode,string responseText) : this(commandTag,responseCode,null,responseText)
        {
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="commandTag">Command tag.</param>
        /// <param name="responseCode">Response code.</param>
        /// <param name="optionalResponse">Optional response. Value null means not specified.</param>
        /// <param name="responseText">Response text after response-code.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>commandTag</b>,<b>responseCode</b> or <b>responseText</b> is null reference.</exception>
        /// <exception cref="ArgumentException">Is raised when any of the arguments has invalid value.</exception>
        public IMAP_r_ServerStatus(string commandTag,string responseCode,IMAP_t_orc optionalResponse,string responseText)
        {
            if(commandTag == null){
                throw new ArgumentNullException("commandTag");
            }
            if(commandTag == string.Empty){
                throw new ArgumentException("The argument 'commandTag' value must be specified.","commandTag");
            }
            if(responseCode == null){
                throw new ArgumentNullException("responseCode");
            }
            if(responseCode == string.Empty){
                throw new ArgumentException("The argument 'responseCode' value must be specified.","responseCode");
            }

            CommandTag        = commandTag;
            ResponseCode      = responseCode;
            OptionalResponse = optionalResponse;
            ResponseText      = responseText;
        }

        /// <summary>
        /// Default cmdTag-less constructor.
        /// </summary>
        /// <param name="responseCode">Response code.</param>
        /// <param name="responseText">Response text after response-code.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>responseCode</b> or <b>responseText</b> is null reference.</exception>
        /// <exception cref="ArgumentException">Is raised when any of the arguments has invalid value.</exception>
        internal IMAP_r_ServerStatus(string responseCode,string responseText)
        {
            ResponseCode = responseCode;
            ResponseText = responseText;
        }


        /// <summary>
        /// Parses IMAP command completion status response from response line.
        /// </summary>
        /// <param name="responseLine">Response line.</param>
        /// <returns>Returns parsed IMAP command completion status response.</returns>
        /// <exception cref="ArgumentNullException">Is raised when <b>responseLine</b> is null reference value.</exception>
        public static IMAP_r_ServerStatus Parse(string responseLine)
        {
            if(responseLine == null){
                throw new ArgumentNullException("responseLine");
            }

            // We continuation "+" response.
            if(responseLine.StartsWith("+")){
                string[] parts        = responseLine.Split(new char[]{' '},2);
                string   responseText = parts.Length == 2 ? parts[1] : null;

                return new IMAP_r_ServerStatus("+","+",responseText);
            }
            // OK/BAD/NO
            else{
                string[]   parts        = responseLine.Split(new char[]{' '},3);
                string     commandTag   = parts[0];
                string     responseCode = parts[1];
                IMAP_t_orc optResponse  = null;
                string   responseText   = parts[2];

                // Optional status code.
                if(parts[2].StartsWith("[")){
                    StringReader r = new StringReader(parts[2]);
                    optResponse  = IMAP_t_orc.Parse(r.ReadParenthesized());
                    responseText = r.ReadToEnd();
                }

                return new IMAP_r_ServerStatus(commandTag,responseCode,optResponse,responseText);
            }
        }


        /// <summary>
        /// Returns this as string.
        /// </summary>
        /// <returns>Returns this as string.</returns>
        public override string ToString()
        {
            StringBuilder retVal = new StringBuilder();
            if(!string.IsNullOrEmpty(CommandTag)){
                retVal.Append(CommandTag + " ");
            }
            retVal.Append(ResponseCode + " ");
            if(OptionalResponse != null){
                retVal.Append("[" + OptionalResponse.ToString() + "] ");
            }
            retVal.Append(ResponseText + "\r\n");

            return retVal.ToString();
        }


        /// <summary>
        /// Gets command tag.
        /// </summary>
        public string CommandTag { get; } = "";

        /// <summary>
        /// Gets IMAP server status response code(OK,NO,BAD).
        /// </summary>
        public string ResponseCode { get; } = "";

        /// <summary>
        /// Gets IMAP server otional response-code. Value null means no optional response.
        /// </summary>
        public IMAP_t_orc OptionalResponse { get; }

        /// <summary>
        /// Gets response human readable text after response-code.
        /// </summary>
        public string ResponseText { get; } = "";

        /// <summary>
        /// Gets if this response is error response.
        /// </summary>
        public bool IsError
        {
            get{ 
                if(ResponseCode.Equals("NO",StringComparison.InvariantCultureIgnoreCase)){
                    return true;
                }
                else if(ResponseCode.Equals("BAD",StringComparison.InvariantCultureIgnoreCase)){
                    return true;
                }
                else{
                    return false;
                }
            }
        }

        /// <summary>
        /// Gets if this response is continuation response.
        /// </summary>
        public bool IsContinue
        {
            get{ return ResponseCode.Equals("+",StringComparison.InvariantCultureIgnoreCase); }
        }


        /// <summary>
        /// Gets IMAP server status response optiona response-code(ALERT,BADCHARSET,CAPABILITY,PARSE,PERMANENTFLAGS,
        /// READ-ONLY,READ-WRITE,TRYCREATE,UIDNEXT,UIDVALIDITY,UNSEEN).
        /// Value null means not specified. For more info see RFC 3501 7.1.
        /// </summary>
        [Obsolete("Use property OptionalResponse instead.")]
        public string OptionalResponseCode
        {
            get{ 
                if(OptionalResponse == null){
                    return null;
                }
                else{
                    return OptionalResponse.ToString().Split(' ')[0];
                }
            }
        }

        /// <summary>
        /// Gets optional response aruments string. Value null means not specified. For more info see RFC 3501 7.1.
        /// </summary>
        [Obsolete("Use property OptionalResponse instead.")]
        public string OptionalResponseArgs
        {
            get{ 
                if(OptionalResponse == null){
                    return null;
                }
                else{
                    string[] code_args = OptionalResponse.ToString().Split(new char[]{' '},2);

                    return code_args.Length == 2 ? code_args[1] : "";
                }
            }
        }
    }
}
