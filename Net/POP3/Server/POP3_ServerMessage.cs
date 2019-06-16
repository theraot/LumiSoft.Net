﻿using System;

namespace LumiSoft.Net.POP3.Server
{
    /// <summary>
    /// This class represents POP3 server message.
    /// </summary>
    public class POP3_ServerMessage
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="uid">Message UID value.</param>
        /// <param name="size">Message size in bytes.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>uid</b> is null reference.</exception>
        /// <exception cref="ArgumentException">Is raised when any of the arguments has invalid value.</exception>
        public POP3_ServerMessage(string uid,int size) : this(uid,size,null)
        {
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="uid">Message UID value.</param>
        /// <param name="size">Message size in bytes.</param>
        /// <param name="tag">User data.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>uid</b> is null reference.</exception>
        /// <exception cref="ArgumentException">Is raised when any of the arguments has invalid value.</exception>
        public POP3_ServerMessage(string uid,int size,object tag)
        {
            if(uid == null){
                throw new ArgumentNullException("uid");
            }
            if(uid == string.Empty){
                throw new ArgumentException("Argument 'uid' value must be specified.");
            }
            if(size < 0){
                throw new ArgumentException("Argument 'size' value must be >= 0.");
            }

            UID  = uid;
            Size = size;
            Tag = tag;
        }


        /// <summary>
        /// Sets IsMarkedForDeletion proerty value.
        /// </summary>
        /// <param name="value">Value.</param>
        internal void SetIsMarkedForDeletion(bool value)
        {
            IsMarkedForDeletion = value;
        }


        /// <summary>
        /// Gets message UID. NOTE: Before accessing this property, check that server supports UIDL command.
        /// </summary>
        public string UID { get; } = "";

        /// <summary>
        /// Gets message size in bytes.
        /// </summary>
        public int Size { get; }

        /// <summary>
        /// Gets if message is marked for deletion.
        /// </summary>
        public bool IsMarkedForDeletion { get; private set; }

        /// <summary>
        /// Gets or sets user data.
        /// </summary>
        public object Tag { get; set; }


        /// <summary>
        /// Gets message 1 based sequence number.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Is raised when this object is disposed and this property is accessed.</exception>
        internal int SequenceNumber { get; set; } = -1;
    }
}
