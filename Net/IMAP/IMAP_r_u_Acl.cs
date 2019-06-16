using System;
using System.Collections.Generic;
using System.Text;

namespace LumiSoft.Net.IMAP
{
    /// <summary>
    /// This class represents IMAP ACL response. Defined in RFC 4314 3.6.2.
    /// </summary>
    public class IMAP_r_u_Acl : IMAP_r_u
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="folderName">Folder name with path.</param>
        /// <param name="entries">ACL entries.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>folderName</b> or <b>entries</b> is null reference.</exception>
        public IMAP_r_u_Acl(string folderName,IMAP_Acl_Entry[] entries)
        {
            if(folderName == null){
                throw new ArgumentNullException("folderName");
            }
            if(folderName == string.Empty){
                throw new ArgumentException("Argument 'folderName' value must be specified.","folderName");
            }

            FolderName = folderName;
            Entires   = entries ?? throw new ArgumentNullException("entries");
        }

        /// <summary>
        /// Parses ACL response from acl-response string.
        /// </summary>
        /// <param name="aclResponse">ACL response.</param>
        /// <returns>Returns parsed ACL response.</returns>
        /// <exception cref="ArgumentNullException">Is raised wehn <b>aclResponse</b> is null reference.</exception>
        public static IMAP_r_u_Acl Parse(string aclResponse)
        {
            if(aclResponse == null){
                throw new ArgumentNullException("aclResponse");
            }

            /* RFC 4314 3.6. ACL Response.
                Data:       mailbox name
                            zero or more identifier rights pairs

                The ACL response occurs as a result of a GETACL command.  The first
                string is the mailbox name for which this ACL applies.  This is
                followed by zero or more pairs of strings; each pair contains the
                identifier for which the entry applies followed by the set of rights
                that the identifier has.
             
                Example:    C: A002 GETACL INBOX
                            S: * ACL INBOX Fred rwipsldexta
                            S: A002 OK Getacl complete
            */

            var r = new StringReader(aclResponse);
            // Eat "*"
            r.ReadWord();
            // Eat "ACL"
            r.ReadWord();

            var               folderName = TextUtils.UnQuoteString(IMAP_Utils.Decode_IMAP_UTF7_String(r.ReadWord()));
            var             items      = r.ReadToEnd().Split(' ');
            var entries    = new List<IMAP_Acl_Entry>();
            for (int i=0;i<items.Length;i+=2){
                entries.Add(new IMAP_Acl_Entry(items[i],items[i + 1]));
            }

            return new IMAP_r_u_Acl(folderName,entries.ToArray());
        }

        /// <summary>
        /// Returns this as string.
        /// </summary>
        /// <returns>Returns this as string.</returns>
        public override string ToString()
        {
            return ToString(IMAP_Mailbox_Encoding.None);
        }

        /// <summary>
        /// Returns this as string.
        /// </summary>
        /// <param name="encoding">Specifies how mailbox name is encoded.</param>
        /// <returns>Returns this as string.</returns>
        public override string ToString(IMAP_Mailbox_Encoding encoding)
        {
            // Example:    S: * ACL INBOX Fred rwipslda test rwipslda

            var retVal = new StringBuilder();
            retVal.Append("* ACL ");
            retVal.Append(IMAP_Utils.EncodeMailbox(FolderName,encoding));
            foreach(IMAP_Acl_Entry e in Entires){
                retVal.Append(" \"" + e.Identifier + "\" \"" + e.Rights + "\"");
            }
            retVal.Append("\r\n");

            return retVal.ToString();
        }

        /// <summary>
        /// Gets folder name.
        /// </summary>
        public string FolderName { get; } = "";

        /// <summary>
        /// Gets ACL entries.
        /// </summary>
        public IMAP_Acl_Entry[] Entires { get; }
    }
}
