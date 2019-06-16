using System;

namespace LumiSoft.Net.SIP.Stack
{
    /// <summary>
    /// This class provides data for ResponseReceived events.
    /// </summary>
    public class SIP_ResponseReceivedEventArgs : EventArgs
    {
        private readonly SIP_Stack             m_pStack;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="stack">Reference to SIP stack.</param>
        /// <param name="transaction">Client transaction what response it is. This value can be null if no matching client response.</param>
        /// <param name="response">Received response.</param>
        internal SIP_ResponseReceivedEventArgs(SIP_Stack stack,SIP_ClientTransaction transaction,SIP_Response response)
        {
            m_pStack       = stack;
            Response    = response;
            ClientTransaction = transaction;
        }


        /// <summary>
        /// Gets response received by SIP stack.
        /// </summary>
        public SIP_Response Response { get; }

        /// <summary>
        /// Gets client transaction which response it is. This value is null if no matching client transaction.
        /// If this core is staless proxy then it's allowed, otherwise core MUST discard that response.
        /// </summary>
        public SIP_ClientTransaction ClientTransaction { get; }

        /// <summary>
        /// Gets SIP dialog where Response belongs to. Returns null if Response doesn't belong any dialog.
        /// </summary>
        public SIP_Dialog Dialog
        {
            get{ return m_pStack.TransactionLayer.MatchDialog(Response); }
        }

        /// <summary>
        /// Gets or creates dialog.
        /// </summary>
        /// <exception cref="InvalidOperationException">Is raised when the specified request method can't establish dialog or
        /// response has no To-Tag.</exception>
        public SIP_Dialog GetOrCreateDialog
        {
            get{
                if(!SIP_Utils.MethodCanEstablishDialog(ClientTransaction.Method)){
                    throw new InvalidOperationException("Request method '" + ClientTransaction.Method + "' can't establish dialog.");
                }
                if(Response.To.Tag == null){
                    throw new InvalidOperationException("Request To-Tag is missing.");
                }
 
                return m_pStack.TransactionLayer.GetOrCreateDialog(ClientTransaction,Response); 
            }
        }
    }
}
