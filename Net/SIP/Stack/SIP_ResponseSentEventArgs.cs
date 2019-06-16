using System;

namespace LumiSoft.Net.SIP.Stack
{
    /// <summary>
    /// This class provides data for <b>SIP_ServerTransaction.ResponseSent</b> method.
    /// </summary>
    public class SIP_ResponseSentEventArgs : EventArgs
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="transaction">Server transaction.</param>
        /// <param name="response">SIP response.</param>
        /// <exception cref="ArgumentNullException">Is raised when any of the arguments is null.</exception>
        public SIP_ResponseSentEventArgs(SIP_ServerTransaction transaction, SIP_Response response)
        {
            ServerTransaction = transaction ?? throw new ArgumentNullException("transaction");
            Response = response ?? throw new ArgumentNullException("response");
        }

        /// <summary>
        /// Gets response which was sent.
        /// </summary>
        public SIP_Response Response { get; }

        /// <summary>
        /// Gets server transaction which sent response.
        /// </summary>
        public SIP_ServerTransaction ServerTransaction { get; }
    }
}
