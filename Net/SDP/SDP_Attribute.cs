using System;

namespace LumiSoft.Net.SDP
{
    /// <summary>
    /// Implements SDP attribute.
    /// </summary>
    public class SDP_Attribute
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="name">Attribute name.</param>
        /// <param name="value">Attribute value.</param>
        public SDP_Attribute(string name,string value)
        {
            Name     = name;
            this.Value = value;
        }


        /// <summary>
        /// Parses media from "a" SDP message field.
        /// </summary>
        /// <param name="aValue">"a" SDP message field.</param>
        /// <returns></returns>
        public static SDP_Attribute Parse(string aValue)
        {
            // a=<attribute>
            // a=<attribute>:<value>

            // Remove a=
            StringReader r = new StringReader(aValue);
            r.QuotedReadToDelimiter('=');

            //--- <attribute> ------------------------------------------------------------
            string name = "";
            string word = r.QuotedReadToDelimiter(':');
            if(word == null){
                throw new Exception("SDP message \"a\" field <attribute> name is missing !");
            }
            name = word;

            //--- <value> ----------------------------------------------------------------
            string value ="";
            word = r.ReadToEnd();
            if(word != null){
                value = word;
            }

            return new SDP_Attribute(name,value);
        }

        /// <summary>
        /// Converts this to valid "a" string.
        /// </summary>
        /// <returns></returns>
        public string ToValue()
        {
            // a=<attribute>
            // a=<attribute>:<value>

            // a=<attribute>
            if(string.IsNullOrEmpty(Value)){
                return "a=" + Name + "\r\n";
            }
            // a=<attribute>:<value>
            else{
                return "a=" + Name + ":" + Value + "\r\n";
            }
        }


        /// <summary>
        /// Gets attribute name.
        /// </summary>
        public string Name { get; } = "";

        /// <summary>
        /// Gets or sets attribute value.
        /// </summary>
        public string Value { get; set; } = "";
    }
}
