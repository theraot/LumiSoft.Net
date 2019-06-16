using System;
using System.IO;

namespace LumiSoft.Net.FTP.Server
{
    /// <summary>
    /// This class provides data for <see cref="FTP_Session.Stor"/> event.
    /// </summary>
    public class FTP_e_Stor : EventArgs
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="file">File name with option path.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>file</b> is null reference.</exception>
        /// <exception cref="ArgumentException">Is raised when any of the arguments has invalid value.</exception>
        public FTP_e_Stor(string file)
        {
            if(file == null){
                throw new ArgumentNullException("file");
            }
            if(file == string.Empty){
                throw new ArgumentException("Argument 'file' name must be specified.","file");
            }

            FileName = file;
        }


        #region Properties implementation

        /// <summary>
        /// Gets or sets error response.
        /// </summary>
        public FTP_t_ReplyLine[] Error { get; set; }

        /// <summary>
        /// Gets file name with optional path.
        /// </summary>
        public string FileName { get; }

        /// <summary>
        /// Gets or sets file stream.
        /// </summary>
        public Stream FileStream { get; set; }

#endregion
    }
}
