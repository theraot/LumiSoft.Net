﻿using System;
using System.Collections.Generic;
using LumiSoft.Net.IMAP.Client;

namespace LumiSoft.Net.IMAP
{
    /// <summary>
    /// This class represents IMAP SEARCH <b>OR (search-key1) (search-key2)</b> key. Defined in RFC 3501 6.4.4.
    /// </summary>
    /// <remarks>Messages that match either search key.</remarks>
    public class IMAP_Search_Key_Or : IMAP_Search_Key
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="key1">Search key1.</param>
        /// <param name="key2">Search key2.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>key1</b> or <b>key2</b> is null reference.</exception>
        public IMAP_Search_Key_Or(IMAP_Search_Key key1,IMAP_Search_Key key2)
        {
            SearchKey1 = key1 ?? throw new ArgumentNullException("key1");
            SearchKey2 = key2 ?? throw new ArgumentNullException("key2");
        }

        /// <summary>
        /// Returns parsed IMAP SEARCH <b>OR (search-key1) (search-key2)</b> key.
        /// </summary>
        /// <param name="r">String reader.</param>
        /// <returns>Returns parsed IMAP SEARCH <b>OR (search-key1) (search-key2)</b> key.</returns>
        /// <exception cref="ArgumentNullException">Is raised when <b>r</b> is null reference.</exception>
        /// <exception cref="ParseException">Is raised when parsing fails.</exception>
        internal static IMAP_Search_Key_Or Parse(StringReader r)
        {
            if(r == null){
                throw new ArgumentNullException("r");
            }

            var word = r.ReadWord();
            if (!string.Equals(word,"OR",StringComparison.InvariantCultureIgnoreCase)){
                throw new ParseException("Parse error: Not a SEARCH 'OR' key.");
            }

            return new IMAP_Search_Key_Or(IMAP_Search_Key.ParseKey(r),IMAP_Search_Key.ParseKey(r));
        }

        /// <summary>
        /// Returns this as string.
        /// </summary>
        /// <returns>Returns this as string.</returns>
        public override string ToString()
        {
            return "OR " + SearchKey1.ToString() + " " + SearchKey2.ToString();
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

            list.Add(new IMAP_Client_CmdPart(IMAP_Client_CmdPart_Type.Constant,"OR "));
            SearchKey1.ToCmdParts(list);
            list.Add(new IMAP_Client_CmdPart(IMAP_Client_CmdPart_Type.Constant," "));
            SearchKey2.ToCmdParts(list);
        }

        /// <summary>
        /// Gets search-key1.
        /// </summary>
        public IMAP_Search_Key SearchKey1 { get; }

        /// <summary>
        /// Gets search-key2.
        /// </summary>
        public IMAP_Search_Key SearchKey2 { get; }
    }
}
