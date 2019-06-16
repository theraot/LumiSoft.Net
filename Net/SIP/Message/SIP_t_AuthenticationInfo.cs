using System;
using System.Text;

namespace LumiSoft.Net.SIP.Message
{
    /// <summary>
    /// Implements SIP "Authentication-Info" value. Defined in RFC 3261.
    /// According RFC 3261 authentication info can contain Digest authentication info only.
    /// </summary>
    public class SIP_t_AuthenticationInfo : SIP_t_Value
    {
        private int    m_NonceCount   = -1;
      
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="value">Authentication-Info valu value.</param>
        public SIP_t_AuthenticationInfo(string value)
        {
            Parse(new StringReader(value));
        }

        /// <summary>
        /// Parses "Authentication-Info" from specified value.
        /// </summary>
        /// <param name="value">SIP "Authentication-Info" value.</param>
        /// <exception cref="ArgumentNullException">Raised when <b>value</b> is null.</exception>
        /// <exception cref="SIP_ParseException">Raised when invalid SIP message.</exception>
        public void Parse(string value)
        {
            if(value == null){
                throw new ArgumentNullException("value");
            }

            Parse(new StringReader(value));
        }

        /// <summary>
        /// Parses "Authentication-Info" from specified reader.
        /// </summary>
        /// <param name="reader">Reader what contains Authentication-Info value.</param>
        /// <exception cref="ArgumentNullException">Raised when <b>reader</b> is null.</exception>
        /// <exception cref="SIP_ParseException">Raised when invalid SIP message.</exception>
        public override void Parse(StringReader reader)
        {
            /*
                Authentication-Info  =  "Authentication-Info" HCOLON ainfo *(COMMA ainfo)
                ainfo                =  nextnonce / message-qop / response-auth / cnonce / nonce-count
                nextnonce            =  "nextnonce" EQUAL nonce-value
                response-auth        =  "rspauth" EQUAL response-digest
                response-digest      =  LDQUOT *LHEX RDQUOT
                nc-value             =  8LHEX
            */

            if(reader == null){
                throw new ArgumentNullException("reader");
            }

            while(reader.Available > 0){
                var word = reader.QuotedReadToDelimiter(',');
                if (word != null && word.Length > 0){
                    var name_value = word.Split(new char[]{'='},2);
                    if (name_value[0].ToLower() == "nextnonce"){
                        this.NextNonce = name_value[1];
                    }
                    else if(name_value[0].ToLower() == "qop"){
                        this.Qop = name_value[1];
                    }
                    else if(name_value[0].ToLower() == "rspauth"){
                        this.ResponseAuth = name_value[1];
                    }
                    else if(name_value[0].ToLower() == "cnonce"){
                        this.CNonce = name_value[1];
                    }
                    else if(name_value[0].ToLower() == "nc"){
                        this.NonceCount = Convert.ToInt32(name_value[1]);
                    }
                    else{
                        throw new SIP_ParseException("Invalid Authentication-Info value !");
                    }
                }
            }
        }

        /// <summary>
        /// Converts SIP_t_AuthenticationInfo to valid Authentication-Info value.
        /// </summary>
        /// <returns></returns>
        public override string ToStringValue()
        {
            /*
                Authentication-Info  =  "Authentication-Info" HCOLON ainfo *(COMMA ainfo)
                ainfo                =  nextnonce / message-qop / response-auth / cnonce / nonce-count
                nextnonce            =  "nextnonce" EQUAL nonce-value
                response-auth        =  "rspauth" EQUAL response-digest
                response-digest      =  LDQUOT *LHEX RDQUOT
                nc-value             =  8LHEX
            */

            var retVal = new StringBuilder();

            if (NextNonce != null){
                retVal.Append("nextnonce=" + NextNonce);
            }

            if(Qop != null){
                if(retVal.Length > 0){
                    retVal.Append(',');
                }

                retVal.Append("qop=" + Qop);
            }

            if(ResponseAuth != null){
                if(retVal.Length > 0){
                    retVal.Append(',');
                }

                retVal.Append("rspauth=" + TextUtils.QuoteString(ResponseAuth));
            }

            if(CNonce != null){
                if(retVal.Length > 0){
                    retVal.Append(',');
                }

                retVal.Append("cnonce=" + CNonce);
            }
            
            if(m_NonceCount != -1){                
                if(retVal.Length > 0){
                    retVal.Append(',');
                }

                retVal.Append("nc=" + m_NonceCount.ToString("X8"));
            }

            return retVal.ToString();
        }

        /// <summary>
        /// Gets or sets server next predicted nonce value. Value null means that value not specified.
        /// </summary>
        public string NextNonce { get; set; }

        /// <summary>
        /// Gets or sets QOP value. Value null means that value not specified.
        /// </summary>
        public string Qop { get; set; }

        /// <summary>
        /// Gets or sets rspauth value. Value null means that value not specified.
        /// This can be only HEX value.
        /// </summary>
        public string ResponseAuth { get; set; }

        /// <summary>
        /// Gets or sets cnonce value. Value null means that value not specified.
        /// </summary>
        public string CNonce { get; set; }

        /// <summary>
        /// Gets or sets nonce count. Value -1 means that value not specified.
        /// </summary>
        public int NonceCount
        {
            get{ return m_NonceCount; }

            set{
                if(value < 0){
                    m_NonceCount = -1;
                }
                else{
                    m_NonceCount = value;
                }
            }
        }
    }
}
