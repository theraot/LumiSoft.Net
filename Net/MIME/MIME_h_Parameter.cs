using System;

namespace LumiSoft.Net.MIME
{
    /// <summary>
    /// Represents MIME header field parameter.
    /// </summary>
    public class MIME_h_Parameter
    {
        private string m_Value      = "";

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="name">Parameter name.</param>
        /// <param name="value">Parameter value. Value null means not specified.</param>
        public MIME_h_Parameter(string name,string value)
        {
            if(name == null){
                throw new ArgumentNullException("name");
            }

            Name  = name;
            m_Value = value;
        }


        #region Properties implementation

        /// <summary>
        /// Gets if this header field parameter is modified since it has loaded.
        /// </summary>
        /// <remarks>All new added header fields parameters has <b>IsModified = true</b>.</remarks>
        /// <exception cref="ObjectDisposedException">Is riased when this class is disposed and this property is accessed.</exception>
        public bool IsModified { get; private set; }

        /// <summary>
        /// Gets parameter name.
        /// </summary>
        public string Name { get; } = "";

        /// <summary>
        /// Gets or sets parameter value. Value null means not specified.
        /// </summary>
        public string Value
        {
            get{ return m_Value; }

            set{ 
                m_Value      = value;
                IsModified = true;
            }
        }

        #endregion

    }
}
