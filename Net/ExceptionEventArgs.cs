using System;

namespace LumiSoft.Net
{
    /// <summary>
    /// This class provides data for error events and methods.
    /// </summary>
    public class ExceptionEventArgs : EventArgs
    {
        private readonly Exception m_pException;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="exception">Exception.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>exception</b> is null reference value.</exception>
        public ExceptionEventArgs(Exception exception)
        {
            if(exception == null){
                throw new ArgumentNullException("exception");
            }

            m_pException = exception;
        }


        #region Properties implementation

        /// <summary>
        /// Gets exception.
        /// </summary>
        public Exception Exception
        {
            get{ return m_pException; }
        }

        #endregion

    }
}
