using System;
using System.Net;

namespace LumiSoft.Net.SIP.Stack
{
    /// <summary>
    /// This class provides data for SIP_Stack.ValidateRequest event.
    /// </summary>
    public class SIP_ValidateRequestEventArgs : EventArgs
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="request">Incoming SIP request.</param>
        /// <param name="remoteEndpoint">IP end point what made request.</param>
        public SIP_ValidateRequestEventArgs(SIP_Request request,IPEndPoint remoteEndpoint)
        {
            Request        = request;
            RemoteEndPoint = remoteEndpoint;
        }


        /// <summary>
        /// Gets incoming SIP request.
        /// </summary>
        public SIP_Request Request { get; }

        /// <summary>
        /// Gets IP end point what made request.
        /// </summary>
        public IPEndPoint RemoteEndPoint { get; }

        /// <summary>
        /// Gets or sets response code. Value null means SIP stack will handle it.
        /// </summary>
        public string ResponseCode { get; set; }
    }
}
