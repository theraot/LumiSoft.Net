using System;
using System.Text;

namespace LumiSoft.Net.SMTP.Server
{
    /// <summary>
    /// This class implements SMTP server reply.
    /// </summary>
    public class SMTP_Reply
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="replyCode">SMTP server reply code.</param>
        /// <param name="replyLine">SMTP server reply line.</param>
        /// <exception cref="ArgumentException">Is raised when any of the arguments has invalid value.</exception>
        /// <exception cref="ArgumentNullException">Is raised when <b>replyLine</b> is null reference.</exception>
        public SMTP_Reply(int replyCode,string replyLine) : this(replyCode,new[]{replyLine})
        {
            if(replyLine == null){
                throw new ArgumentNullException("replyLine");
            }
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="replyCode">SMTP server reply code.</param>
        /// <param name="replyLines">SMTP server reply line(s).</param>
        /// <exception cref="ArgumentException">Is raised when any of the arguments has invalid value.</exception>
        /// <exception cref="ArgumentNullException">Is raised when <b>replyLines</b> is null reference.</exception>
        public SMTP_Reply(int replyCode,string[] replyLines)
        {
            if(replyCode < 200 || replyCode > 599){
                throw new ArgumentException("Argument 'replyCode' value must be >= 200 and <= 599.","replyCode");
            }
            if(replyLines == null){
                throw new ArgumentNullException("replyLines");
            }
            if(replyLines.Length == 0){
                throw new ArgumentException("Argument 'replyLines' must conatin at least one line.","replyLines");
            }

            ReplyCode   = replyCode;
            ReplyLines = replyLines;
        }

        /// <summary>
        /// Returns SMTP server reply as string.
        /// </summary>
        /// <returns>Returns SMTP server reply as string.</returns>
        public override string ToString()
        {
            var retVal = new StringBuilder();
            for (int i=0;i<ReplyLines.Length;i++){
                // Last line.
                if(i == (ReplyLines.Length - 1)){
                    retVal.Append(ReplyCode + " " + ReplyLines[i] + "\r\n");
                }
                else{
                    retVal.Append(ReplyCode + "-" + ReplyLines[i] + "\r\n");
                }
            }

            return retVal.ToString(); 
        }

        /// <summary>
        /// Gets SMTP server reply code.
        /// </summary>
        public int ReplyCode { get; }

        /// <summary>
        /// Gets SMTP server reply lines.
        /// </summary>
        public string[] ReplyLines { get; }
    }
}
