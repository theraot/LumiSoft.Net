using System;

namespace LumiSoft.Net.IMAP
{
    /// <summary>
    /// This class represents IMAP NAMESPACE entry. Defined in RFC 2342 5.
    /// </summary>
    public class IMAP_Namespace_Entry
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="name">Namespace name.</param>
        /// <param name="delimiter">Hierarchy delimiter char.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>name</b> is null reference.</exception>
        /// <exception cref="ArgumentException">Is raised when any of the arguments has invalid value.</exception>
        public IMAP_Namespace_Entry(string name,char delimiter)
        {
            if(name == null){
                throw new ArgumentNullException("name");
            }

            NamespaceName = name;
        }


        /// <summary>
        /// Gets namespace name.
        /// </summary>
        public string NamespaceName { get; } = "";

        /// <summary>
        /// Gets namespace hierarchy delimiter char.
        /// </summary>
        public char HierarchyDelimiter { get; } = '/';
    }
}
