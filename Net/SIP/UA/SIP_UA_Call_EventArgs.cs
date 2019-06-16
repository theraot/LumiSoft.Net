using System;

namespace LumiSoft.Net.SIP.UA
{
    /// <summary>
    /// This class provides data for <b>SIP_UA.IncomingCall</b> event.
    /// </summary>
    [Obsolete("Use SIP stack instead.")]
    public class SIP_UA_Call_EventArgs : EventArgs
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="call">SIP UA call.</param>
        /// <exception cref="ArgumentNullException">Is called when <b>call</b> is null reference.</exception>
        public SIP_UA_Call_EventArgs(SIP_UA_Call call)
        {
            if(call == null){
                throw new ArgumentNullException("call");
            }

            Call = call;
        }


        #region Properties implementation

        /// <summary>
        /// Gets call.
        /// </summary>
        public SIP_UA_Call Call { get; }

#endregion

    }
}
