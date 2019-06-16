using System;

namespace LumiSoft.Net
{
    /// <summary>
    /// Implements absolute-URI. Defined in RFC 3986.4.3.
    /// </summary>
    public class AbsoluteUri
    {
        private string m_Scheme = "";
        private string m_Value = "";

        /// <summary>
        /// Default constructor.
        /// </summary>
        internal AbsoluteUri()
        {
        }

        /// <summary>
        /// Gets URI scheme.
        /// </summary>
        public virtual string Scheme
        {
            get { return m_Scheme; }
        }

        /// <summary>
        /// Gets URI value after scheme.
        /// </summary>
        public string Value
        {
            get { return ToString().Split(new[] { ':' }, 2)[1]; }
        }

        /// <summary>
        /// Parse URI from string value.
        /// </summary>
        /// <param name="value">String URI value.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>value</b> is null reference.</exception>
        /// <exception cref="ArgumentException">Is raised when <b>value</b> has invalid URI value.</exception>
        public static AbsoluteUri Parse(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            if (value == "")
            {
                throw new ArgumentException("Argument 'value' value must be specified.");
            }

            var scheme_value = value.Split(new[] { ':' }, 2);
            if (scheme_value[0].ToLower() == UriSchemes.sip || scheme_value[0].ToLower() == UriSchemes.sips)
            {
                var uri = new SIP_Uri();
                uri.ParseInternal(value);

                return uri;
            }
            else
            {
                var uri = new AbsoluteUri();
                uri.ParseInternal(value);

                return uri;
            }
        }

        /// <summary>
        /// Converts URI to string.
        /// </summary>
        /// <returns>Returns URI as string.</returns>
        public override string ToString()
        {
            return m_Scheme + ":" + m_Value;
        }

        /// <summary>
        /// Parses URI from the specified string.
        /// </summary>
        /// <param name="value">URI string.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>value</b> is null reference.</exception>
        protected virtual void ParseInternal(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            var scheme_value = value.Split(new[] { ':' }, 1);
            m_Scheme = scheme_value[0].ToLower();
            if (scheme_value.Length == 2)
            {
                m_Value = scheme_value[1];
            }
        }
    }
}
