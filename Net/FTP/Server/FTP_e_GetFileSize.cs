using System;

namespace LumiSoft.Net.FTP.Server
{
    /// <summary>
    /// This class provides data for <see cref="FTP_Session.GetFileSize"/> event.
    /// </summary>
    public class FTP_e_GetFileSize : EventArgs
    {
        private long              m_FileSize;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="fileName">File name with optional path.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>fileName</b> is null reference.</exception>
        public FTP_e_GetFileSize(string fileName)
        {
            if(fileName == null){
                throw new ArgumentNullException("fileName");
            }

            FileName = fileName;
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
        /// Gets or sets file size in bytes.
        /// </summary>
        public long FileSize
        {
            get{ return m_FileSize; }

            set{
                if(value < 0){
                    throw new ArgumentException("Property 'FileSize' value must be >= 0.","FileSize");
                }

                m_FileSize = value; 
            }
        }

        #endregion
    }
}
