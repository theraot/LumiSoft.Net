namespace LumiSoft.Net.SIP.Message
{
    /// <summary>
    /// Represents SIP message header field.
    /// </summary>
    public class SIP_HeaderField
    {
        private string m_Value = "";

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="name">Header field name.</param>
        /// <param name="value">Header field value.</param>
        internal SIP_HeaderField(string name, string value)
        {
            Name = name;
            m_Value = value;
        }

        /// <summary>
        /// Gets if header field is multi value header field.
        /// </summary>
        public bool IsMultiValue { get; private set; }

        /// <summary>
        /// Gets header field name.
        /// </summary>
        public string Name { get; } = "";

        /// <summary>
        /// Gets or sets header field value.
        /// </summary>
        public virtual string Value
        {
            get { return m_Value; }

            set { m_Value = value; }
        }

        /// <summary>
        /// Sets property IsMultiValue value.
        /// </summary>
        /// <param name="value">Value to set.</param>
        internal void SetMultiValue(bool value)
        {
            IsMultiValue = value;
        }
    }
}
