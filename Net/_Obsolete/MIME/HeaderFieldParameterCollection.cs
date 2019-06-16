using System;
using System.Collections;

namespace LumiSoft.Net.Mime
{
    /// <summary>
    /// Header field parameters collection.
    /// </summary>
    [Obsolete("See LumiSoft.Net.MIME or LumiSoft.Net.Mail namepaces for replacement.")]
    public class HeaderFieldParameterCollection : IEnumerable
    {
        private readonly ParametizedHeaderField m_pHeaderField;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="headerField">Header field.</param>
        internal HeaderFieldParameterCollection(ParametizedHeaderField headerField)
        {
            m_pHeaderField = headerField;
        }

        /// <summary>
        /// Gets header field parameters count in the collection.
        /// </summary>
        public int Count => m_pHeaderField.ParseParameters().Count;

        /// <summary>
        /// Gets or sets specified parameter value.
        /// </summary>
        public string this[string parameterName]
        {
            get
            {
                parameterName = parameterName.ToLower();

                var parameters = m_pHeaderField.ParseParameters();
                if (!parameters.ContainsKey(parameterName))
                {
                    throw new Exception("Specified parameter '" + parameterName + "' doesn't exist !");
                }

                return (string)parameters[parameterName];
            }

            set
            {
                parameterName = parameterName.ToLower();

                var parameters = m_pHeaderField.ParseParameters();
                if (parameters.ContainsKey(parameterName))
                {
                    parameters[parameterName] = value;

                    m_pHeaderField.StoreParameters(m_pHeaderField.Value, parameters);
                }
            }
        }

        /// <summary>
        /// Adds a new header field parameter with specified name and value to the end of the collection.
        /// </summary>
        /// <param name="parameterName">Parameter name.</param>
        /// <param name="parameterValue">Parameter value.</param>
        public void Add(string parameterName, string parameterValue)
        {
            parameterName = parameterName.ToLower();

            var parameters = m_pHeaderField.ParseParameters();
            if (!parameters.ContainsKey(parameterName))
            {
                parameters.Add(parameterName, parameterValue);

                m_pHeaderField.StoreParameters(m_pHeaderField.Value, parameters);
            }
            else
            {
                throw new Exception("Header field '" + m_pHeaderField.Name + "' parameter '" + parameterName + "' already exists !");
            }
        }

        /// <summary>
        /// Clears the collection of all header field parameters.
        /// </summary>
        public void Clear()
        {
            var parameters = m_pHeaderField.ParseParameters();
            parameters.Clear();
            m_pHeaderField.StoreParameters(m_pHeaderField.Value, parameters);
        }

        /// <summary>
        /// Gets if collection contains specified parameter.
        /// </summary>
        /// <param name="parameterName">Parameter name.</param>
        /// <returns></returns>
        public bool Contains(string parameterName)
        {
            parameterName = parameterName.ToLower();

            var parameters = m_pHeaderField.ParseParameters();
            return parameters.ContainsKey(parameterName);
        }

        /// <summary>
        /// Gets enumerator.
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetEnumerator()
        {
            var parameters = m_pHeaderField.ParseParameters();
            var retVal = new HeaderFieldParameter[parameters.Count];
            int i = 0;
            foreach (DictionaryEntry entry in parameters)
            {
                retVal[i] = new HeaderFieldParameter(entry.Key.ToString(), entry.Value.ToString());
                i++;
            }

            return retVal.GetEnumerator();
        }

        /// <summary>
        /// Removes specified header field parameter from the collection.
        /// </summary>
        /// <param name="parameterName">The name of the header field parameter to remove.</param>
        public void Remove(string parameterName)
        {
            parameterName = parameterName.ToLower();

            var parameters = m_pHeaderField.ParseParameters();
            if (!parameters.ContainsKey(parameterName))
            {
                parameters.Remove(parameterName);

                m_pHeaderField.StoreParameters(m_pHeaderField.Value, parameters);
            }
        }
    }
}
