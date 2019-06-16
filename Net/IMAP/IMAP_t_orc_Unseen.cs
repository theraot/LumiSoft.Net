﻿using System;

namespace LumiSoft.Net.IMAP
{
    /// <summary>
    /// This is class represents IMAP server <b>UNSEEN</b> optional response code. Defined in RFC 3501 7.1.
    /// </summary>
    public class IMAP_t_orc_Unseen : IMAP_t_orc
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="firstUnseen">First unseen message sequence number.</param>
        public IMAP_t_orc_Unseen(int firstUnseen)
        {
            SeqNo = firstUnseen;
        }

        /// <summary>
        /// Parses UNSEEN optional response from string.
        /// </summary>
        /// <param name="value">UNSEEN optional response string.</param>
        /// <returns>Returns UNSEEN optional response.</returns>
        /// <exception cref="ArgumentNullException">Is raised when <b>value</b> is null reference.</exception>
        public new static IMAP_t_orc_Unseen Parse(string value)
        {
            if(value == null){
                throw new ArgumentNullException("value");
            }

            var code_value = value.Split(new[]{' '},2);
            if (!string.Equals("UNSEEN",code_value[0],StringComparison.InvariantCultureIgnoreCase)){
                throw new ArgumentException("Invalid UNSEEN response value.","value");
            }
            if(code_value.Length != 2){
                throw new ArgumentException("Invalid UNSEEN response value.","value");
            }

            return new IMAP_t_orc_Unseen(Convert.ToInt32(code_value[1]));
        }

        /// <summary>
        /// Returns this as string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "UNSEEN " + SeqNo;
        }

        /// <summary>
        /// Gets first unseen message sequence number.
        /// </summary>
        public int SeqNo { get; }
    }
}
