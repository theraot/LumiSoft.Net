using System;
using System.Text;

namespace LumiSoft.Net.IMAP.Client
{
    /// <summary>
    /// This class represents IMAP client selected folder.
    /// </summary>
    public class IMAP_Client_SelectedFolder
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="name">Folder name with path.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>name</b> is null reference value.</exception>
        /// <exception cref="ArgumentException">Is riased when any of the arguments has invalid value.</exception>
        public IMAP_Client_SelectedFolder(string name)
        {
            if(name == null){
                throw new ArgumentNullException("name");
            }
            if(name == string.Empty){
                throw new ArgumentException("The argument 'name' value must be specified.","name");
            }

            Name = name;
        }

        /// <summary>
        /// Returns this object as human readable string.
        /// </summary>
        /// <returns>Returns this object as human readable string.</returns>
        public override string ToString()
        {
            StringBuilder retVal = new StringBuilder();
            retVal.AppendLine("Name: "                + this.Name);
            retVal.AppendLine("UidValidity: "         + this.UidValidity);
            retVal.AppendLine("Flags: "               + StringArrayToString(this.Flags));
            retVal.AppendLine("PermanentFlags: "      + StringArrayToString(this.PermanentFlags));
            retVal.AppendLine("IsReadOnly: "          + this.IsReadOnly);
            retVal.AppendLine("UidNext: "             + this.UidNext);
            retVal.AppendLine("FirstUnseen: "         + this.FirstUnseen);
            retVal.AppendLine("MessagesCount: "       + this.MessagesCount);
            retVal.AppendLine("RecentMessagesCount: " + this.RecentMessagesCount);

            return retVal.ToString();
        }

        /// <summary>
        /// Sets UidValidity property value.
        /// </summary>
        /// <param name="value">Value to set.</param>
        internal void SetUidValidity(long value)
        {
            UidValidity = value;
        }

        /// <summary>
        /// Sets Flags property value.
        /// </summary>
        /// <param name="value">Value to set.</param>
        internal void SetFlags(string[] value)
        {
            Flags = value;
        }

        /// <summary>
        /// Sets PermanentFlags property value.
        /// </summary>
        /// <param name="value">Value to set.</param>
        internal void SetPermanentFlags(string[] value)
        {
            PermanentFlags = value;
        }

        /// <summary>
        /// Sets IsReadOnly property value.
        /// </summary>
        /// <param name="value">Value to set.</param>
        internal void SetReadOnly(bool value)
        {
            IsReadOnly = value;
        }

        /// <summary>
        /// Sets UidNext property value.
        /// </summary>
        /// <param name="value">Value to set.</param>
        internal void SetUidNext(long value)
        {
            UidNext = value;
        }

        /// <summary>
        /// Sets FirstUnseen property value.
        /// </summary>
        /// <param name="value">Value to set.</param>
        internal void SetFirstUnseen(int value)
        {
            FirstUnseen = value;
        }

        /// <summary>
        /// Sets MessagesCount property value.
        /// </summary>
        /// <param name="value">Value to set.</param>
        internal void SetMessagesCount(int value)
        {
            MessagesCount = value;
        }

        /// <summary>
        /// Sets RecentMessagesCount property value.
        /// </summary>
        /// <param name="value">Value to set.</param>
        internal void SetRecentMessagesCount(int value)
        {
            RecentMessagesCount = value;
        }

        /// <summary>
        /// Coneverts string array to comma separated value.
        /// </summary>
        /// <param name="value">String array.</param>
        /// <returns>Returns string array as comma separated value.</returns>
        private string StringArrayToString(string[] value)
        {
            StringBuilder retVal = new StringBuilder();

            for(int i=0;i<value.Length;i++){
                // Last item.
                if(i == (value.Length - 1)){
                    retVal.Append(value[i]);
                }
                else{
                    retVal.Append(value[i] + ",");
                }
            }

            return retVal.ToString();
        }

        /// <summary>
        /// Gets selected folder name(path included).
        /// </summary>
        public string Name { get; } = "";

        /// <summary>
        /// Gets folder UID value. Value null means IMAP server doesn't support <b>UIDVALIDITY</b> feature.
        /// </summary>
        public long UidValidity { get; private set; } = -1;

        /// <summary>
        /// Gets flags what folder supports.
        /// </summary>
        public string[] Flags { get; private set; } = new string[0];

        /// <summary>
        /// Gets permanent flags what folder can store.
        /// </summary>
        public string[] PermanentFlags { get; private set; } = new string[0];

        /// <summary>
        /// Gets if folder is read-only or read-write.
        /// </summary>
        public bool IsReadOnly { get; private set; }

        /// <summary>
        /// Gets next predicted message UID. Value -1 means that IMAP server doesn't support it.
        /// </summary>
        public long UidNext { get; private set; } = -1;

        /// <summary>
        /// Gets first unseen message sequence number. Value -1 means no unseen message.
        /// </summary>
        public int FirstUnseen { get; private set; } = -1;

        /// <summary>
        /// Gets number of messages in this folder.
        /// </summary>
        public int MessagesCount { get; private set; }

        /// <summary>
        /// Gets number of recent messages in this folder.
        /// </summary>
        public int RecentMessagesCount { get; private set; }
    }
}
