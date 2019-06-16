using System;
using System.Text;

namespace LumiSoft.Net.SIP.Message
{
    /// <summary>
    /// Implements SIP "Referred-By" value. Defined in RFC 3892.
    /// </summary>
    /// <remarks>
    /// <code>
    /// RFC 3892 Syntax:
    ///     Referred-By         =  referrer-uri *( SEMI (referredby-id-param / generic-param) )
    ///     referrer-uri        = ( name-addr / addr-spec )
    ///     referredby-id-param = "cid" EQUAL sip-clean-msg-id
    ///     sip-clean-msg-id    = LDQUOT dot-atom "@" (dot-atom / host) RDQUOT
    /// </code>
    /// </remarks>
    public class SIP_t_ReferredBy : SIP_t_ValueWithParams
    {
        private SIP_t_NameAddress m_pAddress;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="value">SIP 'Referred-By' value.</param>
        public SIP_t_ReferredBy(string value)
        {
            m_pAddress = new SIP_t_NameAddress();

            Parse(value);
        }

        /// <summary>
        /// Gets or sets address.
        /// </summary>
        /// <exception cref="ArgumentNullException">Is raised when null value is passed.</exception>
        public SIP_t_NameAddress Address
        {
            get => m_pAddress;

            set => m_pAddress = value ?? throw new ArgumentNullException("Address");
        }

        /// <summary>
        /// Gets or sets 'cid' parameter value. Value null means not specified.
        /// </summary>
        public string CID
        {
            get
            {
                var parameter = Parameters["cid"];
                return parameter?.Value;
            }

            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    Parameters.Remove("cid");
                }
                else
                {
                    Parameters.Set("cid", value);
                }
            }
        }

        /// <summary>
        /// Parses "Referred-By" from specified value.
        /// </summary>
        /// <param name="value">SIP "Referred-By" value.</param>
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
        /// Parses "Referred-By" from specified reader.
        /// </summary>
        /// <param name="reader">Reader from where to parse.</param>
        /// <exception cref="ArgumentNullException">Raised when <b>reader</b> is null.</exception>
        /// <exception cref="SIP_ParseException">Raised when invalid SIP message.</exception>
        public override void Parse(StringReader reader)
        {
            /*
                Referred-By         =  referrer-uri *( SEMI (referredby-id-param / generic-param) )
                referrer-uri        = ( name-addr / addr-spec )
                referredby-id-param = "cid" EQUAL sip-clean-msg-id
                sip-clean-msg-id    = LDQUOT dot-atom "@" (dot-atom / host) RDQUOT
            */

            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }

            // referrer-uri
            m_pAddress.Parse(reader);

            // Parse parameters
            ParseParameters(reader);
        }

        /// <summary>
        /// Converts this to valid "Referred-By" value.
        /// </summary>
        /// <returns>Returns "Referred-By" value.</returns>
        public override string ToStringValue()
        {
            /*
                Referred-By         =  referrer-uri *( SEMI (referredby-id-param / generic-param) )
                referrer-uri        = ( name-addr / addr-spec )
                referredby-id-param = "cid" EQUAL sip-clean-msg-id
                sip-clean-msg-id    = LDQUOT dot-atom "@" (dot-atom / host) RDQUOT
            */

            var retVal = new StringBuilder();

            // referrer-uri
            retVal.Append(m_pAddress.ToStringValue());

            // Add parameters
            retVal.Append(ParametersToString());

            return retVal.ToString();
        }
    }
}
