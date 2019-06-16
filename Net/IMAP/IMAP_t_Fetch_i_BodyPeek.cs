using System.Text;

namespace LumiSoft.Net.IMAP
{
    /// <summary>
    /// This class represents FETCH request BODY.PEEK[] argument(data-item). Defined in RFC 3501.
    /// </summary>
    public class IMAP_t_Fetch_i_BodyPeek : IMAP_t_Fetch_i
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public IMAP_t_Fetch_i_BodyPeek()
        {
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="section">Body section. Value null means not specified.</param>
        /// <param name="offset">Data returning offset. Value -1 means not specified.</param>
        /// <param name="maxCount">Maximum number of bytes to return. Value -1 means not specified.</param>
        public IMAP_t_Fetch_i_BodyPeek(string section,int offset,int maxCount)
        {
            Section  = section;
            Offset   = offset;
            MaxCount = maxCount;
        }


        /// <summary>
        /// Returns this as string.
        /// </summary>
        /// <returns>Returns this as string.</returns>
        public override string ToString()
        {
            StringBuilder retVal = new StringBuilder();
            retVal.Append("BODY.PEEK[");
            if(Section != null){
                retVal.Append(Section);
            }
            retVal.Append("]");                        
            if(Offset > -1){
                retVal.Append("<" + Offset);
                if(MaxCount > -1){
                    retVal.Append("." + MaxCount);
                }
                retVal.Append(">");
            }

            return retVal.ToString();
        }


        /// <summary>
        /// Gets body section. Value null means not specified.
        /// </summary>
        public string Section { get; }

        /// <summary>
        /// Gets start offset. Value -1 means not specified.
        /// </summary>
        public int Offset { get; } = -1;

        /// <summary>
        /// Gets maximum count of bytes to fetch. Value -1 means not specified.
        /// </summary>
        public int MaxCount { get; } = -1;
    }
}
