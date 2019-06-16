using System;

using LumiSoft.Net.MIME;

namespace LumiSoft.Net.Mail
{
    /// <summary>
    /// This class represents "mailbox" address. Defined in RFC 5322 3.4.
    /// </summary>
    /// <example>
    /// <code>
    /// RFC 5322 3.4.
    ///     mailbox    = name-addr / addr-spec
    ///     name-addr  = [display-name] angle-addr
    ///     angle-addr = [CFWS] "&lt;" addr-spec "&gt;" [CFWS]
    ///     addr-spec  = local-part "@" domain
    /// </code>
    /// </example>
    public class Mail_t_Mailbox : Mail_t_Address
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="displayName">Display name. Value null means not specified.</param>
        /// <param name="address">Email address.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>address</b> is null reference.</exception>
        public Mail_t_Mailbox(string displayName,string address)
        {
            DisplayName = displayName;
            Address     = address ?? throw new ArgumentNullException("address");
        }

        /// <summary>
        /// Parses <b>mailbox</b> from specified string value.
        /// </summary>
        /// <param name="value">The <b>mailbox</b> string value.</param>
        /// <returns>Returns parse mailbox.</returns>
        /// <exception cref="ArgumentNullException">Is raised when <b>value</b> is null reference.</exception>
        /// <exception cref="ParseException">Is raised when <b>value</b> is not valid <b>mailbox</b> value.</exception>
        public static Mail_t_Mailbox Parse(string value)
        {
            if(value == null){
                throw new ArgumentNullException("value");
            }

            var        r      = new MIME_Reader(value);
            var retVal = new Mail_t_MailboxList();
            while (true){
                var word = r.QuotedReadToDelimiter(new char[]{',','<'});
                // We processed all data.
                if (string.IsNullOrEmpty(word) && r.Available == 0){
                    throw new ParseException("Not valid 'mailbox' value '" + value + "'.");
                }
                // name-addr

                if(r.Peek(true) == '<'){
                    return new Mail_t_Mailbox(word != null ? MIME_Encoding_EncodedWord.DecodeS(TextUtils.UnQuoteString(word.Trim())) : null,r.ReadParenthesized());
                }
                // addr-spec
                return new Mail_t_Mailbox(null,word);
            }

            throw new ParseException("Not valid 'mailbox' value '" + value + "'.");
        }

        /// <summary>
        /// Returns mailbox as string.
        /// </summary>
        /// <returns>Returns mailbox as string.</returns>
        public override string ToString()
        {
            return ToString(null);
        }

        /// <summary>
        /// Returns address as string value.
        /// </summary>
        /// <param name="wordEncoder">8-bit words ecnoder. Value null means that words are not encoded.</param>
        /// <returns>Returns address as string value.</returns>
        public override string ToString(MIME_Encoding_EncodedWord wordEncoder)
        {
            if(string.IsNullOrEmpty(DisplayName)){
                return Address;
            }

            if(wordEncoder != null && MIME_Encoding_EncodedWord.MustEncode(DisplayName)){
                return wordEncoder.Encode(DisplayName) + " " + "<" + Address + ">";
            }

            return TextUtils.QuoteString(DisplayName) + " " + "<" + Address + ">";
        }

        /// <summary>
        /// Gets display name. Value null means not specified.
        /// </summary>
        public string DisplayName { get; }

        /// <summary>
        /// Gets address.
        /// </summary>
        public string Address { get; }

        /// <summary>
        /// Gets local-part of address.
        /// </summary>
        public string LocalPart
        {
            get{ 
                var localpart_domain = Address.Split('@');

                return localpart_domain[0]; 
            }
        }

        /// <summary>
        /// Gets domain part of address.
        /// </summary>
        public string Domain
        {
            get{ 
                var localpart_domain = Address.Split('@');

                if (localpart_domain.Length == 2){
                    return localpart_domain[1]; 
                }

                return "";
            }
        }
    }
}
