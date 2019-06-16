using System;

namespace LumiSoft.Net.POP3.Client
{
    /// <summary>
    /// POP3 client exception.
    /// </summary>
    public class POP3_ClientException : Exception
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="responseLine">IMAP server response line.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>responseLine</b> is null.</exception>
        public POP3_ClientException(string responseLine) : base(responseLine)
        {
            if(responseLine == null){
                throw new ArgumentNullException("responseLine");
            }

            // <status-code> SP <response-text>
            string[] code_text = responseLine.Split(new char[]{ },2);
            StatusCode = code_text[0];
            if(code_text.Length == 2){
                ResponseText = code_text[1];
            }
        }


        /// <summary>
        /// Gets POP3 server error status code.
        /// </summary>
        public string StatusCode { get; } = "";

        /// <summary>
        /// Gets POP3 server response text after status code.
        /// </summary>
        public string ResponseText { get; } = "";
    }
}
