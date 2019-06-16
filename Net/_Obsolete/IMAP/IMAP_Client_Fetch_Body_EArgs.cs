using System;
using System.IO;

namespace LumiSoft.Net.IMAP.Client
{
    /// <summary>
    /// This class represents FETCH BODY[] data item. Defined in RFC 3501.
    /// </summary>
    [Obsolete("Use Fetch(bool uid,IMAP_t_SeqSet seqSet,IMAP_t_Fetch_i[] items,EventHandler<EventArgs<IMAP_r_u>> callback) intead.")]
    public class IMAP_Client_Fetch_Body_EArgs : EventArgs
    {
        /// <summary>
        /// Defualt constructor.
        /// </summary>
        /// <param name="bodySection">Body section value.</param>
        /// <param name="offset">Body data offset.</param>
        internal IMAP_Client_Fetch_Body_EArgs(string bodySection, int offset)
        {
            BodySection = bodySection;
            Offset = offset;
        }

        /// <summary>
        /// This method is called when message storing has completed.
        /// </summary>
        public event EventHandler StoringCompleted;

        /// <summary>
        /// Gets BODY section value. Value null means not specified(full message).
        /// </summary>
        public string BodySection { get; }

        /// <summary>
        /// Gets BODY data returning start offset. Value null means not specified.
        /// </summary>
        public int Offset { get; } = -1;

        /// <summary>
        /// Gets or sets stream where BODY data is stored.
        /// </summary>
        public Stream Stream { get; set; }

        /// <summary>
        /// Raises <b>StoringCompleted</b> event.
        /// </summary>
        internal void OnStoringCompleted()
        {
            if (StoringCompleted != null)
            {
                StoringCompleted(this, new EventArgs());
            }
        }
    }
}
