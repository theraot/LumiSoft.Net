using System;

namespace LumiSoft.Net.SIP.Message
{
    /// <summary>
    /// Implements single value header field.
    /// Used by header fields like To:,From:,CSeq:, ... .
    /// </summary>
    public class SIP_SingleValueHF<T> : SIP_HeaderField where T : SIP_t_Value
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="name">Header field name.</param>
        /// <param name="value">Header field value.</param>
        public SIP_SingleValueHF(string name,T value) : base(name,"")
        {
            ValueX = value;
        }

        /// <summary>
        /// Parses single value from specified reader.
        /// </summary>
        /// <param name="reader">Reader what contains </param>
        public void Parse(StringReader reader)
        {
            ValueX.Parse(reader);
        }

        /// <summary>
        /// Convert this to string value.
        /// </summary>
        /// <returns>Returns this as string value.</returns>
        public string ToStringValue()
        {
            return ValueX.ToStringValue();
        }

// FIX ME: Change base class Value property or this to new name

        /// <summary>
        /// Gets or sets header field value.
        /// </summary>
        public override string Value
        {
            get{ return ToStringValue(); }

            set{ 
                if(value == null){
                    throw new ArgumentNullException("Property Value value may not be null !");
                }

                Parse(new StringReader(value)); 
            }
        }

        /// <summary>
        /// Gets or sets header field value.
        /// </summary>
        public T ValueX { get; set; }
    }
}
