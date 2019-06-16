﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace LumiSoft.Net.MIME
{
    /// <summary>
    /// Represents MIME header field parameters collection.
    /// </summary>
    public class MIME_h_ParameterCollection : IEnumerable
    {
        /// <summary>
        /// This class represents header field parameter builder.
        /// </summary>
        public class _ParameterBuilder
        {
            private readonly SortedList<int,string> m_pParts;
            private Encoding               m_pEncoding;

            /// <summary>
            /// Default constructor.
            /// </summary>
            /// <param name="name">Parameter name.</param>
            /// <exception cref="ArgumentNullException">Is raised when <b>name</b> is null reference.</exception>
            public _ParameterBuilder(string name)
            {
                if(name == null){
                    throw new ArgumentNullException("name");
                }

                Name = name;

                m_pParts = new SortedList<int,string>();
            }

            /// <summary>
            /// Adds header field parameter part to paramter buffer.
            /// </summary>
            /// <param name="index">Parameter part index.</param>
            /// <param name="encoded">If true parameter part is encoded.</param>
            /// <param name="value">Parameter part value.</param>
            public void AddPart(int index,bool encoded,string value)
            {
                // We should have charset and language information available.
                if(encoded && index == 0){
                    // Syntax: <charset>'<language>'<value>
                    string[] charset_language_value = value.Split('\'');
                    m_pEncoding = Encoding.GetEncoding(charset_language_value[0]);
                    value = charset_language_value[2];
                }

                // Add parameter to list.
                m_pParts.Add(index,value);
            }

            /// <summary>
            /// Gets header field parameter(splitted paramter values concated).
            /// </summary>
            /// <returns>Returns header field parameter.</returns>
            public MIME_h_Parameter GetParamter()
            {
                // Concate parts values and decode value. (SortedList takes care for sorting part indexes)
                StringBuilder value = new StringBuilder();
                foreach(KeyValuePair<int,string> v in m_pParts){
                    value.Append(v.Value);
                }

                if(m_pEncoding != null){
                    return new MIME_h_Parameter(Name,DecodeExtOctet(value.ToString(),m_pEncoding));
                }

                return new MIME_h_Parameter(Name,value.ToString());
            }

            /// <summary>
            /// Gets parameter name.
            /// </summary>
            public string Name { get; }
        }

        private bool                                m_IsModified;
        private readonly Dictionary<string,MIME_h_Parameter> m_pParameters;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="owner">Owner MIME header field.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>owner</b> is null reference.</exception>
        public MIME_h_ParameterCollection(MIME_h owner)
        {
            if(owner == null){
                throw new ArgumentNullException("owner");
            }

            Owner = owner;

            m_pParameters = new Dictionary<string,MIME_h_Parameter>(StringComparer.CurrentCultureIgnoreCase);
        }

        /// <summary>
        /// Removes specified parametr from the collection.
        /// </summary>
        /// <param name="name">Parameter name.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>name</b> is null reference.</exception>
        public void Remove(string name)
        {
            if(name == null){
                throw new ArgumentNullException("name");
            }

            if(m_pParameters.Remove(name)){
                m_IsModified = true;
            }
        }

        /// <summary>
        /// Removes all items from the collection.
        /// </summary>
        public void Clear()
        {
            m_pParameters.Clear();
            m_IsModified = true;
        }

        /// <summary>
        /// Copies header fields parameters to new array.
        /// </summary>
        /// <returns>Returns header fields parameters array.</returns>
        public MIME_h_Parameter[] ToArray()
        {
            MIME_h_Parameter[] retVal = new MIME_h_Parameter[m_pParameters.Count];
            m_pParameters.Values.CopyTo(retVal,0);

            return retVal;
        }

        /// <summary>
        /// Returns header field parameters as string.
        /// </summary>
        /// <returns>Returns header field parameters as string.</returns>
        public override string ToString()
        {
            return ToString(null);
        }

        /// <summary>
        /// Returns header field parameters as string.
        /// </summary>
        /// <param name="charset">Charset to use to encode 8-bit characters. Value null means parameters not encoded.</param>
        /// <returns>Returns header field parameters as string.</returns>
        public string ToString(Encoding charset)
        {
            /* RFC 2231.
             *      If parameter conatins 8-bit byte, we need to encode parameter value
             *      If parameter value length bigger than MIME maximum allowed line length,
             *      we need split value.
            */

            if(charset == null){
                charset = Encoding.Default;
            }

            StringBuilder retVal = new StringBuilder();
            foreach(MIME_h_Parameter parameter in this.ToArray()){
                if(string.IsNullOrEmpty(parameter.Value)){
                    retVal.Append(";\r\n\t" + parameter.Name);
                }
                // We don't need to encode or split value.
                else if((charset == null || Net_Utils.IsAscii(parameter.Value)) && parameter.Value.Length < 76){
                    retVal.Append(";\r\n\t" + parameter.Name + "=" + TextUtils.QuoteString(parameter.Value));
                }
                // We need to encode/split value.
                else{
                    byte[] byteValue = charset.GetBytes(parameter.Value);

                    List<string> values = new List<string>();            
                    // Do encoding/splitting.
                    int    offset    = 0;
                    char[] valueBuff = new char[50];
                    foreach(byte b in byteValue){                                        
                        // We need split value as RFC 2231 says.
                        if(offset >= (50 - 3)){
                            values.Add(new string(valueBuff,0,offset));
                            offset = 0;
                        }
                        
                        // Normal char, we don't need to encode.
                        if(MIME_Reader.IsAttributeChar((char)b)){
                            valueBuff[offset++] = (char)b;
                        }
                        // We need to encode byte as %X2.
                        else{
                            valueBuff[offset++] = '%';
                            valueBuff[offset++] = (b >> 4).ToString("X")[0];
                            valueBuff[offset++] = (b & 0xF).ToString("X")[0];
                        }
                    }
                    // Add pending buffer value.
                    if(offset > 0){
                        values.Add(new string(valueBuff,0,offset));
                    }

                    for(int i=0;i<values.Count;i++){
                        // Only fist value entry has charset and language info.
                        if(charset != null && i == 0){
                            retVal.Append(";\r\n\t" + parameter.Name + "*" + i.ToString() + "*=" + charset.WebName + "''" + values[i]);
                        }
                        else{
                            retVal.Append(";\r\n\t" + parameter.Name + "*" + i.ToString() + "*=" + values[i]);
                        }
                    }
                }
            }

            return retVal.ToString();
        }

        /// <summary>
        /// Parses parameters from the specified value.
        /// </summary>
        /// <param name="value">Header field parameters string.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>value</b> is null reference.</exception>
        public void Parse(string value)
        {
            if(value == null){
                throw new ArgumentNullException("value");
            }

            Parse(new MIME_Reader(value));
        }

        /// <summary>
        /// Parses parameters from the specified reader.
        /// </summary>
        /// <param name="reader">MIME reader.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>reader</b> is null reference.</exception>
        public void Parse(MIME_Reader reader)
        {
            if(reader == null){
                throw new ArgumentNullException("reader");
            }

            /* RFC 2231.
                Asterisks ("*") are reused to provide the indicator that language and
                character set information is present and encoding is being used. A
                single quote ("'") is used to delimit the character set and language
                information at the beginning of the parameter value. Percent signs
                ("%") are used as the encoding flag, which agrees with RFC 2047.
                         
                Character set and language information may be combined with the
                parameter continuation mechanism. For example:

                Content-Type: application/x-stuff
                    title*0*=us-ascii'en'This%20is%20even%20more%20
                    title*1*=%2A%2A%2Afun%2A%2A%2A%20
                    title*2="isn't it!"

                Note that:

                (1) Language and character set information only appear at
                    the beginning of a given parameter value.

                (2) Continuations do not provide a facility for using more
                    than one character set or language in the same
                    parameter value.

                (3) A value presented using multiple continuations may
                    contain a mixture of encoded and unencoded segments.

                (4) The first segment of a continuation MUST be encoded if
                    language and character set information are given.

                (5) If the first segment of a continued parameter value is
                    encoded the language and character set field delimiters
                    MUST be present even when the fields are left blank.
            */

            KeyValueCollection<string,_ParameterBuilder> parameters = new KeyValueCollection<string,_ParameterBuilder>();

            // Parse all parameter parts.
            string[] parameterParts = TextUtils.SplitQuotedString(reader.ToEnd(),';');
            foreach(string part in parameterParts){
                if(string.IsNullOrEmpty(part)){
                    continue;
                }

                string[] name_value = part.Trim().Split(new char[]{'='},2);
                string   paramName  = name_value[0].Trim();
                string   paramValue = null;
                if(name_value.Length == 2){
                    paramValue = TextUtils.UnQuoteString(name_value[1].Trim());
                }
                // Valueless parameter.
                //else{
                                
                string[] nameParts = paramName.Split('*');
                int      index     = 0;
                bool     encoded   = nameParts.Length == 3;
                // Get multi value parameter index.
                if(nameParts.Length >= 2){
                    try{
                        index = Convert.ToInt32(nameParts[1]);
                    }
                    catch{
                    }
                }

                // Single value parameter and we already have parameter with such name, skip it.
                if(nameParts.Length < 2 && parameters.ContainsKey(nameParts[0])){
                    continue;
                }

                // Parameter builder doesn't exist for the specified parameter, create it.
                if(!parameters.ContainsKey(nameParts[0])){
                    parameters.Add(nameParts[0],new _ParameterBuilder(nameParts[0]));
                }
              
                parameters[nameParts[0]].AddPart(index,encoded,paramValue);
            }

            // Build parameters from parts.
            foreach(_ParameterBuilder b in parameters){
                m_pParameters.Add(b.Name,b.GetParamter());
            }

            m_IsModified = false;
        }

        /// <summary>
        /// Decodes non-ascii text with MIME <b>ext-octet</b> method. Defined in RFC 2231 7.
        /// </summary>
        /// <param name="text">Text to decode,</param>
        /// <param name="charset">Charset to use.</param>
        /// <returns>Returns decoded text.</returns>
        /// <exception cref="ArgumentNullException">Is raised when <b>text</b> or <b>charset</b> is null.</exception>
        private static string DecodeExtOctet(string text,Encoding charset)
        {
            if(text == null){
                throw new ArgumentNullException("text");
            }
            if(charset == null){
                throw new ArgumentNullException("charset");
            }

            int    offset        = 0;
            byte[] decodedBuffer = new byte[text.Length];            
            for(int i=0;i<text.Length;i++){
                if(text[i] == '%'){
                    decodedBuffer[offset++] = byte.Parse(text[i + 1].ToString() + text[i + 2].ToString(),System.Globalization.NumberStyles.HexNumber);
                    i += 2;
                }
                else{
                    decodedBuffer[offset++] = (byte)text[i];
                }
            }

            return charset.GetString(decodedBuffer,0,offset);
        }

        /// <summary>
		/// Gets enumerator.
		/// </summary>
		/// <returns></returns>
		public IEnumerator GetEnumerator()
		{
			return m_pParameters.Values.GetEnumerator();
		}

        /// <summary>
        /// Gets if this header field parameters are modified since it has loaded.
        /// </summary>
        /// <remarks>All new added header fields has <b>IsModified = true</b>.</remarks>
        /// <exception cref="ObjectDisposedException">Is riased when this class is disposed and this property is accessed.</exception>
        public bool IsModified
        {
            get{
                if(m_IsModified){
                    return true;
                }

                foreach(MIME_h_Parameter parameter in this.ToArray()){
                    if(parameter.IsModified){
                        return true;
                    }
                }

                return false;
            }
        }

        /// <summary>
        /// Gets owner MIME header field.
        /// </summary>
        public MIME_h Owner { get; }

        /// <summary>
        /// Gets number of items in the collection.
        /// </summary>
        public int Count
        {
            get{ return m_pParameters.Count; }
        }

        /// <summary>
        /// Gets or sets specified header field parameter value. Value null means not specified.
        /// </summary>
        /// <param name="name">Header field name.</param>
        /// <returns>Returns specified header field value or null if specified parameter doesn't exist.</returns>
        /// <exception cref="ArgumentNullException">Is raised when <b>name</b> is null reference.</exception>
        public string this[string name]
        {
            get{
                if(name == null){
                    throw new ArgumentNullException("name");
                }

                MIME_h_Parameter retVal = null;
                if(m_pParameters.TryGetValue(name,out retVal)){
                    return retVal.Value;
                }

                return null;
            }

            set{
                if(name == null){
                    throw new ArgumentNullException("name");
                }

                MIME_h_Parameter retVal = null;
                if(m_pParameters.TryGetValue(name,out retVal)){
                    retVal.Value = value;
                }
                else{
                    m_pParameters.Add(name,new MIME_h_Parameter(name,value));
                }
            }
        }
    }
}
