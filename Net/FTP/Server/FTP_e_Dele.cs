using System;

namespace LumiSoft.Net.FTP.Server
{
    /// <summary>
    /// This class provides data for <see cref="FTP_Session.Dele"/> event.
    /// </summary>
    public class FTP_e_Dele : EventArgs
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="fileName">File name with optional path.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>fileName</b> is null reference.</exception>
        public FTP_e_Dele(string fileName)
        {
            FileName = fileName ?? throw new ArgumentNullException("fileName");
        }

        /// <summary>
        /// Gets file name with optional path.
        /// </summary>
        public string FileName { get; }

        /// <summary>
        /// Gets or sets FTP server response.
        /// </summary>
        public FTP_t_ReplyLine[] Response { get; set; }
    }
}
