using System;

namespace LumiSoft.Net.FTP.Server
{
    /// <summary>
    /// This class provides data for <see cref="FTP_Session.Rnto"/> event.
    /// </summary>
    public class FTP_e_Rnto : EventArgs
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="sourcePath">Source file or directory path.</param>
        /// <param name="targetPath">Target file or directory path.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>sourcePath</b> or <b>targetPath</b> is null reference.</exception>
        public FTP_e_Rnto(string sourcePath,string targetPath)
        {
            SourcePath = sourcePath ?? throw new ArgumentNullException("sourcePath");
            TargetPath = targetPath ?? throw new ArgumentNullException("targetPath");
        }

        /// <summary>
        /// Gets or sets FTP server response.
        /// </summary>
        public FTP_t_ReplyLine[] Response { get; set; }

        /// <summary>
        /// Gets source path.
        /// </summary>
        public string SourcePath { get; }

        /// <summary>
        /// Gets target path.
        /// </summary>
        public string TargetPath { get; }
    }
}
