using System;

namespace LumiSoft.Net.FTP.Server
{
    /// <summary>
    /// This class provides data for <see cref="FTP_Session.Mkd"/> event.
    /// </summary>
    public class FTP_e_Mkd : EventArgs
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="dirName">Directory name with optional path.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>dirName</b> is null reference.</exception>
        public FTP_e_Mkd(string dirName)
        {
            if(dirName == null){
                throw new ArgumentNullException("dirName");
            }

            DirName = dirName;
        }

        /// <summary>
        /// Gets or sets FTP server response.
        /// </summary>
        public FTP_t_ReplyLine[] Response { get; set; }

        /// <summary>
        /// Gets directory name with optional path.
        /// </summary>
        public string DirName { get; }
    }
}
