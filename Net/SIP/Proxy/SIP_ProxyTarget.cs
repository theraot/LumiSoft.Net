using System;

using LumiSoft.Net.SIP.Stack;

namespace LumiSoft.Net.SIP.Proxy
{
    /// <summary>
    /// Represents SIP proxy target in the SIP proxy "target set". Defined in RFC 3261 16.
    /// </summary>
    public class SIP_ProxyTarget
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="targetUri">Target request-URI.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>targetUri</b> is null reference.</exception>
        public SIP_ProxyTarget(SIP_Uri targetUri) : this(targetUri, null)
        {
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="targetUri">Target request-URI.</param>
        /// <param name="flow">Data flow to try for forwarding.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>targetUri</b> is null reference.</exception>
        public SIP_ProxyTarget(SIP_Uri targetUri, SIP_Flow flow)
        {
            TargetUri = targetUri ?? throw new ArgumentNullException("targetUri");
            Flow = flow;
        }

        /// <summary>
        /// Gets data flow. Value null means that new flow must created.
        /// </summary>
        public SIP_Flow Flow { get; }

        /// <summary>
        /// Gets target URI.
        /// </summary>
        public SIP_Uri TargetUri { get; }
    }
}
