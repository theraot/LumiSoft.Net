﻿using System;

namespace LumiSoft.Net.SIP.Stack
{
    /// <summary>
    /// Implements SIP Request-Line. Defined in RFC 3261.
    /// </summary>
    public class SIP_RequestLine
    {
        private string m_Method = "";
        private AbsoluteUri m_pUri;
        private string m_Version = "";

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="method">SIP method.</param>
        /// <param name="uri">Request URI.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>method</b> or <b>uri</b> is null reference.</exception>
        /// <exception cref="ArgumentException">Is raised when any of the arguments has invalid value.</exception>
        public SIP_RequestLine(string method, AbsoluteUri uri)
        {
            if (method == null)
            {
                throw new ArgumentNullException("method");
            }
            if (!SIP_Utils.IsToken(method))
            {
                throw new ArgumentException("Argument 'method' value must be token.");
            }

            m_Method = method.ToUpper();
            m_pUri = uri ?? throw new ArgumentNullException("uri");
            m_Version = "SIP/2.0";
        }

        /// <summary>
        /// Gets or sets request method. This value is always in upper-case.
        /// </summary>
        /// <exception cref="ArgumentNullException">Is raised when <b>value</b> is null reference.</exception>
        /// <exception cref="ArgumentException">Is raised when <b>value</b> has invalid value.</exception>
        public string Method
        {
            get => m_Method;

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("Method");
                }
                if (!SIP_Utils.IsToken(value))
                {
                    throw new ArgumentException("Property 'Method' value must be token.");
                }

                m_Method = value.ToUpper();
            }
        }

        /// <summary>
        /// Gets or sets request URI.
        /// </summary>
        /// <exception cref="ArgumentNullException">Is raised when <b>value</b> is null reference.</exception>
        public AbsoluteUri Uri
        {
            get => m_pUri;

            set => m_pUri = value ?? throw new ArgumentNullException("Uri");
        }

        /// <summary>
        /// Gets or sets SIP version.
        /// </summary>
        /// <exception cref="ArgumentNullException">Is raised when <b>value</b> is null reference.</exception>
        /// <exception cref="ArgumentException">Is raised when <b>value</b> has invalid value.</exception>
        public string Version
        {
            get => m_Version;

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("Version");
                }
                if (value == "")
                {
                    throw new ArgumentException("Property 'Version' value must be specified.");
                }

                m_Version = value;
            }
        }

        /// <summary>
        /// Returns Request-Line string.
        /// </summary>
        /// <returns>Returns Request-Line string.</returns>
        public override string ToString()
        {
            // RFC 3261 25.
            //  Request-Line = Method SP Request-URI SP SIP-Version CRLF

            return m_Method + " " + m_pUri.ToString() + " " + m_Version + "\r\n";
        }
    }
}
