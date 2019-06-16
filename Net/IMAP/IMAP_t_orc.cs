using System;

namespace LumiSoft.Net.IMAP
{
    /// <summary>
    /// This is base class for any IMAP server optional response codes. Defined in RFC 3501 7.1.
    /// </summary>
    public abstract class IMAP_t_orc
    {
        /// <summary>
        /// Parses IMAP optional response from string.
        /// </summary>
        /// <param name="value">Optional response string.</param>
        /// <returns>Returns parsed optional response.</returns>
        /// <exception cref="ArgumentNullException">Is raised when <b>value</b> is null reference.</exception>
        public static IMAP_t_orc Parse(string value)
        {
            if(value == null){
                throw new ArgumentNullException("value");
            }

            string responseCode = value.Split(' ')[0];
            if(string.Equals("ALERT",responseCode,StringComparison.InvariantCultureIgnoreCase)){
                return IMAP_t_orc_Alert.Parse(value);
            }

            if(string.Equals("BADCHARSET",responseCode,StringComparison.InvariantCultureIgnoreCase)){
                return IMAP_t_orc_BadCharset.Parse(value);
            }
            if(string.Equals("CAPABILITY",responseCode,StringComparison.InvariantCultureIgnoreCase)){
                return IMAP_t_orc_Capability.Parse(value);
            }
            if(string.Equals("PARSE",responseCode,StringComparison.InvariantCultureIgnoreCase)){
                return IMAP_t_orc_Parse.Parse(value);
            }
            if(string.Equals("PERMANENTFLAGS",responseCode,StringComparison.InvariantCultureIgnoreCase)){
                return IMAP_t_orc_PermanentFlags.Parse(value);
            }
            if(string.Equals("READ-ONLY",responseCode,StringComparison.InvariantCultureIgnoreCase)){
                return IMAP_t_orc_ReadOnly.Parse(value);
            }
            if(string.Equals("READ-WRITE",responseCode,StringComparison.InvariantCultureIgnoreCase)){
                return IMAP_t_orc_ReadWrite.Parse(value);
            }
            if(string.Equals("TRYCREATE",responseCode,StringComparison.InvariantCultureIgnoreCase)){
                return IMAP_t_orc_TryCreate.Parse(value);
            }
            if(string.Equals("UIDNEXT",responseCode,StringComparison.InvariantCultureIgnoreCase)){
                return IMAP_t_orc_UidNext.Parse(value);
            }
            if(string.Equals("UIDVALIDITY",responseCode,StringComparison.InvariantCultureIgnoreCase)){
                return IMAP_t_orc_UidValidity.Parse(value);
            }
            if(string.Equals("UNSEEN",responseCode,StringComparison.InvariantCultureIgnoreCase)){
                return IMAP_t_orc_Unseen.Parse(value);
            }
            //---------------------
            if(string.Equals("APPENDUID",responseCode,StringComparison.InvariantCultureIgnoreCase)){
                return IMAP_t_orc_AppendUid.Parse(value);
            }
            if(string.Equals("COPYUID",responseCode,StringComparison.InvariantCultureIgnoreCase)){
                return IMAP_t_orc_CopyUid.Parse(value);
            }
            return IMAP_t_orc_Unknown.Parse(value);
        }
    }
}
