namespace LumiSoft.Net.Mime.vCard
{
    /// <summary>
    /// vCard name implementation.
    /// </summary>
    public class Name
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="lastName">Last name.</param>
        /// <param name="firstName">First name.</param>
        /// <param name="additionalNames">Comma separated additional names.</param>
        /// <param name="honorificPrefix">Honorific prefix.</param>
        /// <param name="honorificSuffix">Honorific suffix.</param>
        public Name(string lastName,string firstName,string additionalNames,string honorificPrefix,string honorificSuffix)
        {
            LastName        = lastName;
            FirstName       = firstName;
            AdditionalNames = additionalNames;
            HonorificPerfix = honorificPrefix;
            HonorificSuffix = honorificSuffix;
        }

        /// <summary>
        /// Internal parse constructor.
        /// </summary>
        internal Name()
        {
        }

        /// <summary>
        /// Converts item to vCard N structure string.
        /// </summary>
        /// <returns></returns>
        public string ToValueString()
        {
            return LastName + ";" + FirstName + ";" + AdditionalNames + ";" + HonorificPerfix + ";" + HonorificSuffix;
        }

        /// <summary>
        /// Parses name info from vCard N item.
        /// </summary>
        /// <param name="item">vCard N item.</param>
        internal static Name Parse(Item item)
        {       
            string[] items = item.DecodedValue.Split(';');
            Name name = new Name();
            if(items.Length >= 1){
                name.LastName = items[0];
            }
            if(items.Length >= 2){
                name.FirstName = items[1];
            }
            if(items.Length >= 3){
                name.AdditionalNames = items[2];
            }
            if(items.Length >= 4){
                name.HonorificPerfix = items[3];
            }
            if(items.Length >= 5){
                name.HonorificSuffix = items[4];
            }
            return name;
        }

        /// <summary>
        /// Gets last name.
        /// </summary>
        public string LastName { get; private set; } = "";

        /// <summary>
        /// Gets first name.
        /// </summary>
        public string FirstName { get; private set; } = "";

        /// <summary>
        /// Gets comma separated additional names.
        /// </summary>
        public string AdditionalNames { get; private set; } = "";

        /// <summary>
        /// Gets honorific prefix.
        /// </summary>
        public string HonorificPerfix { get; private set; } = "";

        /// <summary>
        /// Gets honorific suffix.
        /// </summary>
        public string HonorificSuffix { get; private set; } = "";
    }
}
