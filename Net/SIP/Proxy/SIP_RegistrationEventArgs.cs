using System;

namespace LumiSoft.Net.SIP.Proxy
{
    /// <summary>
    /// This class provides data for <b>SIP_Registrar.AorRegistered</b>,<b>SIP_Registrar.AorUnregistered</b> and <b>SIP_Registrar.AorUpdated</b> event.
    /// </summary>
    public class SIP_RegistrationEventArgs : EventArgs
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="registration">SIP reggistration.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>registration</b> is null reference.</exception>
        public SIP_RegistrationEventArgs(SIP_Registration registration)
        {
            if(registration == null){
                throw new ArgumentNullException("registration");
            }

            Registration = registration;
        }

        /// <summary>
        /// Gets SIP registration.
        /// </summary>
        public SIP_Registration Registration { get; }
    }
}
