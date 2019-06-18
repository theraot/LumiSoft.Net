using System;
using System.Diagnostics;

namespace LumiSoft.Net
{
    /// <summary>
    /// Provides data for the SysError event for servers.
    /// </summary>
    public class ErrorEventArgs
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="stackTrace"></param>
        public ErrorEventArgs(Exception x,StackTrace stackTrace)
        {
            Exception  = x;
            StackTrace = stackTrace;
        }

        /// <summary>
        /// Occured error's exception.
        /// </summary>
        public Exception Exception { get; }

        /// <summary>
        /// Occured error's stacktrace.
        /// </summary>
        public StackTrace StackTrace { get; }

        /// <summary>
        /// Gets comment text.
        /// </summary>
        public string Text { get; } = "";
    }
}
