using System;
using System.Collections.Generic;
using LumiSoft.Net.IMAP.Client;

namespace LumiSoft.Net.IMAP
{
    /// <summary>
    /// This class is base class for IMAP SEARCH search-key. Defined in RFC 3501 6.4.4.
    /// </summary>
    public abstract class IMAP_Search_Key
    {
        /// <summary>
        /// Parses one search key or search key group.
        /// </summary>
        /// <param name="r">String reader.</param>
        /// <returns>Returns one parsed search key or search key group.</returns>
        /// <exception cref="ArgumentNullException">Is raised when <b>r</b> is null reference.</exception>
        /// <exception cref="ParseException">Is raised when parsing fails.</exception>
        internal static IMAP_Search_Key ParseKey(StringReader r)
        {
            if(r == null){
                throw new ArgumentNullException("r");
            }

            r.ReadToFirstChar();

            // Keys group
            if(r.StartsWith("(",false)){
                return IMAP_Search_Key_Group.Parse(new StringReader(r.ReadParenthesized()));
            }
            // ANSWERED

            if(r.StartsWith("ANSWERED",false)){
                return IMAP_Search_Key_Answered.Parse(r);
            }
            // BCC
            if(r.StartsWith("BCC",false)){
                return IMAP_Search_Key_Bcc.Parse(r);
            }
            // BEFORE
            if(r.StartsWith("BEFORE",false)){
                return IMAP_Search_Key_Before.Parse(r);
            }
            // BODY
            if(r.StartsWith("BODY",false)){
                return IMAP_Search_Key_Body.Parse(r);
            }
            // CC
            if(r.StartsWith("CC",false)){
                return IMAP_Search_Key_Cc.Parse(r);
            }
            // DELETED
            if(r.StartsWith("DELETED",false)){
                return IMAP_Search_Key_Deleted.Parse(r);
            }
            // DRAFT
            if(r.StartsWith("DRAFT",false)){
                return IMAP_Search_Key_Draft.Parse(r);
            }
            // FLAGGED
            if(r.StartsWith("FLAGGED",false)){
                return IMAP_Search_Key_Flagged.Parse(r);
            }
            // FROM
            if(r.StartsWith("FROM",false)){
                return IMAP_Search_Key_From.Parse(r);
            }
            // HEADER
            if(r.StartsWith("HEADER",false)){
                return IMAP_Search_Key_Header.Parse(r);
            }
            // KEYWORD
            if(r.StartsWith("KEYWORD",false)){
                return IMAP_Search_Key_Keyword.Parse(r);
            }
            // LARGER
            if(r.StartsWith("LARGER",false)){
                return IMAP_Search_Key_Larger.Parse(r);
            }
            // NEW
            if(r.StartsWith("NEW",false)){
                return IMAP_Search_Key_New.Parse(r);
            }
            // NOT
            if(r.StartsWith("NOT",false)){
                return IMAP_Search_Key_Not.Parse(r);
            }
            // OLD
            if(r.StartsWith("OLD",false)){
                return IMAP_Search_Key_Old.Parse(r);
            }
            // ON
            if(r.StartsWith("ON",false)){
                return IMAP_Search_Key_On.Parse(r);
            }
            // OR
            if(r.StartsWith("OR",false)){
                return IMAP_Search_Key_Or.Parse(r);
            }
            // RECENT
            if(r.StartsWith("RECENT",false)){
                return IMAP_Search_Key_Recent.Parse(r);
            }
            // SEEN
            if(r.StartsWith("SEEN",false)){
                return IMAP_Search_Key_Seen.Parse(r);
            }
            // SENTBEFORE
            if(r.StartsWith("SENTBEFORE",false)){
                return IMAP_Search_Key_SentBefore.Parse(r);
            }
            // SENTON
            if(r.StartsWith("SENTON",false)){
                return IMAP_Search_Key_SentOn.Parse(r);
            }
            // SENTSINCE
            if(r.StartsWith("SENTSINCE",false)){
                return IMAP_Search_Key_SentSince.Parse(r);
            }
            // SEQSET
            if(r.StartsWith("SEQSET",false)){
                return IMAP_Search_Key_SeqSet.Parse(r);
            }
            // SINCE
            if(r.StartsWith("SINCE",false)){
                return IMAP_Search_Key_Since.Parse(r);
            }
            // SMALLER
            if(r.StartsWith("SMALLER",false)){
                return IMAP_Search_Key_Smaller.Parse(r);
            }
            // SUBJECT
            if(r.StartsWith("SUBJECT",false)){
                return IMAP_Search_Key_Subject.Parse(r);
            }
            // TEXT
            if(r.StartsWith("TEXT",false)){
                return IMAP_Search_Key_Text.Parse(r);
            }
            // TO
            if(r.StartsWith("TO",false)){
                return IMAP_Search_Key_To.Parse(r);
            }
            // UID
            if(r.StartsWith("UID",false)){
                return IMAP_Search_Key_Uid.Parse(r);
            }
            // UNANSWERED
            if(r.StartsWith("UNANSWERED",false)){
                return IMAP_Search_Key_Unanswered.Parse(r);
            }
            // UNDELETED
            if(r.StartsWith("UNDELETED",false)){
                return IMAP_Search_Key_Undeleted.Parse(r);
            }
            // UNDRAFT
            if(r.StartsWith("UNDRAFT",false)){
                return IMAP_Search_Key_Undraft.Parse(r);
            }
            // UNFLAGGED
            if(r.StartsWith("UNFLAGGED",false)){
                return IMAP_Search_Key_Unflagged.Parse(r);
            }
            // UNKEYWORD
            if(r.StartsWith("UNKEYWORD",false)){
                return IMAP_Search_Key_Unkeyword.Parse(r);
            }
            // UNSEEN
            if(r.StartsWith("UNSEEN",false)){
                return IMAP_Search_Key_Unseen.Parse(r);
            }
// Check if we hae sequence-set. Because of IMAP specification sucks a little here, why the hell they didn't
            // do the keyword(SEQSET) for it, like UID. Now we just have to try if it is sequence-set or BAD key.
            try{
                return IMAP_Search_Key_SeqSet.Parse(r);
            }
            catch{
                throw new ParseException("Unknown search key '" + r.ReadToEnd() + "'.");
            }
        }

        /// <summary>
        /// Stores IMAP search-key command parts to the specified array.
        /// </summary>
        /// <param name="list">Array where to store command parts.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>list</b> is null reference.</exception>
        internal abstract void ToCmdParts(List<IMAP_Client_CmdPart> list);
    }
}
