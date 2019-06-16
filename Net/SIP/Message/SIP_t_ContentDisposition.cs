using System;
using System.Text;

namespace LumiSoft.Net.SIP.Message
{
    /// <summary>
    /// Implements SIP "Content-Disposition" value. Defined in RFC 3261.
    /// </summary>
    /// <remarks>
    /// <code>
    /// RFC 3261 Syntax:
    ///     Content-Disposition  = disp-type *( SEMI disp-param )
    ///     disp-type            = "render" / "session" / "icon" / "alert" / disp-extension-token
    ///     disp-param           = handling-param / generic-param
    ///     handling-param       = "handling" EQUAL ( "optional" / "required" / other-handling )
    ///     other-handling       = token
    ///     disp-extension-token = token
    /// </code>
    /// </remarks>
    public class SIP_t_ContentDisposition : SIP_t_ValueWithParams
    {
        private string m_DispositionType = "";

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="value">SIP SIP_t_ContentDisposition value.</param>
        public SIP_t_ContentDisposition(string value)
        {
            Parse(value);
        }

        /// <summary>
        /// Gets or sets disposition type. Known values: "render","session","icon","alert".
        /// </summary>
        public string DispositionType
        {
            get => m_DispositionType;

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("DispositionType");
                }
                if (!TextUtils.IsToken(value))
                {
                    throw new ArgumentException("Invalid DispositionType value, value must be 'token' !");
                }

                m_DispositionType = value;
            }
        }

        /// <summary>
        /// Gets or sets 'handling' parameter value. Value null means not specified.
        /// Known value: "optional","required".
        /// </summary>
        public string Handling
        {
            get
            {
                var parameter = Parameters["handling"];
                return parameter?.Value;
            }

            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    Parameters.Remove("handling");
                }
                else
                {
                    Parameters.Set("handling", value);
                }
            }
        }

        /// <summary>
        /// Parses "Content-Disposition" from specified value.
        /// </summary>
        /// <param name="value">SIP "Content-Disposition" value.</param>
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
        /// Parses "Content-Disposition" from specified reader.
        /// </summary>
        /// <param name="reader">Reader from where to parse.</param>
        /// <exception cref="ArgumentNullException">Raised when <b>reader</b> is null.</exception>
        /// <exception cref="SIP_ParseException">Raised when invalid SIP message.</exception>
        public override void Parse(StringReader reader)
        {
            /* 
                Content-Disposition  = disp-type *( SEMI disp-param )
                disp-type            = "render" / "session" / "icon" / "alert" / disp-extension-token
                disp-param           = handling-param / generic-param
                handling-param       = "handling" EQUAL ( "optional" / "required" / other-handling )
                other-handling       = token
                disp-extension-token = token
            */

            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }

            // disp-type
            var word = reader.ReadWord();
            m_DispositionType = word ?? throw new SIP_ParseException("SIP Content-Disposition 'disp-type' value is missing !");

            // Parse parameters
            ParseParameters(reader);
        }

        /// <summary>
        /// Converts this to valid "Content-Disposition" value.
        /// </summary>
        /// <returns>Returns "Content-Disposition" value.</returns>
        public override string ToStringValue()
        {
            var retVal = new StringBuilder();
            retVal.Append(m_DispositionType);
            retVal.Append(ParametersToString());

            return retVal.ToString();
        }
    }
}
