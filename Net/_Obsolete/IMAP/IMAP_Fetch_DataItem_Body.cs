using System;
using System.Text;

namespace LumiSoft.Net.IMAP
{
    /// <summary>
    /// This class represents FETCH BODY[] data item. Defined in RFC 3501.
    /// </summary>
    [Obsolete("Use Fetch(bool uid,IMAP_t_SeqSet seqSet,IMAP_t_Fetch_i[] items,EventHandler<EventArgs<IMAP_r_u>> callback) intead.")]
    public class IMAP_Fetch_DataItem_Body : IMAP_Fetch_DataItem
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public IMAP_Fetch_DataItem_Body()
        {
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="section">Body section. Value null means not specified.</param>
        /// <param name="offset">Data returning offset. Value -1 means not specified.</param>
        /// <param name="maxCount">Maximum number of bytes to return. Value -1 means not specified.</param>
        public IMAP_Fetch_DataItem_Body(string section, int offset, int maxCount)
        {
            Section = section;
            Offset = offset;
            MaxCount = maxCount;
        }

        /// <summary>
        /// Gets maximum count of bytes to fetch. Value -1 means not specified.
        /// </summary>
        public int MaxCount { get; } = -1;

        /// <summary>
        /// Gets start offset. Value -1 means not specified.
        /// </summary>
        public int Offset { get; } = -1;

        /// <summary>
        /// Gets body section. Value null means not specified.
        /// </summary>
        public string Section { get; }

        /// <summary>
        /// Returns this as string.
        /// </summary>
        /// <returns>Returns this as string.</returns>
        public override string ToString()
        {
            var retVal = new StringBuilder();
            retVal.Append("BODY[");
            if (Section != null)
            {
                retVal.Append(Section);
            }
            retVal.Append("]");
            if (Offset > -1)
            {
                retVal.Append("<" + Offset);
                if (MaxCount > -1)
                {
                    retVal.Append("." + MaxCount);
                }
                retVal.Append(">");
            }

            return retVal.ToString();
        }
    }
}
