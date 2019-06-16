using System;
using System.Text;

using LumiSoft.Net.MIME;

namespace LumiSoft.Net.Mail
{
    /// <summary>
    /// This class represent generic <b>address-list</b> header fields. For example: To header.
    /// </summary>
    /// <example>
    /// <code>
    /// RFC 5322.
    ///     header       = "FiledName:" address-list CRLF
    ///     address-list = (address *("," address))
    ///     address      = mailbox / group
    /// </code>
    /// </example>
    public class Mail_h_AddressList : MIME_h
    {
        private string m_ParseValue;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="fieldName">Header field name. For example: "To".</param>
        /// <param name="values">Addresses collection.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>filedName</b> or <b>values</b> is null reference.</exception>
        /// <exception cref="ArgumentException">Is raised when any of the arguments has invalid value.</exception>
        public Mail_h_AddressList(string fieldName, Mail_t_AddressList values)
        {
            if (fieldName == null)
            {
                throw new ArgumentNullException("fieldName");
            }
            if (fieldName == string.Empty)
            {
                throw new ArgumentException("Argument 'fieldName' value must be specified.");
            }

            Name = fieldName;
            Addresses = values ?? throw new ArgumentNullException("values");
        }

        /// <summary>
        /// Gets addresses collection.
        /// </summary>
        public Mail_t_AddressList Addresses { get; }

        /// <summary>
        /// Gets if this header field is modified since it has loaded.
        /// </summary>
        /// <remarks>All new added header fields has <b>IsModified = true</b>.</remarks>
        /// <exception cref="ObjectDisposedException">Is riased when this class is disposed and this property is accessed.</exception>
        public override bool IsModified
        {
            get { return Addresses.IsModified; }
        }

        /// <summary>
        /// Gets header field name. For example "To".
        /// </summary>
        public override string Name { get; }

        /// <summary>
        /// Parses header field from the specified value.
        /// </summary>
        /// <param name="value">Header field value. Header field name must be included. For example: 'Content-Type: text/plain'.</param>
        /// <returns>Returns parsed header field.</returns>
        /// <exception cref="ArgumentNullException">Is raised when <b>value</b> is null reference.</exception>
        /// <exception cref="ParseException">Is raised when header field parsing errors.</exception>
        public static Mail_h_AddressList Parse(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            var name_value = value.Split(new[] { ':' }, 2);
            if (name_value.Length != 2)
            {
                throw new ParseException("Invalid header field value '" + value + "'.");
            }

            /* RFC 5322 3.4.
                address         =   mailbox / group
                mailbox         =   name-addr / addr-spec
                name-addr       =   [display-name] angle-addr
                angle-addr      =   [CFWS] "<" addr-spec ">" [CFWS] / obs-angle-addr
                group           =   display-name ":" [group-list] ";" [CFWS]
                display-name    =   phrase
                mailbox-list    =   (mailbox *("," mailbox)) / obs-mbox-list
                address-list    =   (address *("," address)) / obs-addr-list
                group-list      =   mailbox-list / CFWS / obs-group-list
            */

            var retVal = new Mail_h_AddressList(name_value[0], Mail_t_AddressList.Parse(name_value[1].Trim()));
            retVal.m_ParseValue = value;
            retVal.Addresses.AcceptChanges();

            return retVal;
        }

        /// <summary>
        /// Returns header field as string.
        /// </summary>
        /// <param name="wordEncoder">8-bit words ecnoder. Value null means that words are not encoded.</param>
        /// <param name="parmetersCharset">Charset to use to encode 8-bit characters. Value null means parameters not encoded.</param>
        /// <param name="reEncode">If true always specified encoding is used. If false and header field value not modified, original encoding is kept.</param>
        /// <returns>Returns header field as string.</returns>
        public override string ToString(MIME_Encoding_EncodedWord wordEncoder, Encoding parmetersCharset, bool reEncode)
        {
            if (reEncode || IsModified)
            {
                var retVal = new StringBuilder();
                retVal.Append(Name + ": ");
                for (int i = 0; i < Addresses.Count; i++)
                {
                    if (i > 0)
                    {
                        retVal.Append("\t");
                    }

                    // Don't add ',' for last item.
                    if (i == (Addresses.Count - 1))
                    {
                        retVal.Append(Addresses[i].ToString(wordEncoder) + "\r\n");
                    }
                    else
                    {
                        retVal.Append(Addresses[i].ToString(wordEncoder) + ",\r\n");
                    }
                }
                // No items, we need to add ending CRLF.
                if (Addresses.Count == 0)
                {
                    retVal.Append("\r\n");
                }

                return retVal.ToString();
            }

            return m_ParseValue;
        }
    }
}
