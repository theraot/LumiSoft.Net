using System;
using System.Text;

namespace LumiSoft.Net.SIP.Message
{
    /// <summary>
    /// Implements SIP "ac-value" value. Defined in RFC 3841.
    /// </summary>
    /// <remarks>
    /// <code>
    /// RFC 3841 Syntax:
    ///     ac-value       = "*" *(SEMI ac-params)
    ///     ac-params      = feature-param / req-param / explicit-param / generic-param
    ///                      ;;feature param from RFC 3840
    ///                      ;;generic-param from RFC 3261
    ///     req-param      = "require"
    ///     explicit-param = "explicit"
    /// </code>
    /// </remarks>
    public class SIP_t_ACValue : SIP_t_ValueWithParams
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public SIP_t_ACValue()
        {
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="value">SIP 'ac-value' value.</param>
        public SIP_t_ACValue(string value)
        {
            Parse(value);
        }

        /// <summary>
        /// Gets or sets 'explicit' parameter value.
        /// </summary>
        public bool Explicit
        {
            get
            {
                var parameter = Parameters["explicit"];
                if (parameter != null)
                {
                    return true;
                }

                return false;
            }

            set
            {
                if (!value)
                {
                    Parameters.Remove("explicit");
                }
                else
                {
                    Parameters.Set("explicit", null);
                }
            }
        }

        /// <summary>
        /// Gets or sets 'require' parameter value.
        /// </summary>
        public bool Require
        {
            get
            {
                var parameter = Parameters["require"];
                if (parameter != null)
                {
                    return true;
                }

                return false;
            }

            set
            {
                if (!value)
                {
                    Parameters.Remove("require");
                }
                else
                {
                    Parameters.Set("require", null);
                }
            }
        }

        /// <summary>
        /// Parses "ac-value" from specified value.
        /// </summary>
        /// <param name="value">SIP "ac-value" value.</param>
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
        /// Parses "ac-value" from specified reader.
        /// </summary>
        /// <param name="reader">Reader from where to parse.</param>
        /// <exception cref="ArgumentNullException">Raised when <b>reader</b> is null.</exception>
        /// <exception cref="SIP_ParseException">Raised when invalid SIP message.</exception>
        public override void Parse(StringReader reader)
        {
            /*
                ac-value       = "*" *(SEMI ac-params)
                ac-params      = feature-param / req-param / explicit-param / generic-param
                                 ;;feature param from RFC 3840
                                 ;;generic-param from RFC 3261
                req-param      = "require"
                explicit-param = "explicit"
            */

            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }

            var word = reader.ReadWord();
            if (word == null)
            {
                throw new SIP_ParseException("Invalid 'ac-value', '*' is missing !");
            }

            // Parse parameters
            ParseParameters(reader);
        }

        /// <summary>
        /// Converts this to valid "ac-value" value.
        /// </summary>
        /// <returns>Returns "ac-value" value.</returns>
        public override string ToStringValue()
        {
            /*
                ac-value       = "*" *(SEMI ac-params)
                ac-params      = feature-param / req-param / explicit-param / generic-param
                                 ;;feature param from RFC 3840
                                 ;;generic-param from RFC 3261
                req-param      = "require"
                explicit-param = "explicit"
            */

            var retVal = new StringBuilder();

            // *
            retVal.Append("*");

            // Add parameters
            retVal.Append(ParametersToString());

            return retVal.ToString();
        }
    }
}
