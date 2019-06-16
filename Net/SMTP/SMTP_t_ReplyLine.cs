﻿using System;

namespace LumiSoft.Net.SMTP
{
    /// <summary>
    /// This class represent s SMTP server reply-line. Defined in RFC 5321 4.2.
    /// </summary>
    public class SMTP_t_ReplyLine
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="replyCode">SMTP server reply code.</param>
        /// <param name="text">SMTP server reply text.</param>
        /// <param name="isLastLine">Specifies if this line is last line in response.</param>
        public SMTP_t_ReplyLine(int replyCode, string text, bool isLastLine)
        {
            if (text == null)
            {
                text = "";
            }

            ReplyCode = replyCode;
            Text = text;
            IsLastLine = isLastLine;
        }

        /// <summary>
        /// Gets if this is last reply line.
        /// </summary>
        public bool IsLastLine { get; } = true;

        /// <summary>
        /// Gets SMTP server reply code.
        /// </summary>
        public int ReplyCode { get; }

        /// <summary>
        /// Gets SMTP server relpy text.
        /// </summary>
        public string Text { get; }

        /// <summary>
        /// Parses SMTP reply-line from
        /// </summary>
        /// <param name="line">SMTP server reply-line.</param>
        /// <returns>Returns parsed SMTP server reply-line.</returns>
        /// <exception cref="ArgumentNullException">Is raised when <b>line</b> is null reference.</exception>
        /// <exception cref="ParseException">Is raised when reply-line parsing fails.</exception>
        public static SMTP_t_ReplyLine Parse(string line)
        {
            if (line == null)
            {
                throw new ArgumentNullException("line");
            }

            /* RFC 5321 4.2.
                Reply-line     = *( Reply-code "-" [ textstring ] CRLF )
                                 Reply-code [ SP textstring ] CRLF
             
                Since, in violation of this specification, the text is sometimes not sent, clients that do not
                receive it SHOULD be prepared to process the code alone (with or without a trailing space character).
            */

            if (line.Length < 3)
            {
                throw new ParseException("Invalid SMTP server reply-line '" + line + "'.");
            }

            int replyCode = 0;
            if (!int.TryParse(line.Substring(0, 3), out replyCode))
            {
                throw new ParseException("Invalid SMTP server reply-line '" + line + "' reply-code.");
            }

            bool isLastLine = true;
            if (line.Length > 3)
            {
                isLastLine = (line[3] == ' ');
            }

            var text = "";
            if (line.Length > 5)
            {
                text = line.Substring(4);
            }

            return new SMTP_t_ReplyLine(replyCode, text, isLastLine);
        }

        /// <summary>
        /// Returns this as SMTP server <b>reply-line</b>.
        /// </summary>
        /// <returns>Returns this as SMTP server <b>reply-line</b>.</returns>
        public override string ToString()
        {
            if (IsLastLine)
            {
                return ReplyCode.ToString() + " " + Text + "\r\n";
            }

            return ReplyCode.ToString() + "-" + Text + "\r\n";
        }
    }
}
