using System;
using System.Text;

namespace LumiSoft.Net.SIP.Message
{
    /// <summary>
    /// Implements SIP_t_NameAddress + parameters value.
    /// </summary>
    public class SIP_t_AddressParam : SIP_t_ValueWithParams
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public SIP_t_AddressParam()
        {
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="value">SIP_t_NameAddress + parameters value.</param>
        public SIP_t_AddressParam(string value)
        {
            Parse(value);
        }

        /// <summary>
        /// Gets address.
        /// </summary>
        public SIP_t_NameAddress Address { get; private set; }

        /// <summary>
        /// Parses this from specified value.
        /// </summary>
        /// <param name="value">Address + params value.</param>
        /// <exception cref="ArgumentNullException">Raised when <b>value</b> is null.</exception>
        /// <exception cref="SIP_ParseException">Raised when invalid SIP message.</exception>
        public void Parse(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            Parse(new StringReader(value));
        }

        /// <summary>
        /// Parses this from address param string.
        /// </summary>
        /// <param name="reader">Reader what contains address param string.</param>
        /// <exception cref="ArgumentNullException">Raised when <b>reader</b> is null.</exception>
        /// <exception cref="SIP_ParseException">Raised when invalid SIP message.</exception>
        public override void Parse(StringReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }

            // Parse address
            var address = new SIP_t_NameAddress();
            address.Parse(reader);
            Address = address;

            // Parse parameters.
            ParseParameters(reader);
        }

        /// <summary>
        /// Converts this to valid value string.
        /// </summary>
        /// <returns></returns>
        public override string ToStringValue()
        {
            var retVal = new StringBuilder();

            // Add address
            retVal.Append(Address.ToStringValue());

            // Add parameters
            foreach (SIP_Parameter parameter in Parameters)
            {
                if (parameter.Value != null)
                {
                    retVal.Append(";" + parameter.Name + "=" + parameter.Value);
                }
                else
                {
                    retVal.Append(";" + parameter.Name);
                }
            }

            return retVal.ToString();
        }
    }
}
