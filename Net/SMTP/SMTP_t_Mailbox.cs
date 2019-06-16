﻿using System;

//using LumiSoft.Net.ABNF;
using LumiSoft.Net.MIME;

namespace LumiSoft.Net.SMTP
{
    /// <summary>
    /// This class represents SMTP "Mailbox" address. Defined in RFC 5321 4.1.2.
    /// </summary>
    /// <example>
    /// <code>
    /// RFC 5321 4.1.2.
    ///     Mailbox    = Local-part "@" ( Domain / address-literal )
    ///     Local-part = Dot-string / Quoted-string
    ///                  ; MAY be case-sensitive
    /// </code>
    /// </example>
    public class SMTP_t_Mailbox
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="localPart">Local part of mailbox.</param>
        /// <param name="domain">Domain of mailbox.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>localPart</b> or <b>domain</b> is null reference.</exception>
        public SMTP_t_Mailbox(string localPart,string domain)
        {
            if(localPart == null){
                throw new ArgumentNullException("localPart");
            }
            if(domain == null){
                throw new ArgumentNullException("domain");
            }

            LocalPart = localPart;
            Domain    = domain;
        }

        /*
        /// <summary>
        /// Parses SMTP mailbox from the specified string.
        /// </summary>
        /// <param name="value">Mailbox string.</param>
        /// <returns>Returns parsed SMTP mailbox.</returns>
        /// <exception cref="ArgumentNullException">Is raised when <b>value</b> is null reference.</exception>
        public static SMTP_t_Mailbox Parse(string value)
        {
            if(value == null){
                throw new ArgumentNullException("value");
            }

            return Parse(new ABNF_Reader(value));
        }

        /// <summary>
        /// Parses SMTP mailbox from the specified reader.
        /// </summary>
        /// <param name="reader">Source reader.</param>
        /// <returns>Returns parsed SMTP mailbox.</returns>
        /// <exception cref="ArgumentNullException">Is raised when <b>reader</b> is null reference.</exception>
        public static SMTP_t_Mailbox Parse(ABNF_Reader reader)
        {
            if(reader == null){
                throw new ArgumentNullException("reader");
            }

            // TODO:

            return null;
        }
        */

        /// <summary>
        /// Returns mailbox as string.
        /// </summary>
        /// <returns>Returns mailbox as string.</returns>
        public override string ToString()
        {
            if(MIME_Reader.IsDotAtom(LocalPart)){
                return LocalPart + "@" + (Net_Utils.IsIPAddress(Domain) ? "[" + Domain + "]" : Domain);
            }
            else{
                return TextUtils.QuoteString(LocalPart) + "@" + (Net_Utils.IsIPAddress(Domain) ? "[" + Domain + "]" : Domain);
            }
        }

        /// <summary>
        /// Gets local-part of mailbox.
        /// </summary>
        /// <remarks>If local-part is <b>Quoted-string</b>, quotes will not returned.</remarks>
        public string LocalPart { get; }

        /// <summary>
        /// Gets domain of mailbox.
        /// </summary>
        /// <remarks>If domain is <b>address-literal</b>, surrounding bracets will be removed.</remarks>
        public string Domain { get; }
    }
}
