using System;
using System.Text;

namespace LumiSoft.Net.IMAP
{
    /// <summary>
    /// This class represents IMAP FLAGS response. Defined in RFC 3501 7.2.6.
    /// </summary>
    public class IMAP_r_u_Flags : IMAP_r_u
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="flags">Mailbox flags list.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>flags</b> is null reference.</exception>
        public IMAP_r_u_Flags(string[] flags)
        {
            Flags = flags ?? throw new ArgumentNullException("flags");
        }

        /// <summary>
        /// Parses FLAGS response from exists-response string.
        /// </summary>
        /// <param name="response">Exists response string.</param>
        /// <returns>Returns parsed flags response.</returns>
        /// <exception cref="ArgumentNullException">Is raised when <b>response</b> is null reference.</exception>
        public static IMAP_r_u_Flags Parse(string response)
        {
            if(response == null){
                throw new ArgumentNullException("response");
            }

            /* RFC 3501 7.2.6. FLAGS Response.                         
                Contents:   flag parenthesized list

                The FLAGS response occurs as a result of a SELECT or EXAMINE
                command.  The flag parenthesized list identifies the flags (at a
                minimum, the system-defined flags) that are applicable for this
                mailbox.  Flags other than the system flags can also exist,
                depending on server implementation.

                The update from the FLAGS response MUST be recorded by the client.

                Example:    S: * FLAGS (\Answered \Flagged \Deleted \Seen \Draft)
            */

            var r = new StringReader(response.Split(new[]{' '},3)[2]);

            return new IMAP_r_u_Flags(r.ReadParenthesized().Split(' '));
        }

        /// <summary>
        /// Returns this as string.
        /// </summary>
        /// <returns>Returns this as string.</returns>
        public override string ToString()
        {
            // Example:    S: * FLAGS (\Answered \Flagged \Deleted \Seen \Draft)

            var retVal = new StringBuilder();
            retVal.Append("* FLAGS (");
            for(int i=0;i<Flags.Length;i++){
                if(i > 0){
                    retVal.Append(" ");
                }
                retVal.Append(Flags[i]);
            }
            retVal.Append(")\r\n");

            return retVal.ToString();
        }

        /// <summary>
        /// Gets mailbox supported flags.
        /// </summary>
        public string[] Flags { get; }
    }
}
