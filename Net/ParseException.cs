using System;

namespace LumiSoft.Net
{
    /// <summary>
    /// This exception is thrown when parse errors are encountered.
    /// </summary>
    public class ParseException : Exception
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="message"></param>
        public ParseException(string message) 
            : base(message)
        {
            // Empty
        }

        public ParseException()
            : base()
        {
            // Empty
        }

        public ParseException(string message, Exception innerException)
            : base(message, innerException)
        {
            // Empty
        }
    }
}
