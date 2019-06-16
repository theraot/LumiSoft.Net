using System;

namespace LumiSoft.Net.IMAP
{
    /// <summary>
    /// This is class represents IMAP server <b>PERMANENTFLAGS</b> optional response code. Defined in RFC 3501 7.1.
    /// </summary>
    public class IMAP_t_orc_PermanentFlags : IMAP_t_orc
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="flags">List of supported permanent flags.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>flags</b> is null reference.</exception>
        public IMAP_t_orc_PermanentFlags(string[] flags)
        {
            Flags = flags ?? throw new ArgumentNullException("flags");
        }

        /// <summary>
        /// Parses PERMANENTFLAGS optional response from string.
        /// </summary>
        /// <param name="value">PERMANENTFLAGS optional response string.</param>
        /// <returns>Returns PERMANENTFLAGS optional response.</returns>
        /// <exception cref="ArgumentNullException">Is raised when <b>value</b> is null reference.</exception>
        public new static IMAP_t_orc_PermanentFlags Parse(string value)
        {
            if(value == null){
                throw new ArgumentNullException("value");
            }

            var code_value = value.Split(new char[]{' '},2);
            if (!string.Equals("PERMANENTFLAGS",code_value[0],StringComparison.InvariantCultureIgnoreCase)){
                throw new ArgumentException("Invalid PERMANENTFLAGS response value.","value");
            }
            if(code_value.Length != 2){
                throw new ArgumentException("Invalid PERMANENTFLAGS response value.","value");
            }

            var r = new StringReader(code_value[1]);
            r.ReadWord();

            return new IMAP_t_orc_PermanentFlags(r.ReadParenthesized().Split(' '));
        }

        /// <summary>
        /// Returns this as string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "PERMANENTFLAGS (" + Net_Utils.ArrayToString(Flags," ") + ")";
        }

        /// <summary>
        /// Gets list of supported permanent flags.
        /// </summary>
        public string[] Flags { get; }
    }
}
