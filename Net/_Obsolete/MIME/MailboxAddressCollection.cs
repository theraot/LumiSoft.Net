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
        private readonly List<MailboxAddress> _mailboxes;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public MailboxAddressCollection()
        {
            _mailboxes = new List<MailboxAddress>();
        }

        /// <summary>
        /// Gets mailboxes count in the collection.
        /// </summary>
        public int Count => _mailboxes.Count;

        /// <summary>
        /// Gets or sets owner of this collection.
        /// </summary>
        internal Address Owner { get; set; }

        /// <summary>
        /// Gets mailbox from specified index.
        /// </summary>
        public MailboxAddress this[int index] => (MailboxAddress)_mailboxes[index];

        /// <summary>
        /// Adds a new mailbox to the end of the collection.
        /// </summary>
        /// <param name="mailbox">Mailbox to add.</param>
        public void Add(MailboxAddress mailbox)
        {
            _mailboxes.Add(mailbox);

            OnCollectionChanged();
        }

        /// <summary>
        /// Clears the collection of all mailboxes.
        /// </summary>
        public void Clear()
        {
            _mailboxes.Clear();

            OnCollectionChanged();
        }

        /// <summary>
        /// Gets enumerator.
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetEnumerator()
        {
            return _mailboxes.GetEnumerator();
        }

        /// <summary>
        /// Inserts a new mailbox into the collection at the specified location.
        /// </summary>
        /// <param name="index">The location in the collection where you want to add the mailbox.</param>
        /// <param name="mailbox">Mailbox to add.</param>
        public void Insert(int index, MailboxAddress mailbox)
        {
            _mailboxes.Insert(index, mailbox);

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

            var mailboxes = TextUtils.SplitQuotedString(mailboxList, ',');
            foreach (string mailbox in mailboxes)
            {
                _mailboxes.Add(MailboxAddress.Parse(mailbox));
            }
        }

        /// <summary>
        /// Removes header field at the specified index from the collection.
        /// </summary>
        /// <param name="index">Index of the mailbox which to remove.</param>
        public void Remove(int index)
        {
            _mailboxes.RemoveAt(index);

            OnCollectionChanged();
        }

        /// <summary>
        /// Removes specified mailbox from the collection.
        /// </summary>
        /// <param name="mailbox">Mailbox to remove.</param>
        public void Remove(MailboxAddress mailbox)
        {
            _mailboxes.Remove(mailbox);

            OnCollectionChanged();
        }

        /// <summary>
        /// Convert addresses to Rfc 2822 mailbox-list string.
        /// </summary>
        /// <returns></returns>
        public string ToMailboxListString()
        {
            var retVal = "";
            for (int i = 0; i < _mailboxes.Count; i++)
            {
                // For last address don't add , and <TAB>
                if (i == (_mailboxes.Count - 1))
                {
                    retVal += ((MailboxAddress)_mailboxes[i]).ToMailboxAddressString();
                }
                else
                {
                    retVal += ((MailboxAddress)_mailboxes[i]).ToMailboxAddressString() + ",\t";
                }
            }

            return retVal;
        }

        /// <summary>
        /// This called when collection has changed. Item is added,deleted,changed or collection cleared.
        /// </summary>
        internal void OnCollectionChanged()
        {
            (Owner as GroupAddress)?.OnChanged();
        }
    }
}
