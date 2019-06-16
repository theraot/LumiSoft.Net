﻿using System;
using System.Collections.Generic;
using LumiSoft.Net.IMAP.Client;

namespace LumiSoft.Net.IMAP
{
    /// <summary>
    /// This class represents IMAP SEARCH <b>UNKEYWORD (flag)</b> key. Defined in RFC 3501 6.4.4.
    /// </summary>
    /// <remarks>Messages that do not have the specified keyword flag set.</remarks>
    public class IMAP_Search_Key_Unkeyword : IMAP_Search_Key
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="value">String value.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>value</b> is null reference.</exception>
        public IMAP_Search_Key_Unkeyword(string value)
        {
            if(value == null){
                throw new ArgumentNullException("value");
            }

            Value = value;
        }


        /// <summary>
        /// Returns parsed IMAP SEARCH <b>UNKEYWORD (string)</b> key.
        /// </summary>
        /// <param name="r">String reader.</param>
        /// <returns>Returns parsed IMAP SEARCH <b>UNKEYWORD (string)</b> key.</returns>
        /// <exception cref="ArgumentNullException">Is raised when <b>r</b> is null reference.</exception>
        /// <exception cref="ParseException">Is raised when parsing fails.</exception>
        internal static IMAP_Search_Key_Unkeyword Parse(StringReader r)
        {
            if(r == null){
                throw new ArgumentNullException("r");
            }

            string word = r.ReadWord();
            if(!string.Equals(word,"UNKEYWORD",StringComparison.InvariantCultureIgnoreCase)){
                throw new ParseException("Parse error: Not a SEARCH 'UNKEYWORD' key.");
            }
            string value = r.ReadWord();
            if(value == null){
                throw new ParseException("Parse error: Invalid 'UNKEYWORD' value.");
            }

            return new IMAP_Search_Key_Unkeyword(value);
        }


        /// <summary>
        /// Returns this as string.
        /// </summary>
        /// <returns>Returns this as string.</returns>
        public override string ToString()
        {
            return "UNKEYWORD " + Value;
        }


        /// <summary>
        /// Stores IMAP search-key command parts to the specified array.
        /// </summary>
        /// <param name="list">Array where to store command parts.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>list</b> is null reference.</exception>
        internal override void ToCmdParts(List<IMAP_Client_CmdPart> list)
        {
            if(list == null){
                throw new ArgumentNullException("list");
            }

            list.Add(new IMAP_Client_CmdPart(IMAP_Client_CmdPart_Type.Constant,ToString()));
        }


        /// <summary>
        /// Gets keyword value.
        /// </summary>
        public string Value { get; } = "";
    }
}
