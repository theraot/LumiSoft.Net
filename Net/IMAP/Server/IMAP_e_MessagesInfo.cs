﻿using System;
using System.Collections.Generic;

namespace LumiSoft.Net.IMAP.Server
{
    /// <summary>
    /// This class provides data for <b cref="IMAP_Session.GetMessagesInfo">IMAP_Session.GetMessagesInfo</b> event.
    /// </summary>
    public class IMAP_e_MessagesInfo : EventArgs
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="folder">Folder name with optional path.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>folder</b> is null reference.</exception>
        internal IMAP_e_MessagesInfo(string folder)
        {
            if(folder == null){
                throw new ArgumentNullException("folder");
            }

            Folder = folder;

            MessagesInfo = new List<IMAP_MessageInfo>();
        }

        /// <summary>
        /// Gets folder name with optional path.
        /// </summary>
        public string Folder { get; }

        /// <summary>
        /// Gets messages info collection.
        /// </summary>
        public List<IMAP_MessageInfo> MessagesInfo { get; }

        /// <summary>
        /// Gets messages count.
        /// </summary>
        internal int Exists
        {
            get{ return MessagesInfo.Count; }
        }

        /// <summary>
        /// Gets messages count with recent flag set.
        /// </summary>
        internal int Recent
        {
            get{ 
                int count = 0;
                foreach(IMAP_MessageInfo m in MessagesInfo){
                    foreach(string flag in m.Flags){
                        if(string.Equals(flag,"Recent",StringComparison.InvariantCultureIgnoreCase)){
                            count++;
                            break;
                        }
                    }
                }

                return count; 
            }
        }

        /// <summary>
        /// Get messages first unseen message 1-based sequnece number. Returns -1 if no umseen messages.
        /// </summary>
        internal int FirstUnseen
        {
            get{
                for(int i=0;i<MessagesInfo.Count;i++){
                    if(!MessagesInfo[i].ContainsFlag("Seen")){
                        return i + 1;
                    }
                }

                return -1; 
            }
        }

        /// <summary>
        /// Gets messages count with seen flag not set.
        /// </summary>
        internal int Unseen
        {
            get{ 
                int count = MessagesInfo.Count;
                foreach(IMAP_MessageInfo m in MessagesInfo){
                    foreach(string flag in m.Flags){
                        if(string.Equals(flag,"Seen",StringComparison.InvariantCultureIgnoreCase)){
                            count--;
                            break;
                        }
                    }
                }

                return count; 
            }
        }

        /// <summary>
        /// Gets next message predicted UID value.
        /// </summary>
        internal long UidNext
        {
            get{ 
                long maxUID = 0;
                foreach(IMAP_MessageInfo m in MessagesInfo){
                    if(m.UID > maxUID){
                        maxUID = m.UID;
                    }
                }
                maxUID++;
                
                return maxUID; 
            }
        }
    }
}
