﻿using System;
using System.Collections.Generic;
using LumiSoft.Net.IMAP.Client;

namespace LumiSoft.Net.IMAP
{
    /// <summary>
    /// This class represents IMAP SEARCH <b>SENTBEFORE (date)</b> key. Defined in RFC 3501 6.4.4.
    /// </summary>
    /// <remarks>Messages whose [RFC-2822] Date: header (disregarding time and
    /// timezone) is earlier than the specified date.</remarks>
    public class IMAP_Search_Key_SentBefore : IMAP_Search_Key
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="value">Date value.</param>
        public IMAP_Search_Key_SentBefore(DateTime value)
        {
            Date = value;
        }

        /// <summary>
        /// Returns parsed IMAP SEARCH <b>SENTBEFORE (string)</b> key.
        /// </summary>
        /// <param name="r">String reader.</param>
        /// <returns>Returns parsed IMAP SEARCH <b>SENTBEFORE (string)</b> key.</returns>
        /// <exception cref="ArgumentNullException">Is raised when <b>r</b> is null reference.</exception>
        /// <exception cref="ParseException">Is raised when parsing fails.</exception>
        internal static IMAP_Search_Key_SentBefore Parse(StringReader r)
        {
            if(r == null){
                throw new ArgumentNullException("r");
            }

            string word = r.ReadWord();
            if(!string.Equals(word,"SENTBEFORE",StringComparison.InvariantCultureIgnoreCase)){
                throw new ParseException("Parse error: Not a SEARCH 'SENTBEFORE' key.");
            }
            string value = r.ReadWord();
            if(value == null){
                throw new ParseException("Parse error: Invalid 'SENTBEFORE' value.");
            }
            DateTime date;
            try{
                date = IMAP_Utils.ParseDate(value);
            }
            catch{
                throw new ParseException("Parse error: Invalid 'SENTBEFORE' value.");
            }

            return new IMAP_Search_Key_SentBefore(date);
        }

        /// <summary>
        /// Returns this as string.
        /// </summary>
        /// <returns>Returns this as string.</returns>
        public override string ToString()
        {
            return "SENTBEFORE " + Date.ToString("dd-MMM-yyyy");
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
        /// Gets date value.
        /// </summary>
        public DateTime Date { get; }
    }
}
