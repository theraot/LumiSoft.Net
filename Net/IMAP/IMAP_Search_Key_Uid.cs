﻿using System;
using System.Collections.Generic;
using LumiSoft.Net.IMAP.Client;

namespace LumiSoft.Net.IMAP
{
    /// <summary>
    /// This class represents IMAP SEARCH <b>UID (sequence set)</b> key. Defined in RFC 3501 6.4.4.
    /// </summary>
    /// <remarks>Messages with unique identifiers corresponding to the specified
    /// unique identifier set.  Sequence set ranges are permitted.</remarks>
    public class IMAP_Search_Key_Uid : IMAP_Search_Key
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="seqSet">IMAP sequence-set.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>seqSet</b> is null reference.</exception>
        public IMAP_Search_Key_Uid(IMAP_t_SeqSet seqSet)
        {
            if(seqSet == null){
                throw new ArgumentNullException("seqSet");
            }

            Value = seqSet;
        }


        /// <summary>
        /// Returns parsed IMAP SEARCH <b>UID (sequence set)</b> key.
        /// </summary>
        /// <param name="r">String reader.</param>
        /// <returns>Returns parsed IMAP SEARCH <b>UID (sequence set)</b> key.</returns>
        /// <exception cref="ArgumentNullException">Is raised when <b>r</b> is null reference.</exception>
        /// <exception cref="ParseException">Is raised when parsing fails.</exception>
        internal static IMAP_Search_Key_Uid Parse(StringReader r)
        {
            if(r == null){
                throw new ArgumentNullException("r");
            }

            string word = r.ReadWord();
            if(!string.Equals(word,"UID",StringComparison.InvariantCultureIgnoreCase)){
                throw new ParseException("Parse error: Not a SEARCH 'UID' key.");
            }
            r.ReadToFirstChar();
            string value = r.QuotedReadToDelimiter(' ');
            if(value == null){
                throw new ParseException("Parse error: Invalid 'UID' value.");
            }
            
            try{
                return new IMAP_Search_Key_Uid(IMAP_t_SeqSet.Parse(value));
            }
            catch{
                throw new ParseException("Parse error: Invalid 'UID' value.");
            }
        }


        /// <summary>
        /// Returns this as string.
        /// </summary>
        /// <returns>Returns this as string.</returns>
        public override string ToString()
        {
            return "UID " + Value.ToString();
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
        /// Gets sequence-set value.
        /// </summary>
        public IMAP_t_SeqSet Value { get; }


        //--- OBSOLETE -----

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="seqSet">IMAP sequence-set.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>seqSet</b> is null reference.</exception>
        [Obsolete("Use constructor 'IMAP_Search_Key_Uid(IMAP_t_SeqSet seqSet)' instead.")]
        public IMAP_Search_Key_Uid(IMAP_SequenceSet seqSet)
        {
            if(seqSet == null){
                throw new ArgumentNullException("seqSet");
            }

            Value = IMAP_t_SeqSet.Parse(seqSet.ToSequenceSetString());
        }
    }
}
