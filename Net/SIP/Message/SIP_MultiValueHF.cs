using System;
using System.Collections.Generic;
using System.Text;

namespace LumiSoft.Net.SIP.Message
{
    /// <summary>
    /// Implements generic multi value SIP header field.
    /// This is used by header fields like Via,Contact, ... .
    /// </summary>
    public class SIP_MultiValueHF<T> : SIP_HeaderField where T : SIP_t_Value,new()
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="name">Header field name.</param>
        /// <param name="value">Header field value.</param>
        public SIP_MultiValueHF(string name,string value) : base(name,value)
        {
            Values = new List<T>();

            SetMultiValue(true);

            Parse(value);
        }


        /// <summary>
        /// Parses multi value header field values.
        /// </summary>
        /// <param name="value">Header field value.</param>
        private void Parse(string value)
        {
            Values.Clear();
            
            StringReader r = new StringReader(value);
            while(r.Available > 0){
                r.ReadToFirstChar();
                // If we have COMMA, just consume it, it last value end.
                if(r.StartsWith(",")){
                    r.ReadSpecifiedLength(1);
                }

                // Allow xxx-param to pasre 1 value from reader.
                T param = new T();
                param.Parse(r);
                Values.Add(param);                
            }
        }

        /// <summary>
        /// Converts to valid mutli value header field value.
        /// </summary>
        /// <returns></returns>
        private string ToStringValue()
        {
            StringBuilder retVal = new StringBuilder();
            // Syntax: xxx-parm *(COMMA xxx-parm)
            for(int i=0;i<Values.Count;i++){
                retVal.Append(Values[i].ToStringValue());

                // Don't add comma for last item.
                if(i < Values.Count - 1){
                    retVal.Append(',');
                }
            }

            return retVal.ToString();
        }


        /// <summary>
        /// Gets header field values.
        /// </summary>
        /// <returns></returns>
        public object[] GetValues()
        {
            return Values.ToArray();
        }

        /// <summary>
        /// Removes value from specified index.
        /// </summary>
        /// <param name="index">Index of value to remove.</param>
        public void Remove(int index)
        {
            if(index > -1 && index < Values.Count){
                Values.RemoveAt(index);
            }
        }


        /// <summary>
        /// Gets or sets header field value.
        /// </summary>
        public override string Value
        {
            get{ return this.ToStringValue(); }

            set{
                if(value != null){
                    throw new ArgumentNullException("Property Value value may not be null !");
                }

                Parse(value);

                base.Value = value;
            }
        }

        /// <summary>
        /// Gets header field values.
        /// </summary>
        public List<T> Values { get; }

        /// <summary>
        /// Gets values count.
        /// </summary>
        public int Count
        {
            get{ return Values.Count; }
        }
    }
}
