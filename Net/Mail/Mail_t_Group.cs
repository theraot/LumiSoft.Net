using System.Collections.Generic;
using System.Text;

using LumiSoft.Net.MIME;

namespace LumiSoft.Net.Mail
{
    /// <summary>
    /// This class represents "group" address. Defined in RFC 5322 3.4.
    /// </summary>
    public class Mail_t_Group : Mail_t_Address
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="displayName">Display name. Value null means not specified.</param>
        public Mail_t_Group(string displayName)
        {
            DisplayName = displayName;

            Members = new List<Mail_t_Mailbox>();
        }


        /// <summary>
        /// Returns mailbox as string.
        /// </summary>
        /// <returns>Returns mailbox as string.</returns>
        public override string ToString()
        {
            return ToString(null);
        }

        /// <summary>
        /// Returns address as string value.
        /// </summary>
        /// <param name="wordEncoder">8-bit words ecnoder. Value null means that words are not encoded.</param>
        /// <returns>Returns address as string value.</returns>
        public override string ToString(MIME_Encoding_EncodedWord wordEncoder)
        {
            StringBuilder retVal = new StringBuilder();
            if(string.IsNullOrEmpty(DisplayName)){
                retVal.Append(":");
            }
            else{
                if(MIME_Encoding_EncodedWord.MustEncode(DisplayName)){
                    retVal.Append(wordEncoder.Encode(DisplayName) + ":");
                }
                else{
                    retVal.Append(TextUtils.QuoteString(DisplayName) + ":");
                }
            }
            for(int i=0;i<Members.Count;i++){
                retVal.Append(Members[i].ToString(wordEncoder));
                if(i < (Members.Count - 1)){
                    retVal.Append(",");
                }
            }
            retVal.Append(";");            

            return retVal.ToString();
        }


        /// <summary>
        /// Gets or sets diplay name. Value null means not specified.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets groiup address members collection.
        /// </summary>
        public List<Mail_t_Mailbox> Members { get; }
    }
}
