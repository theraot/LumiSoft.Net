﻿using System;
using System.Text;

namespace LumiSoft.Net.MIME
{
    /// <summary>
    /// This class represent header field what parsing has failed.
    /// </summary>
    public class MIME_h_Unparsed : MIME_h
    {
        private readonly string    m_ParseValue;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="value">Header field value. Header field name must be included. For example: 'Content-Type: text/plain'.</param>
        /// <param name="exception">Parsing error.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>value</b> is null reference.</exception>
        /// <exception cref="ParseException">Is raised when header field parsing errors.</exception>
        internal MIME_h_Unparsed(string value,Exception exception)
        {
            if(value == null){
                throw new ArgumentNullException("value");
            }
            
            string[] name_value = value.Split(new char[]{':'},2);
            if(name_value.Length != 2){
                throw new ParseException("Invalid Content-Type: header field value '" + value + "'.");
            }

            Name       = name_value[0];
            Value      = name_value[1].Trim();
            m_ParseValue = value;
            Exception = exception;
        }

        /// <summary>
        /// Parses header field from the specified value.
        /// </summary>
        /// <param name="value">Header field value. Header field name must be included. For example: 'Content-Type: text/plain'.</param>
        /// <returns>Returns parsed header field.</returns>
        /// <exception cref="InvalidOperationException">Is alwyas raised when this mewthod is accsessed.</exception>
        public static MIME_h_Unparsed Parse(string value)
        {
            throw new InvalidOperationException();
        }

        /// <summary>
        /// Returns header field as string.
        /// </summary>
        /// <param name="wordEncoder">8-bit words ecnoder. Value null means that words are not encoded.</param>
        /// <param name="parmetersCharset">Charset to use to encode 8-bit characters. Value null means parameters not encoded.</param>
        /// <param name="reEncode">If true always specified encoding is used. If false and header field value not modified, original encoding is kept.</param>
        /// <returns>Returns header field as string.</returns>
        public override string ToString(MIME_Encoding_EncodedWord wordEncoder,Encoding parmetersCharset,bool reEncode)
        {
            return m_ParseValue;
        }

        /// <summary>
        /// Gets if this header field is modified since it has loaded.
        /// </summary>
        /// <remarks>All new added header fields has <b>IsModified = true</b>.</remarks>
        /// <exception cref="ObjectDisposedException">Is riased when this class is disposed and this property is accessed.</exception>
        public override bool IsModified
        {
            get{ return false; }
        }

        /// <summary>
        /// Gets header field name.
        /// </summary>
        public override string Name { get; }

        /// <summary>
        /// Gets header field value.
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Gets error happened during parse.
        /// </summary>
        public Exception Exception { get; }
    }
}
