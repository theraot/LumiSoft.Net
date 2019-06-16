﻿using System;
using System.Text;

namespace LumiSoft.Net.IMAP
{
    /// <summary>
    /// This class represents IMAP MYRIGHTS response. Defined in RFC 4314 3.8.
    /// </summary>
    public class IMAP_r_u_MyRights : IMAP_r_u
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="folder">Folder name with path.</param>
        /// <param name="rights">Rights values.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>folder</b> is null reference.</exception>
        /// <exception cref="ArgumentException">Is raised when any of the arguments has invalid value.</exception>
        public IMAP_r_u_MyRights(string folder,string rights)
        {
            if(folder == null){
                throw new ArgumentNullException("folder");
            }
            if(folder == string.Empty){
                throw new ArgumentException("Argument 'folder' value must be specified.","folder");
            }

            FolderName = folder;
            
            Rights = rights;
        }

        /// <summary>
        /// Parses MYRIGHTS response from MYRIGHTS-response string.
        /// </summary>
        /// <param name="myRightsResponse">MYRIGHTS response line.</param>
        /// <returns>Returns parsed MYRIGHTS response.</returns>
        /// <exception cref="ArgumentNullException">Is raised when <b>myRightsResponse</b> is null reference.</exception>
        public static IMAP_r_u_MyRights Parse(string myRightsResponse)
        {
            if(myRightsResponse == null){
                throw new ArgumentNullException("myRightsResponse");
            }

            /* RFC 4314 3.8. MYRIGHTS Response.
                Data:       mailbox name
                            rights

                The MYRIGHTS response occurs as a result of a MYRIGHTS command.  The
                first string is the mailbox name for which these rights apply.  The
                second string is the set of rights that the client has.

                Section 2.1.1 details additional server requirements related to
                handling of the virtual "d" and "c" rights.
             
                Example:    C: A003 MYRIGHTS INBOX
                            S: * MYRIGHTS INBOX rwiptsldaex
                            S: A003 OK Myrights complete
            */

            var r = new StringReader(myRightsResponse);
            // Eat "*"
            r.ReadWord();
            // Eat "MYRIGHTS"
            r.ReadWord();

            var folder = IMAP_Utils.Decode_IMAP_UTF7_String(r.ReadWord(true));
            var rights = r.ReadToEnd().Trim();

            return new IMAP_r_u_MyRights(folder,rights);
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
            // Example:    S: * MYRIGHTS INBOX rwiptsldaex

            var retVal = new StringBuilder();
            retVal.Append("* MYRIGHTS " + IMAP_Utils.EncodeMailbox(FolderName,encoding) + " \"" + Rights + "\"\r\n");

            return retVal.ToString();
        }

        /// <summary>
        /// Gets folder name.
        /// </summary>
        public string FolderName { get; } = "";

        /// <summary>
        /// Gets rights list.
        /// </summary>
        public string Rights { get; }
    }
}
