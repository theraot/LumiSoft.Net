using System;

namespace LumiSoft.Net.WebDav
{
    /// <summary>
    /// This class represents WebDav default property.
    /// </summary>
    public class WebDav_p_Default : WebDav_p
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="nameSpace">Property namespace.</param>
        /// <param name="name">Property name.</param>
        /// <param name="value">Property value.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>name</b> is null reference.</exception>
        /// <exception cref="ArgumentException">Is raised when any of the arguments has invalid value.</exception>
        public WebDav_p_Default(string nameSpace, string name, string value)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            if (name == string.Empty)
            {
                throw new ArgumentException("Argument 'name' value must be specified.");
            }

            Namespace = nameSpace;
            Name = name;
            Value = value;
        }

        /// <summary>
        /// Gets property name.
        /// </summary>
        public override string Name { get; }

        /// <summary>
        /// Gets property namespace.
        /// </summary>
        public override string Namespace { get; } = "";

        /// <summary>
        /// Gets property value.
        /// </summary>
        public override string Value { get; }
    }
}
