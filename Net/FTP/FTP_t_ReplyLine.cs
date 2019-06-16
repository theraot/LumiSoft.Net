﻿using System;

namespace LumiSoft.Net.FTP
{
    /// <summary>
    /// This class represent s FTP server reply-line.
    /// </summary>
    public class FTP_t_ReplyLine
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="replyCode">FTP server reply code.</param>
        /// <param name="text">FTP server reply text.</param>
        /// <param name="isLastLine">Specifies if this line is last line in response.</param>
        public FTP_t_ReplyLine(int replyCode, string text, bool isLastLine)
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
        /// Parses FTP reply-line from 
        /// </summary>
        /// <param name="line">FTP server reply-line.</param>
        /// <returns>Returns parsed FTP server reply-line.</returns>
        /// <exception cref="ArgumentNullException">Is raised when <b>line</b> is null reference.</exception>
        /// <exception cref="ParseException">Is raised when reply-line parsing fails.</exception>
        public static FTP_t_ReplyLine Parse(string line)
        {
            if (line == null)
            {
                throw new ArgumentNullException("line");
            }
            if (line.Length < 3)
            {
                throw new ParseException("Invalid FTP server reply-line '" + line + "'.");
            }

            int replyCode = 0;
            if (!int.TryParse(line.Substring(0, 3), out replyCode))
            {
                throw new ParseException("Invalid FTP server reply-line '" + line + "' reply-code.");
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

            return new FTP_t_ReplyLine(replyCode, text, isLastLine);
        }

        /// <summary>
        /// Returns this as FTP server <b>reply-line</b>.
        /// </summary>
        /// <returns>Returns this as FTP server <b>reply-line</b>.</returns>
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
