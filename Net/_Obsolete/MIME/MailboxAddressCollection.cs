using System;
using System.Collections;
using System.Collections.Generic;

namespace LumiSoft.Net.Mime
{
	/// <summary>
	/// Rfc 2822 3.4 mailbox-list. Syntax: mailbox *(',' mailbox).
	/// </summary>
    [Obsolete("See LumiSoft.Net.MIME or LumiSoft.Net.Mail namepaces for replacement.")]
	public class MailboxAddressCollection : IEnumerable
	{
        private readonly List<MailboxAddress> m_pMailboxes;

		/// <summary>
		/// Default constructor.
		/// </summary>
		public MailboxAddressCollection()
		{	
			m_pMailboxes = new List<MailboxAddress>();
		}

        /// <summary>
		/// Adds a new mailbox to the end of the collection.
		/// </summary>
		/// <param name="mailbox">Mailbox to add.</param>
		public void Add(MailboxAddress mailbox)
		{
			m_pMailboxes.Add(mailbox);

			OnCollectionChanged();
		}

        /// <summary>
		/// Inserts a new mailbox into the collection at the specified location.
		/// </summary>
		/// <param name="index">The location in the collection where you want to add the mailbox.</param>
		/// <param name="mailbox">Mailbox to add.</param>
		public void Insert(int index,MailboxAddress mailbox)
		{
			m_pMailboxes.Insert(index,mailbox);

			OnCollectionChanged();
		}

        /// <summary>
		/// Removes header field at the specified index from the collection.
		/// </summary>
		/// <param name="index">Index of the mailbox which to remove.</param>
		public void Remove(int index)
		{
			m_pMailboxes.RemoveAt(index);

			OnCollectionChanged();
		}

		/// <summary>
		/// Removes specified mailbox from the collection.
		/// </summary>
		/// <param name="mailbox">Mailbox to remove.</param>
		public void Remove(MailboxAddress mailbox)
		{
			m_pMailboxes.Remove(mailbox);

			OnCollectionChanged();
		}

        /// <summary>
		/// Clears the collection of all mailboxes.
		/// </summary>
		public void Clear()
		{
			m_pMailboxes.Clear();

			OnCollectionChanged();
		}

        /// <summary>
		/// Parses mailboxes from Rfc 2822 3.4 mailbox-list string. Syntax: mailbox *(',' mailbox).
		/// </summary>
		/// <param name="mailboxList">Mailbox list string.</param>
		public void Parse(string mailboxList)
		{
			// We need to parse right !!! 
			// Can't use standard String.Split() because commas in quoted strings must be skiped.
			// Example: "ivar, test" <ivar@lumisoft.ee>,"xxx" <ivar2@lumisoft.ee>

			string[] mailboxes = TextUtils.SplitQuotedString(mailboxList,',');
			foreach(string mailbox in mailboxes){
				m_pMailboxes.Add(MailboxAddress.Parse(mailbox));
			}
		}

        /// <summary>
		/// Convert addresses to Rfc 2822 mailbox-list string.
		/// </summary>
		/// <returns></returns>
		public string ToMailboxListString()
		{
			string retVal = "";
			for(int i=0;i<m_pMailboxes.Count;i++){
				// For last address don't add , and <TAB>
				if(i == (m_pMailboxes.Count - 1)){
					retVal += ((MailboxAddress)m_pMailboxes[i]).ToMailboxAddressString();
				}
				else{
					retVal += ((MailboxAddress)m_pMailboxes[i]).ToMailboxAddressString() + ",\t";
				}
			}
            
			return retVal;
		}

        /// <summary>
		/// This called when collection has changed. Item is added,deleted,changed or collection cleared.
		/// </summary>
		internal void OnCollectionChanged()
		{
			if(Owner != null){
				if(Owner is GroupAddress){
					((GroupAddress)Owner).OnChanged();
				}				
			}
		}

        /// <summary>
		/// Gets enumerator.
		/// </summary>
		/// <returns></returns>
		public IEnumerator GetEnumerator()
		{
			return m_pMailboxes.GetEnumerator();
		}

        /// <summary>
		/// Gets mailbox from specified index.
		/// </summary>
		public MailboxAddress this[int index]
		{
			get{ return (MailboxAddress)m_pMailboxes[index]; }
		}

		/// <summary>
		/// Gets mailboxes count in the collection.
		/// </summary>
		public int Count
		{
			get{ return m_pMailboxes.Count; }
		}

		/// <summary>
		/// Gets or sets owner of this collection.
		/// </summary>
		internal Address Owner { get; set; }
    }
}
