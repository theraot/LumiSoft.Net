﻿using System;
using System.IO;

namespace LumiSoft.Net.IMAP.Server
{
    /// <summary>
    /// This class provides data for <b cref="IMAP_Session.Append">IMAP_Session.Append</b> event.
    /// </summary>
    public class IMAP_e_Append : EventArgs
    {
        private IMAP_r_ServerStatus m_pResponse;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="folder">Folder name with optional path.</param>
        /// <param name="flags">Message flags.</param>
        /// <param name="date">IMAP internal date. Value DateTime.MinValue means not specified.</param>
        /// <param name="size">Message size in bytes.</param>
        /// <param name="response">Default IMAP server response.</param>
        /// <exception cref="ArgumentNullException">Is riased when <b>folder</b>,<b>flags</b> or <b>response</b> is null reference.</exception>
        internal IMAP_e_Append(string folder,string[] flags,DateTime date,int size,IMAP_r_ServerStatus response)
        {
            Folder    = folder ?? throw new ArgumentNullException("folder");
            Flags    = flags ?? throw new ArgumentNullException("flags");
            InternalDate      = date;
            Size      = size;
            m_pResponse = response ?? throw new ArgumentNullException("response");
        }

        /// <summary>
        /// Gets or sets IMAP server response to this operation.
        /// </summary>
        /// <exception cref="ArgumentNullException">Is raised when null reference value set.</exception>
        public IMAP_r_ServerStatus Response
        {
            get{ return m_pResponse; }

            set{
                m_pResponse = value ?? throw new ArgumentNullException("value"); 
            }
        }

        /// <summary>
        /// Gets folder name with optional path.
        /// </summary>
        public string Folder { get; }

        /// <summary>
        /// Gets message flags.
        /// </summary>
        public string[] Flags { get; }

        /// <summary>
        /// Gets message internal date. Value DateTime.MinValue means not specified.
        /// </summary>
        public DateTime InternalDate { get; } = DateTime.MinValue;

        /// <summary>
        /// Gets message size in bytes.
        /// </summary>
        public int Size { get; }

        /// <summary>
        /// Gets or sets message stream.
        /// </summary>
        public Stream Stream { get; set; }

        /// <summary>
        /// This event is raised when message storing has completed.
        /// </summary>
        public event EventHandler Completed;

        /// <summary>
        /// Raises <b>Completed</b> event.
        /// </summary>
        internal void OnCompleted()
        {
            if(Completed != null){
                Completed(this,new EventArgs());
            }

            // Release event.
            Completed = null;
        }
    }
}
