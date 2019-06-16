using System;

namespace LumiSoft.Net.SMTP.Client
{
    /// <summary>
    /// SMTP client exception.
    /// </summary>
    public class SMTP_ClientException : Exception
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="responseLine">SMTP server response line.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>responseLine</b> is null.</exception>
        public SMTP_ClientException(string responseLine) : base(responseLine.TrimEnd())
        {
            if(responseLine == null){
                throw new ArgumentNullException("responseLine");
            }

            ReplyLines = new SMTP_t_ReplyLine[]{SMTP_t_ReplyLine.Parse(responseLine)};
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="replyLines">SMTP server error reply lines.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>replyLines</b> is null reference.</exception>
        public SMTP_ClientException(SMTP_t_ReplyLine[] replyLines) : base(replyLines[0].ToString().TrimEnd())
        {
            ReplyLines = replyLines ?? throw new ArgumentNullException("replyLines");            
        }

        /// <summary>
        /// Gets SMTP status code.
        /// </summary>
        [Obsolete("Use property 'ReplyLines' insead.")]
        public int StatusCode
        {
            get{ return ReplyLines[0].ReplyCode; }
        }

        /// <summary>
        /// Gets SMTP server response text after status code.
        /// </summary>
        [Obsolete("Use property 'ReplyLines' insead.")]
        public string ResponseText
        {
            get{ return ReplyLines[0].Text; }
        }

        /// <summary>
        /// Gets SMTP server error reply lines.
        /// </summary>
        public SMTP_t_ReplyLine[] ReplyLines { get; }

        /// <summary>
        /// Gets if it is permanent SMTP(5xx) error.
        /// </summary>
        public bool IsPermanentError
        {
            get
            {
                if(ReplyLines[0].ReplyCode >= 500 && ReplyLines[0].ReplyCode <= 599){
                    return true;
                }

                return false;
            }
        }
    }
}
