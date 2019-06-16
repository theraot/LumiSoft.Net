namespace LumiSoft.Net.WebDav
{
    /// <summary>
    /// This class is base class for any WebDav property.
    /// </summary>
    public abstract class WebDav_p
    {
        /// <summary>
        /// Gets property name.
        /// </summary>
        public abstract string Name
        {
            get;
        }
        /// <summary>
        /// Gets property namespace.
        /// </summary>
        public abstract string Namespace
        {
            get;
        }

        /// <summary>
        /// Gets property value.
        /// </summary>
        public abstract string Value
        {
            get;
        }
    }
}
