using System;

namespace LumiSoft.Net.SIP.Message
{
    /// <summary>
    /// Implements SIP "option-tag" value. Defined in RFC 3261.
    /// </summary>
    /// <remarks>
    /// <code>
    /// RFC 3261 Syntax:
    ///     option-tag = token
    /// </code>
    /// </remarks>
    public class SIP_t_OptionTag : SIP_t_Value
    {
        private string m_OptionTag = "";

        /// <summary>
        /// Gets or sets option tag.
        /// </summary>
        public string OptionTag
        {
            get => m_OptionTag;

            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentException("property OptionTag value cant be null or empty !");
                }

                m_OptionTag = value;
            }
        }

        /// <summary>
        /// Parses "option-tag" from specified value.
        /// </summary>
        /// <param name="value">SIP "option-tag" value.</param>
        /// <exception cref="ArgumentNullException">Raised when <b>reader</b> is null.</exception>
        /// <exception cref="SIP_ParseException">Raised when invalid SIP message.</exception>
        public void Parse(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("reader");
            }

            Parse(new StringReader(value));
        }

        /// <summary>
        /// Parses "option-tag" from specified reader.
        /// </summary>
        /// <param name="reader">Reader from where to parse.</param>
        /// <exception cref="ArgumentNullException">Raised when <b>reader</b> is null.</exception>
        /// <exception cref="SIP_ParseException">Raised when invalid SIP message.</exception>
        public override void Parse(StringReader reader)
        {
            // option-tag = token

            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }

            // Get Method
            var word = reader.ReadWord();
            m_OptionTag = word ?? throw new ArgumentException("Invalid 'option-tag' value, value is missing !");
        }

        /// <summary>
        /// Converts this to valid "option-tag" value.
        /// </summary>
        /// <returns>Returns "option-tag" value.</returns>
        public override string ToStringValue()
        {
            return m_OptionTag;
        }
    }
}
