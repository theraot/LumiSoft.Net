using System;
using System.Text;

namespace LumiSoft.Net.IMAP
{
    /// <summary>
    /// This class represents IMAP MYRIGHTS response. Defined in RFC 4314 3.7.
    /// </summary>
    public class IMAP_r_u_ListRights : IMAP_r_u
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="folder">Folder name with path.</param>
        /// <param name="identifier">Identifier name. Normally this is user or group name.</param>
        /// <param name="requiredRights">Required rights.</param>
        /// <param name="optionalRights">Optional rights.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>folder</b> or <b>identifier</b> is null reference.</exception>
        /// <exception cref="ArgumentException">Is raised when any of the arguments has invalid value.</exception>
        public IMAP_r_u_ListRights(string folder,string identifier,string requiredRights,string optionalRights)
        {
            if(folder == null){
                throw new ArgumentNullException("folder");
            }
            if(folder == string.Empty){
                throw new ArgumentException("Argument 'folder' name must be specified.","folder");
            }
            if(identifier == null){
                throw new ArgumentNullException("identifier");
            }
            if(identifier == string.Empty){
                throw new ArgumentException("Argument 'identifier' name must be specified.","identifier");
            }

            FolderName     = folder;
            Identifier     = identifier;
            RequiredRights = requiredRights == string.Empty ? null : requiredRights;
            OptionalRights = optionalRights == string.Empty ? null : optionalRights;
        }

        /// <summary>
        /// Parses LISTRIGHTS response from LISTRIGHTS-response string.
        /// </summary>
        /// <param name="listRightsResponse">LISTRIGHTS response line.</param>
        /// <returns>Returns parsed LISTRIGHTS response.</returns>
        /// <exception cref="ArgumentNullException">Is raised when <b>listRightsResponse</b> is null reference.</exception>
        public static IMAP_r_u_ListRights Parse(string listRightsResponse)
        {
            if(listRightsResponse == null){
                throw new ArgumentNullException("listRightsResponse");
            }

            /* RFC 4314 3.7. LISTRIGHTS Response.
                Data:       mailbox name
                            identifier
                            required rights
                            list of optional rights

                The LISTRIGHTS response occurs as a result of a LISTRIGHTS command.
                The first two strings are the mailbox name and identifier for which
                this rights list applies.  Following the identifier is a string
                containing the (possibly empty) set of rights the identifier will
                always be granted in the mailbox.

                Following this are zero or more strings each containing a set of
                rights the identifier can be granted in the mailbox.  Rights
                mentioned in the same string are tied together.  The server MUST
                either grant all tied rights to the identifier in the mailbox or
                grant none.  Section 2.1.1 details additional server requirements
                related to handling of the virtual "d" and "c" rights.

                The same right MUST NOT be listed more than once in the LISTRIGHTS
                command.
              
                Example:    C: a001 LISTRIGHTS ~/Mail/saved smith
                            S: * LISTRIGHTS ~/Mail/saved smith la r swicdkxte
                            S: a001 OK Listrights completed
            */

            StringReader r = new StringReader(listRightsResponse);
            // Eat "*"
            r.ReadWord();
            // Eat "LISTRIGHTS"
            r.ReadWord();

            string folder     = IMAP_Utils.Decode_IMAP_UTF7_String(r.ReadWord(true));
            string identifier = r.ReadWord(true);
            string reqRights  = r.ReadWord(true);
            string optRights  = r.ReadWord(true);

            return new IMAP_r_u_ListRights(folder,identifier,reqRights,optRights);
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
            // Example:    S: * LISTRIGHTS ~/Mail/saved smith la r swicdkxte

            StringBuilder retVal = new StringBuilder();
            retVal.Append("* LISTRIGHTS " + IMAP_Utils.EncodeMailbox(FolderName,encoding) + " \"" + RequiredRights + "\" " + OptionalRights + "\r\n");
            
            return retVal.ToString();
        }

        /// <summary>
        /// Gets folder name.
        /// </summary>
        public string FolderName { get; } = "";

        /// <summary>
        /// Gets identifier. Normaly this is user or group name.
        /// </summary>
        public string Identifier { get; } = "";

        /// <summary>
        /// Gets required rights.
        /// </summary>
        public string RequiredRights { get; }

        /// <summary>
        /// Gets optional rights.
        /// </summary>
        public string OptionalRights { get; }
    }
}
