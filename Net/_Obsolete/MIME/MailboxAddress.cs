using System;

using LumiSoft.Net.MIME;

namespace LumiSoft.Net.Mime
{
    /// <summary>
    /// RFC 2822 3.4. (Address Specification) Mailbox address.
    /// <p/>
    /// Syntax: ["display-name"&lt;SP&gt;]&lt;local-part@domain&gt;.
    /// </summary>
    [Obsolete("See LumiSoft.Net.MIME or LumiSoft.Net.Mail namepaces for replacement.")]
    public class MailboxAddress : Address
    {
        private string _displayName = "";
        private string _emailAddress = "";

        /// <summary>
        /// Default constructor.
        /// </summary>
        public MailboxAddress() : base(false)
        {
        }

        /// <summary>
        /// Creates new mailbox from specified email address.
        /// </summary>
        /// <param name="emailAddress">Email address.</param>
        public MailboxAddress(string emailAddress) : base(false)
        {
            _emailAddress = emailAddress;
        }

        /// <summary>
        /// Creates new mailbox from specified name and email address.
        /// </summary>
        /// <param name="displayName">Display name.</param>
        /// <param name="emailAddress">Email address.</param>
        public MailboxAddress(string displayName, string emailAddress) : base(false)
        {
            _displayName = displayName;
            _emailAddress = emailAddress;
        }

        /// <summary>
        /// Gets or sets display name.
        /// </summary>
        public string DisplayName
        {
            get => _displayName;

            set
            {
                _displayName = value;

                OnChanged();
            }
        }

        /// <summary>
        /// Gets domain from email address. For example domain is "lumisoft.ee" from "ivar@lumisoft.ee".
        /// </summary>
        public string Domain
        {
            get
            {
                if (EmailAddress.IndexOf("@") != -1)
                {
                    return EmailAddress.Substring(EmailAddress.IndexOf("@") + 1);
                }

                return "";
            }
        }

        /// <summary>
        /// Gets or sets email address. For example ivar@lumisoft.ee.
        /// </summary>
        public string EmailAddress
        {
            get => _emailAddress;

            set
            {
                // Email address can contain only ASCII chars.
                if (!Core.IsAscii(value))
                {
                    throw new Exception("Email address can contain ASCII chars only !");
                }

                _emailAddress = value;

                OnChanged();
            }
        }

        /// <summary>
        /// Gets local-part from email address. For example mailbox is "ivar" from "ivar@lumisoft.ee".
        /// </summary>
        public string LocalPart
        {
            get
            {
                if (EmailAddress.IndexOf("@") > -1)
                {
                    return EmailAddress.Substring(0, EmailAddress.IndexOf("@"));
                }

                return EmailAddress;
            }
        }

        /// <summary>
        /// Gets Mailbox as RFC 2822(3.4. Address Specification) string. Format: ["display-name"&lt;SP&gt;]&lt;local-part@domain&gt;.
        /// For example, "Ivar Lumi" &lt;ivar@lumisoft.ee&gt;.
        /// </summary>
        [Obsolete("Use ToMailboxAddressString instead !")]
        public string MailboxString
        {
            get
            {
                var retVal = "";
                if (DisplayName != "")
                {
                    retVal += TextUtils.QuoteString(DisplayName) + " ";
                }
                retVal += "<" + EmailAddress + ">";

                return retVal;
            }
        }

        /// <summary>
        /// Parses mailbox from mailbox address string.
        /// </summary>
        /// <param name="mailbox">Mailbox string. Format: ["diplay-name"&lt;SP&gt;]&lt;local-part@domain&gt;.</param>
        /// <returns></returns>
        public static MailboxAddress Parse(string mailbox)
        {
            mailbox = mailbox.Trim();

            /* We must parse following situations:
                "Ivar Lumi" <ivar@lumisoft.ee>
                "Ivar Lumi" ivar@lumisoft.ee
                <ivar@lumisoft.ee>
                ivar@lumisoft.ee                
                Ivar Lumi <ivar@lumisoft.ee>
            */

            var name = "";
            var emailAddress = mailbox;

            // Email address is between <> and remaining left part is display name
            if (mailbox.IndexOf("<") > -1 && mailbox.IndexOf(">") > -1)
            {
                name = MIME_Encoding_EncodedWord.DecodeS(TextUtils.UnQuoteString(mailbox.Substring(0, mailbox.LastIndexOf("<"))));
                emailAddress = mailbox.Substring(mailbox.LastIndexOf("<") + 1, mailbox.Length - mailbox.LastIndexOf("<") - 2).Trim();
            }
            else
            {
                // There is name included, parse it
                if (mailbox.StartsWith("\""))
                {
                    int startIndex = mailbox.IndexOf("\"");
                    if (startIndex > -1 && mailbox.LastIndexOf("\"") > startIndex)
                    {
                        name = MIME_Encoding_EncodedWord.DecodeS(mailbox.Substring(startIndex + 1, mailbox.LastIndexOf("\"") - startIndex - 1).Trim());
                    }

                    emailAddress = mailbox.Substring(mailbox.LastIndexOf("\"") + 1).Trim();
                }

                // Right part must be email address
                emailAddress = emailAddress.Replace("<", "").Replace(">", "").Trim();
            }

            return new MailboxAddress(name, emailAddress);
        }

        /// <summary>
        /// Converts this to valid mailbox address string.
        /// Defined in RFC 2822(3.4. Address Specification) string. Format: ["display-name"&lt;SP&gt;]&lt;local-part@domain&gt;.
        /// For example, "Ivar Lumi" &lt;ivar@lumisoft.ee&gt;.
        /// If display name contains unicode chrs, display name will be encoded with canonical encoding in utf-8 charset.
        /// </summary>
        /// <returns></returns>
        public string ToMailboxAddressString()
        {
            var retVal = "";
            if (_displayName.Length > 0)
            {
                if (Core.IsAscii(_displayName))
                {
                    retVal = TextUtils.QuoteString(_displayName) + " ";
                }
                else
                {
                    // Encoded word must be treated as unquoted and unescaped word.
                    retVal = MimeUtils.EncodeWord(_displayName) + " ";
                }
            }
            retVal += "<" + EmailAddress + ">";

            return retVal;
        }

        /// <summary>
        /// This called when mailox address has changed.
        /// </summary>
        internal void OnChanged()
        {
            if (Owner != null)
            {
                if (Owner is AddressList)
                {
                    ((AddressList)Owner).OnCollectionChanged();
                }
                else
                {
                    (Owner as MailboxAddressCollection)?.OnCollectionChanged();
                }
            }
        }
    }
}
