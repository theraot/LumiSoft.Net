using System;
using System.Text;

namespace LumiSoft.Net.SIP.Message
{
    /// <summary>
    /// Implements SIP "reason-value" value. Defined in rfc 3326.
    /// </summary>
    /// <remarks>
    /// <code>
    /// RFC 3326 Syntax:
    ///     reason-value      =  protocol *(SEMI reason-params)
    ///     protocol          =  "SIP" / "Q.850" / token
    ///     reason-params     =  protocol-cause / reason-text / reason-extension
    ///     protocol-cause    =  "cause" EQUAL cause
    ///     cause             =  1*DIGIT
    ///     reason-text       =  "text" EQUAL quoted-string
    ///     reason-extension  =  generic-param
    /// </code>
    /// </remarks>
    public class SIP_t_ReasonValue : SIP_t_ValueWithParams
    {
        private string m_Protocol = "";

        /// <summary>
        /// Default constructor.
        /// </summary>
        public SIP_t_ReasonValue()
        {
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="value">SIP reason-value value.</param>
        public SIP_t_ReasonValue(string value)
        {
            Parse(value);
        }

        /// <summary>
        /// Gets or sets 'cause' parameter value. The cause parameter contains a SIP status code.
        /// Value -1 means not specified.
        /// </summary>
        public int Cause
        {
            get
            {
                if (Parameters["cause"] == null)
                {
                    return -1;
                }

                return Convert.ToInt32(Parameters["cause"].Value);
            }

            set
            {
                if (value < 0)
                {
                    Parameters.Remove("cause");
                }
                else
                {
                    Parameters.Set("cause", value.ToString());
                }
            }
        }

        /// <summary>
        /// Gets or sets protocol.
        /// </summary>
        /// <exception cref="ArgumentNullException">Is raised when null value is passed.</exception>
        public string Protocol
        {
            get { return m_Protocol; }

            set
            {
                m_Protocol = value ?? throw new ArgumentNullException("Protocol");
            }
        }

        /// <summary>
        /// Gets or sets 'text' parameter value. Value null means not specified.
        /// </summary>
        public string Text
        {
            get
            {
                var parameter = Parameters["text"];
                return parameter?.Value;
            }

            set
            {
                if (value == null)
                {
                    Parameters.Remove("text");
                }
                else
                {
                    Parameters.Set("text", value);
                }
            }
        }

        /// <summary>
        /// Parses "reason-value" from specified value.
        /// </summary>
        /// <param name="value">SIP "reason-value" value.</param>
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
        /// Parses "reason-value" from specified reader.
        /// </summary>
        /// <param name="reader">Reader from where to parse.</param>
        /// <exception cref="ArgumentNullException">Raised when <b>reader</b> is null.</exception>
        /// <exception cref="SIP_ParseException">Raised when invalid SIP message.</exception>
        public override void Parse(StringReader reader)
        {
            /*
                reason-value      =  protocol *(SEMI reason-params)
                protocol          =  "SIP" / "Q.850" / token
                reason-params     =  protocol-cause / reason-text / reason-extension
                protocol-cause    =  "cause" EQUAL cause
                cause             =  1*DIGIT
                reason-text       =  "text" EQUAL quoted-string
                reason-extension  =  generic-param
            */

            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }

            // protocol
            var word = reader.ReadWord();
            m_Protocol = word ?? throw new SIP_ParseException("SIP reason-value 'protocol' value is missing !");

            // Parse parameters
            ParseParameters(reader);
        }

        /// <summary>
        /// Converts this to valid "reason-value" value.
        /// </summary>
        /// <returns>Returns "reason-value" value.</returns>
        public override string ToStringValue()
        {
            /*
                reason-value      =  protocol *(SEMI reason-params)
                protocol          =  "SIP" / "Q.850" / token
                reason-params     =  protocol-cause / reason-text / reason-extension
                protocol-cause    =  "cause" EQUAL cause
                cause             =  1*DIGIT
                reason-text       =  "text" EQUAL quoted-string
                reason-extension  =  generic-param
            */

            var retVal = new StringBuilder();

            // Add protocol
            retVal.Append(m_Protocol);

            // Add parameters
            retVal.Append(ParametersToString());

            return retVal.ToString();
        }
    }
}
