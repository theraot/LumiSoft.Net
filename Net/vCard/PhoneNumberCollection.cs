using System.Collections;
using System.Collections.Generic;

namespace LumiSoft.Net.Mime.vCard
{
    /// <summary>
    /// vCard phone number collection implementation.
    /// </summary>
    public class PhoneNumberCollection : IEnumerable
    {
        private readonly vCard             m_pOwner;
        private readonly List<PhoneNumber> m_pCollection;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="owner">Owner vCard.</param>
        internal PhoneNumberCollection(vCard owner)
        {
            m_pOwner      = owner;
            m_pCollection = new List<PhoneNumber>();

            foreach(Item item in owner.Items.Get("TEL")){
                m_pCollection.Add(PhoneNumber.Parse(item));
            }
        }

        /// <summary>
        /// Add new phone number to the collection.
        /// </summary>
        /// <param name="type">Phone number type. Note: This value can be flagged value !</param>
        /// <param name="number">Phone number.</param>
        public void Add(PhoneNumberType_enum type,string number)
        {            
            var item = m_pOwner.Items.Add("TEL",PhoneNumber.PhoneTypeToString(type),number);
            m_pCollection.Add(new PhoneNumber(item,type,number));
        }

        /// <summary>
        /// Removes specified item from the collection.
        /// </summary>
        /// <param name="item">Item to remove.</param>
        public void Remove(PhoneNumber item)
        {
            m_pOwner.Items.Remove(item.Item);
            m_pCollection.Remove(item);
        }

        /// <summary>
        /// Removes all items from the collection.
        /// </summary>
        public void Clear()
        {
            foreach(PhoneNumber number in m_pCollection){
                m_pOwner.Items.Remove(number.Item);
            }
            m_pCollection.Clear();
        }

        /// <summary>
		/// Gets enumerator.
		/// </summary>
		/// <returns></returns>
		public IEnumerator GetEnumerator()
		{
			return m_pCollection.GetEnumerator();
		}

        /// <summary>
        /// Gets number of items in the collection.
        /// </summary>
        public int Count
        {
            get{ return m_pCollection.Count; }
        }
    }
}
