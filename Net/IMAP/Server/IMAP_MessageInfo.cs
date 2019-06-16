using System;
using System.Text;

namespace LumiSoft.Net.IMAP.Server
{
    /// <summary>
    /// This class represents IMAP message info.
    /// </summary>
    public class IMAP_MessageInfo
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="id">Message ID.</param>
        /// <param name="uid">Message IMAP UID value.</param>
        /// <param name="flags">Message flags.</param>
        /// <param name="size">Message size in bytes.</param>
        /// <param name="internalDate">Message IMAP internal date.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>id</b> or <b>flags</b> is null reference.</exception>
        /// <exception cref="ArgumentException">Is raised when any of the arguments has invalid value.</exception>
        public IMAP_MessageInfo(string id,long uid,string[] flags,int size,DateTime internalDate)
        {
            if(id == null){
                throw new ArgumentNullException("id");
            }
            if(id == string.Empty){
                throw new ArgumentException("Argument 'id' value must be specified.","id");
            }
            if(uid < 1){
                throw new ArgumentException("Argument 'uid' value must be >= 1.","uid");
            }
            if(flags == null){
                throw new ArgumentNullException("flags");
            }

            ID           = id;
            UID          = uid;
            Flags       = flags;
            Size         = size;
            InternalDate = internalDate;
        }


        #region method ContainsFlag

        /// <summary>
        /// Gets if this message info contains specified message flag.
        /// </summary>
        /// <param name="flag">Message flag.</param>
        /// <returns>Returns true if message info contains specified message flag.</returns>
        public bool ContainsFlag(string flag)
        {
            if(flag == null){
                throw new ArgumentNullException("flag");
            }

            foreach(string f in Flags){
                if(string.Equals(f,flag,StringComparison.InvariantCultureIgnoreCase)){
                    return true;
                }
            }

            return false;
        }

        #endregion


        #region method FlagsToImapString

        /// <summary>
        /// Flags to IMAP flags string.
        /// </summary>
        /// <returns>Returns IMAP flags string.</returns>
        internal string FlagsToImapString()
        {
            StringBuilder retVal = new StringBuilder();
            retVal.Append("(");
            for(int i=0;i<Flags.Length;i++){
                if(i > 0){
                    retVal.Append(" ");
                }

                retVal.Append("\\" + Flags[i]);
            }
            retVal.Append(")");

            return retVal.ToString();
        }

        #endregion

        #region method UpdateFlags

        /// <summary>
        /// Updates IMAP message flags.
        /// </summary>
        /// <param name="setType">Flags set type.</param>
        /// <param name="flags">IMAP message flags.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>flags</b> is null reference.</exception>
        internal void UpdateFlags(IMAP_Flags_SetType setType,string[] flags)
        {
            if(flags == null){
                throw new ArgumentNullException("flags");
            }

            if(setType == IMAP_Flags_SetType.Add){
                Flags = IMAP_Utils.MessageFlagsAdd(Flags,flags);
            }
            else if(setType == IMAP_Flags_SetType.Remove){
                Flags = IMAP_Utils.MessageFlagsRemove(Flags,flags);
            }
            else{
                Flags = flags;
            }
        }

        #endregion


        #region Properties implementation

        /// <summary>
        /// Gets message ID value.
        /// </summary>
        public string ID { get; }

        /// <summary>
        /// Gets message IMAP UID value.
        /// </summary>
        public long UID { get; }

        /// <summary>
        /// Gets message flags.
        /// </summary>
        public string[] Flags { get; private set; }

        /// <summary>
        /// Gets message size in bytes.
        /// </summary>
        public int Size { get; }

        /// <summary>
        /// Gets message IMAP internal date.
        /// </summary>
        public DateTime InternalDate { get; }


        /// <summary>
        /// Gets or sets message one-based sequnece number.
        /// </summary>
        internal int SeqNo { get; set; } = 1;

#endregion
    }
}
