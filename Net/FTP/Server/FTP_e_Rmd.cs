using System;

namespace LumiSoft.Net.FTP.Server
{
    /// <summary>
    /// This class provides data for <see cref="FTP_Session.Rmd"/> event.
    /// </summary>
    public class FTP_e_Rmd : EventArgs
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="dirName">Directory name with optional path.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>dirName</b> is null reference.</exception>
        public FTP_e_Rmd(string dirName)
        {
            DirName = dirName ?? throw new ArgumentNullException("dirName");
        }

        /// <summary>
        /// Gets directory name with optional path.
        /// </summary>
        public string DirName { get; }

        /// <summary>
        /// Gets or sets FTP server response.
        /// </summary>
        public FTP_t_ReplyLine[] Response { get; set; }
    }
}
