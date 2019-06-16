namespace LumiSoft.Net.Mime.vCard
{
    /// <summary>
    /// vCard email address implementation.
    /// </summary>
    public class EmailAddress
    {
        private EmailAddressType_enum m_Type         = EmailAddressType_enum.Internet;
        private string                m_EmailAddress = "";
                
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="item">Owner vCard item.</param>
        /// <param name="type">Email type. Note: This value can be flagged value !</param>
        /// <param name="emailAddress">Email address.</param>
        internal EmailAddress(Item item,EmailAddressType_enum type,string emailAddress)
        {
            Item        = item;
            m_Type         = type;
            m_EmailAddress = emailAddress;
        }


        /// <summary>
        /// This method is called when some property has changed, wee need to update underlaying vCard item.
        /// </summary>
        private void Changed()
        {
            Item.ParametersString = EmailTypeToString(m_Type);
            Item.SetDecodedValue(m_EmailAddress);
        }


        /// <summary>
        /// Parses email address from vCard EMAIL structure string.
        /// </summary>
        /// <param name="item">vCard EMAIL item.</param>
        internal static EmailAddress Parse(Item item)
        {
            EmailAddressType_enum type = EmailAddressType_enum.NotSpecified;
            if(item.ParametersString.ToUpper().IndexOf("PREF") != -1){
                type |= EmailAddressType_enum.Preferred;
            }
            if(item.ParametersString.ToUpper().IndexOf("INTERNET") != -1){
                type |= EmailAddressType_enum.Internet;
            }
            if(item.ParametersString.ToUpper().IndexOf("X400") != -1){
                type |= EmailAddressType_enum.X400;
            }

            return new EmailAddress(item,type,item.DecodedValue);
        }

        /// <summary>
        /// Converts EmailAddressType_enum to vCard item parameters string.
        /// </summary>
        /// <param name="type">Value to convert.</param>
        /// <returns></returns>
        internal static string EmailTypeToString(EmailAddressType_enum type)
        {
            string retVal = "";
            if((type & EmailAddressType_enum.Internet) != 0){
                retVal += "INTERNET,";
            }
            if((type & EmailAddressType_enum.Preferred) != 0){
                retVal += "PREF,";
            }
            if((type & EmailAddressType_enum.X400) != 0){
                retVal += "X400,";
            }
            if(retVal.EndsWith(",")){
                retVal = retVal.Substring(0,retVal.Length - 1);
            }

            return retVal;
        }


        /// <summary>
        /// Gets underlaying vCrad item.
        /// </summary>
        public Item Item { get; }

        /// <summary>
        /// Gets or sets email type. Note: This property can be flagged value !
        /// </summary>
        public EmailAddressType_enum EmailType
        {
            get{ return m_Type; }

            set{ 
                m_Type = value; 
                Changed();
            }
        }

        /// <summary>
        /// Gets or sets email address.
        /// </summary>
        public string Email
        {
            get{ return m_EmailAddress; }

            set{ 
                m_EmailAddress = value; 
                Changed();
            }
        }
    }
}
