using System;
using System.IO;

namespace LumiSoft.Net.IMAP.Client
{
    /// <summary>
    /// This class provides data for <b cref="IMAP_Client.FetchGetStoreStream">IMAP_Client.FetchGetStoreStream</b> event.
    /// </summary>
    public class IMAP_Client_e_FetchGetStoreStream : EventArgs
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="fetch">Fetch response.</param>
        /// <param name="dataItem">Fetch data-item.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>fetch</b> or <b>dataItem</b> is null reference.</exception>
        public IMAP_Client_e_FetchGetStoreStream(IMAP_r_u_Fetch fetch, IMAP_t_Fetch_r_i dataItem)
        {
            FetchResponse = fetch ?? throw new ArgumentNullException("fetch");
            DataItem = dataItem ?? throw new ArgumentNullException("dataItem");
        }

        /// <summary>
        /// Gets FETCH data-item which stream to get.
        /// </summary>
        public IMAP_t_Fetch_r_i DataItem { get; }

        /// <summary>
        /// Gets related FETCH response.
        /// </summary>
        public IMAP_r_u_Fetch FetchResponse { get; }

        /// <summary>
        /// Gets stream where to store data-item data.
        /// </summary>
        public Stream Stream { get; set; }
    }
}
