using System;

namespace LumiSoft.Net.SMTP.Relay
{
    /// <summary>
    /// This class provides data for <b>Relay_Server.SessionCompleted</b> event.
    /// </summary>
    public class Relay_SessionCompletedEventArgs
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="session">Relay session what completed processing.</param>
        /// <param name="exception">Exception what happened or null if relay completed successfully.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>session</b> is null.</exception>
        public Relay_SessionCompletedEventArgs(Relay_Session session,Exception exception)
        {
            if(session == null){
                throw new ArgumentNullException("session");
            }

            Session   = session;
            Exception = exception;
        }

        /// <summary>
        /// Gets relay session what completed processing.
        /// </summary>
        public Relay_Session Session { get; }

        /// <summary>
        /// Gets Exception what happened or null if relay completed successfully.
        /// </summary>
        public Exception Exception { get; }
    }
}
