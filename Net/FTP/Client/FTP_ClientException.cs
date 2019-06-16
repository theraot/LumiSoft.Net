using System;

namespace LumiSoft.Net.FTP.Client
{
    /// <summary>
    /// FTP client exception.
    /// </summary>
    public class FTP_ClientException : Exception
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="responseLine">FTP server response line.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>responseLine</b> is null.</exception>
        public FTP_ClientException(string responseLine) : base(responseLine)
        {
            if(responseLine == null){
                throw new ArgumentNullException("responseLine");
            }

            string[] code_text = responseLine.Split(new char[]{' '},2);
            try{
                StatusCode = Convert.ToInt32(code_text[0]);
            }
            catch{
            }
            if(code_text.Length == 2){
                ResponseText =  code_text[1];                
            }
        }

        /// <summary>
        /// Gets FTP status code.
        /// </summary>
        public int StatusCode { get; } = 500;

        /// <summary>
        /// Gets FTP server response text after status code.
        /// </summary>
        public string ResponseText { get; } = "";

        /// <summary>
        /// Gets if it is permanent FTP(5xx) error.
        /// </summary>
        public bool IsPermanentError
        {
            get{
                if(StatusCode >= 500 && StatusCode <= 599){
                    return true;
                }
                else{
                    return false;
                }
            }
        }
    }
}
