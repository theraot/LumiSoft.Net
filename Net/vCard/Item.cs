using System;
using System.Text;

using LumiSoft.Net.MIME;

namespace LumiSoft.Net.Mime.vCard
{
    /// <summary>
    /// vCard structure item.
    /// </summary>
    public class Item
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="name">Item name.</param>
        /// <param name="parameters">Item parameters.</param>
        /// <param name="value">Item encoded value value.</param>
        internal Item(string name,string parameters,string value)
        {
            Name       = name;
            ParametersString = parameters;
            Value      = value;
        }


        /// <summary>
        /// Sets item decoded value. Value will be encoded as needed and stored to item.Value property.
        /// Also property item.ParametersString is updated to reflect right encoding(always base64, required by rfc) and charset (utf-8).
        /// </summary>
        /// <param name="value"></param>
        public void SetDecodedValue(string value)
        {
            /* RFC 2426 vCrad 5. Differences From vCard v2.1
                The QUOTED-PRINTABLE inline encoding has been eliminated.
                Only the "B" encoding of [RFC 2047] is an allowed value for
                the ENCODING parameter.
              
                The CRLF character sequence in a text type value is specified 
                with the backslash character sequence "\n" or "\N".
             
                Any COMMA or SEMICOLON in a text type value must be backslash escaped.
            */

            if(NeedEncode(value)){
                // Remove encoding and charset parameters
                string newParmString = "";
                string[] parameters = ParametersString.ToLower().Split(';');
                foreach(string parameter in parameters){
                    string[] name_value = parameter.Split('=');
                    if(name_value[0] == "encoding" || name_value[0] == "charset"){                        
                    }
                    else if(parameter.Length > 0){
                        newParmString += parameter + ";";
                    }
                }
                // Add encoding parameter
                newParmString += "ENCODING=b;CHARSET=utf-8";
      
                this.ParametersString = newParmString;
                this.Value = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(value));
            }
            else{
                this.Value = value;
            }
        }


        /// <summary>
        /// Converts item to vCal item string.
        /// </summary>
        /// <returns></returns>
        internal string ToItemString()
        {
            if(ParametersString.Length > 0){
                return Name + ";" + ParametersString + ":" + FoldData(Value);
            }
            else{
                return Name + ":" + FoldData(Value);
            }
        }


        /// <summary>
        /// CHecks if specified value must be encoded.
        /// </summary>
        /// <param name="value">String value.</param>
        /// <returns>Returns true if value must be encoded, otherwise false.</returns>
        private bool NeedEncode(string value)
        {
            // We have 8-bit chars.
            if(!Net_Utils.IsAscii(value)){
                return true;
            }

            // Allow only prontable chars and whitespaces.
            foreach(char c in value){
                if(!(char.IsLetterOrDigit(c) || char.IsWhiteSpace(c))){
                    return true;
                }
            }

            return false;
        }


        // Is it needed ?

        /// <summary>
        /// Folds long data line to folded lines.
        /// </summary>
        /// <param name="data">Data to fold.</param>
        /// <returns></returns>
        private string FoldData(string data)
        {
            /* Folding rules:
                *) Line may not be bigger than 76 chars.
                *) If possible fold between TAB or SP
                *) If no fold point, just fold from char 76
            */

            // Data line too big, we need to fold data.
            if(data.Length > 76){
                int startPosition       = 0;
                int lastPossibleFoldPos = -1;
                StringBuilder retVal = new StringBuilder();
                for(int i=0;i<data.Length;i++){
                    char c = data[i];
                    // We have possible fold point
                    if(c == ' ' || c == '\t'){
                        lastPossibleFoldPos = i;
                    }

                    // End of data reached
                    if(i == (data.Length - 1)){
                        retVal.Append(data.Substring(startPosition));
                    }
                    // We need to fold
                    else if((i - startPosition) >= 76){
                        // There wasn't any good fold point(word is bigger than line), just fold from current position.
                        if(lastPossibleFoldPos == -1){
                            lastPossibleFoldPos = i;
                        }
                    
                        retVal.Append(data.Substring(startPosition,lastPossibleFoldPos - startPosition) + "\r\n\t");

                        i = lastPossibleFoldPos;
                        lastPossibleFoldPos = -1;
                        startPosition       = i;
                    }
                }

                return retVal.ToString();
            }
            else{
                return data;
            }
        }


        /// <summary>
        /// Gest item name.
        /// </summary>
        public string Name { get; } = "";

        /// <summary>
        /// Gets or sets item parameters.
        /// </summary>
        public string ParametersString { get; set; } = "";

        /// <summary>
        /// Gets or sets item encoded value. NOTE: If you set this property value, you must encode data 
        /// by yourself and also set right ENCODING=encoding; and CHARSET=charset; prameter in item.ParametersString !!!
        /// Normally use method item.SetDecodedStringValue method instead, this does all you need.
        /// </summary>
        public string Value { get; set; } = "";

        /// <summary>
        /// Gets item decoded value. If param string specifies Encoding and/or Charset, 
        /// item.Value will be decoded accordingly.
        /// </summary>
        public string DecodedValue
        {
            /* RFC 2426 vCrad 5. Differences From vCard v2.1              
                The CRLF character sequence in a text type value is specified 
                with the backslash character sequence "\n" or "\N".
             
                Any COMMA or SEMICOLON in a text type value must be backslash escaped.
            */

            get{ 
                string data     = Value;
                string encoding = null;
                string charset  = null;
                string[] parameters = ParametersString.ToLower().Split(';');
                foreach(string parameter in parameters){
                    string[] name_value = parameter.Split('=');
                    if(name_value[0] == "encoding" && name_value.Length > 1){
                        encoding = name_value[1];
                    }
                    else if(name_value[0] == "charset" && name_value.Length > 1){
                        charset = name_value[1];
                    }
                }

                // Encoding specified, decode data.
                if(encoding != null){
                    if(encoding == "quoted-printable"){
                        data = System.Text.Encoding.Default.GetString(MIME_Utils.QuotedPrintableDecode(System.Text.Encoding.Default.GetBytes(data)));
                    }
                    else if(encoding == "b"){
                        data = System.Text.Encoding.Default.GetString(Net_Utils.FromBase64(System.Text.Encoding.Default.GetBytes(data)));
                    }
                    else{
                        throw new Exception("Unknown data encoding '" + encoding + "' !");
                    }
                }
                // Charset specified, convert data to specified charset.
                if(charset != null){
                    data = System.Text.Encoding.GetEncoding(charset).GetString(System.Text.Encoding.Default.GetBytes(data));
                }

                // FIX ME: this must be done with structured fields
                //data = data.Replace("\\n","\r\n");
                //data = TextUtils.UnEscapeString(data); Messes up structured fields

                return data; 
            }
        }
    }
}
