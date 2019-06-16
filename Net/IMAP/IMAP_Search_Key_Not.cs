using System;
using System.Collections.Generic;
using LumiSoft.Net.IMAP.Client;

namespace LumiSoft.Net.IMAP
{
    /// <summary>
    /// This class represents IMAP SEARCH <b>NOT (search-key)</b> key. Defined in RFC 3501 6.4.4.
    /// </summary>
    /// <remarks>Messages that do not match the specified search key.</remarks>
    public class IMAP_Search_Key_Not : IMAP_Search_Key
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="key">Search KEY.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>key</b> is null reference.</exception>
        public IMAP_Search_Key_Not(IMAP_Search_Key key)
        {
            SearchKey = key ?? throw new ArgumentNullException("key");
        }

        /// <summary>
        /// Returns parsed IMAP SEARCH <b>NOT (search-key)</b> key.
        /// </summary>
        /// <param name="r">String reader.</param>
        /// <returns>Returns parsed IMAP SEARCH <b>NOT (search-key)</b> key.</returns>
        /// <exception cref="ArgumentNullException">Is raised when <b>r</b> is null reference.</exception>
        /// <exception cref="ParseException">Is raised when parsing fails.</exception>
        internal static IMAP_Search_Key_Not Parse(StringReader r)
        {
            if(r == null){
                throw new ArgumentNullException("r");
            }

            var word = r.ReadWord();
            if (!string.Equals(word,"NOT",StringComparison.InvariantCultureIgnoreCase)){
                throw new ParseException("Parse error: Not a SEARCH 'NOT' key.");
            }

            return new IMAP_Search_Key_Not(IMAP_Search_Key.ParseKey(r));
        }

        /// <summary>
        /// Returns this as string.
        /// </summary>
        /// <returns>Returns this as string.</returns>
        public override string ToString()
        {
            return "NOT " + SearchKey.ToString();
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

            list.Add(new IMAP_Client_CmdPart(IMAP_Client_CmdPart_Type.Constant,"NOT "));
            SearchKey.ToCmdParts(list);
        }

        /// <summary>
        /// Gets search KEY.
        /// </summary>
        public IMAP_Search_Key SearchKey { get; }
    }
}
