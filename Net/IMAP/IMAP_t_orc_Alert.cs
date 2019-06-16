using System;

namespace LumiSoft.Net.IMAP
{
    /// <summary>
    /// This is class represents IMAP server <b>ALERT</b> optional response code. Defined in RFC 3501 7.1.
    /// </summary>
    public class IMAP_t_orc_Alert : IMAP_t_orc
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="text">Alert text.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>text</b> is null reference.</exception>
        public IMAP_t_orc_Alert(string text)
        {
            AlertText = text ?? throw new ArgumentNullException("text");
        }

        /// <summary>
        /// Gets alert text.
        /// </summary>
        public string AlertText { get; }

        /// <summary>
        /// Parses ALERT optional response from string.
        /// </summary>
        /// <param name="value">ALERT optional response string.</param>
        /// <returns>Returns ALERT optional response.</returns>
        /// <exception cref="ArgumentNullException">Is raised when <b>value</b> is null reference.</exception>
        public new static IMAP_t_orc_Alert Parse(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            var code_value = value.Split(new[] { ' ' }, 2);
            if (!string.Equals("ALERT", code_value[0], StringComparison.InvariantCultureIgnoreCase))
            {
                throw new ArgumentException("Invalid ALERT response value.", "value");
            }

            return new IMAP_t_orc_Alert(code_value.Length == 2 ? code_value[1] : "");
        }

        /// <summary>
        /// Returns this as string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "ALERT " + AlertText;
        }
    }
}
