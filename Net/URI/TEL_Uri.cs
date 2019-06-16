﻿namespace LumiSoft.Net
{
    /// <summary>
    /// Implements TEL URI. Defined in RFC 2806.
    /// </summary>
    public class TEL_Uri : AbsoluteUri
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        internal TEL_Uri()
        {
        }

        public bool IsGlobal => false;

        public string PhoneNmber => "";
    }
}
