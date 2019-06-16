using System;

namespace LumiSoft.Net.IMAP
{
    /// <summary>
    /// This is class represents IMAP server <b>UIDNEXT</b> optional response code. Defined in RFC 3501 7.1.
    /// </summary>
    public class IMAP_t_orc_UidNext : IMAP_t_orc
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="uidNext">Next UID value.</param>
        public IMAP_t_orc_UidNext(int uidNext)
        {
            UidNext = uidNext;
        }

        /// <summary>
        /// Gets next message predicted UID value.
        /// </summary>
        public int UidNext { get; }

        /// <summary>
        /// Parses UIDNEXT optional response from string.
        /// </summary>
        /// <param name="value">UIDNEXT optional response string.</param>
        /// <returns>Returns UIDNEXT optional response.</returns>
        /// <exception cref="ArgumentNullException">Is raised when <b>value</b> is null reference.</exception>
        public new static IMAP_t_orc_UidNext Parse(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            var code_value = value.Split(new[] { ' ' }, 2);
            if (!string.Equals("UIDNEXT", code_value[0], StringComparison.InvariantCultureIgnoreCase))
            {
                throw new ArgumentException("Invalid UIDNEXT response value.", "value");
            }
            if (code_value.Length != 2)
            {
                throw new ArgumentException("Invalid UIDNEXT response value.", "value");
            }

            return new IMAP_t_orc_UidNext(Convert.ToInt32(code_value[1]));
        }

        /// <summary>
        /// Returns this as string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "UIDNEXT " + UidNext;
        }
    }
}
