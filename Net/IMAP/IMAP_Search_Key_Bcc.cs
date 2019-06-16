﻿using System;
using System.Collections.Generic;
using LumiSoft.Net.IMAP.Client;

namespace LumiSoft.Net.IMAP
{
    /// <summary>
    /// This class represents IMAP SEARCH <b>BCC (string)</b> key. Defined in RFC 3501 6.4.4.
    /// </summary>
    /// <remarks>Messages that contain the specified string in the message header BCC field.</remarks>
    public class IMAP_Search_Key_Bcc : IMAP_Search_Key
    {
        private readonly string m_Value = "";

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="value">String value.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>value</b> is null reference.</exception>
        public IMAP_Search_Key_Bcc(string value)
        {
            if(value == null){
                throw new ArgumentNullException("value");
            }

            m_Value = value;
        }


        #region static method Parse

        /// <summary>
        /// Returns parsed IMAP SEARCH <b>BCC (string)</b> key.
        /// </summary>
        /// <param name="r">String reader.</param>
        /// <returns>Returns parsed IMAP SEARCH <b>BCC (string)</b> key.</returns>
        /// <exception cref="ArgumentNullException">Is raised when <b>r</b> is null reference.</exception>
        /// <exception cref="ParseException">Is raised when parsing fails.</exception>
        internal static IMAP_Search_Key_Bcc Parse(StringReader r)
        {
            if(r == null){
                throw new ArgumentNullException("r");
            }

            string word = r.ReadWord();
            if(!string.Equals(word,"BCC",StringComparison.InvariantCultureIgnoreCase)){
                throw new ParseException("Parse error: Not a SEARCH 'BCC' key.");
            }
            string value = IMAP_Utils.ReadString(r);
            if(value == null){
                throw new ParseException("Parse error: Invalid 'BCC' value.");
            }

            return new IMAP_Search_Key_Bcc(value);
        }

        #endregion


        #region override method ToString

        /// <summary>
        /// Returns this as string.
        /// </summary>
        /// <returns>Returns this as string.</returns>
        public override string ToString()
        {
            return "BCC " + TextUtils.QuoteString(m_Value);
        }

        #endregion


        #region internal override method ToCmdParts

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

            list.Add(new IMAP_Client_CmdPart(IMAP_Client_CmdPart_Type.Constant,"BCC "));
            list.Add(new IMAP_Client_CmdPart(IMAP_Client_CmdPart_Type.String,m_Value));
        }

        #endregion


        #region Properties implementation

        /// <summary>
        /// Gets BCC filter value.
        /// </summary>
        public string Value
        {
            get{ return m_Value; }
        }

        #endregion
    }
}
