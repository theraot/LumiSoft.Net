using System;

namespace LumiSoft.Net.SIP.Message
{
    /// <summary>
    /// This class represents SIP value parameter.
    /// </summary>
    public class SIP_Parameter
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="name">Parameter name.</param>
        public SIP_Parameter(string name) : this(name,"")
        {
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="name">Parameter name.</param>
        /// <param name="value">Parameter value.</param>
        public SIP_Parameter(string name,string value)
        {
            if(name == null){
                throw new ArgumentNullException("name");
            }
            if(name == ""){
                throw new ArgumentException("Parameter 'name' value may no be empty string !");
            }

            Name  = name;
            Value = value;
        }


        #region Properties Implementation

        /// <summary>
        /// Gets parameter name.
        /// </summary>
        public string Name { get; } = "";

        /// <summary>
        /// Gets or sets parameter name. Value null means value less tag prameter.
        /// </summary>
        public string Value { get; set; } = "";

#endregion

    }
}
