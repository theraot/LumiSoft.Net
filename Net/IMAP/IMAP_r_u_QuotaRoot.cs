using System;
using System.Collections.Generic;
using System.Text;

namespace LumiSoft.Net.IMAP
{
    /// <summary>
    /// This class represents IMAP QUOTAROOT response. Defined in RFC 2087 5.2.
    /// </summary>
    public class IMAP_r_u_QuotaRoot : IMAP_r_u
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="folder">Folder name with path.</param>
        /// <param name="quotaRoots">Quota roots.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>folder</b> or <b>quotaRoots</b> is null reference.</exception>
        /// <exception cref="ArgumentException">Is raised when any of the arguments has invalid value.</exception>
        public IMAP_r_u_QuotaRoot(string folder,string[] quotaRoots)
        {
            if(folder == null){
                throw new ArgumentNullException("folder");
            }
            if(folder == string.Empty){
                throw new ArgumentException("Argument 'folder' name must be specified.","folder");
            }

            FolderName = folder;
            QuotaRoots = quotaRoots ?? throw new ArgumentNullException("quotaRoots");
        }

        /// <summary>
        /// Parses QUOTAROOT response from quotaRoot-response string.
        /// </summary>
        /// <param name="response">QUOTAROOT response string.</param>
        /// <returns>Returns parsed QUOTAROOT response.</returns>
        /// <exception cref="ArgumentNullException">Is raised when <b>response</b> is null reference.</exception>
        public static IMAP_r_u_QuotaRoot Parse(string response)
        {
            if(response == null){
                throw new ArgumentNullException("response");
            }

            /* RFC 2087 5.2. QUOTAROOT Response.
                Data:       mailbox name
                            zero or more quota root names

                This response occurs as a result of a GETQUOTAROOT command.  The
                first string is the mailbox and the remaining strings are the
                names of the quota roots for the mailbox.

                Example:    S: * QUOTAROOT INBOX ""
                            S: * QUOTAROOT comp.mail.mime
            */

            var r = new StringReader(response);
            // Eat "*"
            r.ReadWord();
            // Eat "QUOTAROOT"
            r.ReadWord();

            var folderName = TextUtils.UnQuoteString(IMAP_Utils.Decode_IMAP_UTF7_String(r.ReadWord()));
            var quotaRoots = new List<string>();
            while (r.Available > 0){
                var quotaRoot = r.ReadWord();
                if (quotaRoot != null){
                    quotaRoots.Add(quotaRoot);
                }
                else{
                    break;
                }
            }

            return new IMAP_r_u_QuotaRoot(folderName,quotaRoots.ToArray());
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
            // Example:    S: * QUOTAROOT INBOX ""

            var retVal = new StringBuilder();
            retVal.Append("* QUOTAROOT " + IMAP_Utils.EncodeMailbox(FolderName,encoding));
            foreach(string root in QuotaRoots){
                retVal.Append(" \"" + root + "\"");
            }
            retVal.Append("\r\n");

            return retVal.ToString();
        }

        /// <summary>
        /// Gets folder name.
        /// </summary>
        public string FolderName { get; } = "";

        /// <summary>
        /// Gets quota roots.
        /// </summary>
        public string[] QuotaRoots { get; }
    }
}
