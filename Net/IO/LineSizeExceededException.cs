using System;

namespace LumiSoft.Net.IO
{
    /// <summary>
    /// The exception that is thrown when maximum allowed line size has exceeded.
    /// </summary>
    public class LineSizeExceededException : Exception
    {
        public LineSizeExceededException()
        {
            // Empty
        }

        public LineSizeExceededException(string message)
            : base(message)
        {
            // Empty
        }

        public LineSizeExceededException(string message, Exception innerException)
            : base(message, innerException)
        {
            // Empty
        }
    }
}
