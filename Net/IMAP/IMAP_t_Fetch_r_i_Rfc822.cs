﻿using System;
using System.IO;

namespace LumiSoft.Net.IMAP
{
    /// <summary>
    /// This class represents IMAP FETCH response RFC822 data-item. Defined in RFC 3501 7.4.2.
    /// </summary>
    public class IMAP_t_Fetch_r_i_Rfc822 : IMAP_t_Fetch_r_i
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="stream">Message stream.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>stream</b> is null reference.</exception>
        public IMAP_t_Fetch_r_i_Rfc822(Stream stream)
        {
            if(stream == null){
                throw new ArgumentNullException("stream");
            }

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
        /// Gets message stream.
        /// </summary>
        public Stream Stream { get; private set; }
    }
}
