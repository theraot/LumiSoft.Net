using System;
using System.IO;

namespace LumiSoft.Net.IMAP
{
    /// <summary>
    /// This class represents IMAP FETCH response BODY[] data-item. Defined in RFC 3501 7.4.2.
    /// </summary>
    public class IMAP_t_Fetch_r_i_Body : IMAP_t_Fetch_r_i
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="section">Body section value. Value null means not specified(full message).</param>
        /// <param name="offset">Data starting offset. Value -1 means not specified.</param>
        /// <param name="stream">Data stream.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>stream</b> is null reference.</exception>
        public IMAP_t_Fetch_r_i_Body(string section,int offset,Stream stream)
        {
            if(stream == null){
                throw new ArgumentNullException("stream");
            }

            BodySection = section;
            Offset  = offset;
            Stream = stream;
        }

        /// <summary>
        /// Sets Stream property value.
        /// </summary>
        /// <param name="stream">Stream.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>stream</b> is null reference.</exception>
        internal void SetStream(Stream stream)
        {
            if(stream == null){
                throw new ArgumentNullException("stream");
            }

            Stream = stream;
        }

        /// <summary>
        /// Gets BODY section value. Value null means not specified(full message).
        /// </summary>
        public string BodySection { get; }

        /// <summary>
        /// Gets BODY data returning start offset. Value -1 means not specified.
        /// </summary>
        public int Offset { get; } = -1;

        /// <summary>
        /// Gets data stream.
        /// </summary>
        public Stream Stream { get; private set; }
    }
}
