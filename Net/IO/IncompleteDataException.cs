using System;

namespace LumiSoft.Net.IO
{
    /// <summary>
    /// The exception that is thrown when incomplete data received.
    /// For example for ReadPeriodTerminated() method reaches end of stream before getting period terminator.
    /// </summary>
    public class IncompleteDataException : Exception
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public IncompleteDataException()
        {
            // Empty
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="message">Exception message text.</param>
        public IncompleteDataException(string message)
            : base(message)
        {
            // Empty
        }

        public IncompleteDataException(string message, Exception innerException)
            : base(message, innerException)
        {
            // Empty
        }
    }
}
