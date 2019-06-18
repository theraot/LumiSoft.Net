using System;

namespace LumiSoft.Net.IO
{
    /// <summary>
    /// The exception that is thrown when maximum allowed data size has exceeded.
    /// </summary>
    public class DataSizeExceededException : Exception
    {
        public DataSizeExceededException()
        {
            // Empty
        }

        public DataSizeExceededException(string message)
            : base(message)
        {
            // Empty
        }

        public DataSizeExceededException(string message, Exception innerException)
            : base(message, innerException)
        {
            // Empty
        }
    }
}
