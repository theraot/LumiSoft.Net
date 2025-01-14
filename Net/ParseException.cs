﻿#pragma warning disable CA2237 // Mark ISerializable types with serializable

using System;

namespace LumiSoft.Net
{
    /// <inheritdoc />
    /// <summary>
    ///     This exception is thrown when parse errors are encountered.
    /// </summary>
    public class ParseException : Exception
    {
        public ParseException()
        {
            // Empty
        }

        /// <inheritdoc />
        /// <summary>
        ///     Default constructor.
        /// </summary>
        /// <param name="message"></param>
        public ParseException(string message)
            : base(message)
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