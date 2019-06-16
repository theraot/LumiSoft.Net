using System;

namespace LumiSoft.Net.Log
{
    /// <summary>
    /// This class provides data for <b>Logger.WriteLog</b> event.
    /// </summary>
    public class WriteLogEventArgs : EventArgs
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="logEntry">New log entry.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>logEntry</b> is null.</exception>
        public WriteLogEventArgs(LogEntry logEntry)
        {
            LogEntry = logEntry ?? throw new ArgumentNullException("logEntry");
        }

        /// <summary>
        /// Gets new log entry.
        /// </summary>
        public LogEntry LogEntry { get; }
    }
}
