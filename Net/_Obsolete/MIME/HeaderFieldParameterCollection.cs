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
        private readonly ParametizedHeaderField _headerField;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="headerField">Header field.</param>
        internal HeaderFieldParameterCollection(ParametizedHeaderField headerField)
        {
            _headerField = headerField;
        }

        /// <summary>
        /// Gets header field parameters count in the collection.
        /// </summary>
        public int Count => _headerField.ParseParameters().Count;

        /// <summary>
        /// Gets or sets specified parameter value.
        /// </summary>
        public string this[string parameterName]
        {
            get
            {
                parameterName = parameterName.ToLower();

                var parameters = _headerField.ParseParameters();
                if (!parameters.ContainsKey(parameterName))
                {
                    throw new Exception("Specified parameter '" + parameterName + "' doesn't exist !");
                }

                return (string)parameters[parameterName];
            }

            set
            {
                parameterName = parameterName.ToLower();

                var parameters = _headerField.ParseParameters();
                if (parameters.ContainsKey(parameterName))
                {
                    parameters[parameterName] = value;

                    _headerField.StoreParameters(_headerField.Value, parameters);
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

            var parameters = _headerField.ParseParameters();
            if (!parameters.ContainsKey(parameterName))
            {
                parameters.Add(parameterName, parameterValue);

                _headerField.StoreParameters(_headerField.Value, parameters);
            }
            else
            {
                throw new Exception("Header field '" + _headerField.Name + "' parameter '" + parameterName + "' already exists !");
            }
        }

        /// <summary>
        /// Clears the collection of all header field parameters.
        /// </summary>
        public void Clear()
        {
            var parameters = _headerField.ParseParameters();
            parameters.Clear();
            _headerField.StoreParameters(_headerField.Value, parameters);
        }

        /// <summary>
        /// Gets if collection contains specified parameter.
        /// </summary>
        /// <param name="parameterName">Parameter name.</param>
        /// <returns></returns>
        public bool Contains(string parameterName)
        {
            parameterName = parameterName.ToLower();

            var parameters = _headerField.ParseParameters();
            return parameters.ContainsKey(parameterName);
        }

        /// <summary>
        /// Gets enumerator.
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetEnumerator()
        {
            var parameters = _headerField.ParseParameters();
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

            var parameters = _headerField.ParseParameters();
            if (!parameters.ContainsKey(parameterName))
            {
                parameters.Remove(parameterName);

                _headerField.StoreParameters(_headerField.Value, parameters);
            }
        }
    }
}
