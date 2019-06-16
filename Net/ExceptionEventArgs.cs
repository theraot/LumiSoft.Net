using System;

namespace LumiSoft.Net
{
    /// <summary>
    /// This class provides data for error events and methods.
    /// </summary>
    public class ExceptionEventArgs : EventArgs
    {
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

            Exception = exception;
        }

        /// <summary>
        /// Gets exception.
        /// </summary>
        public Exception Exception { get; }
    }
}
