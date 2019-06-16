using System;

namespace LumiSoft.Net.Mime
{
	/// <summary>
	/// Rfc 2822 3.4 Address class. This class is base class for MailboxAddress and GroupAddress.
	/// </summary>
    [Obsolete("See LumiSoft.Net.MIME or LumiSoft.Net.Mail namepaces for replacement.")]
	public abstract class Address
	{
        /// <summary>
		/// Default constructor.
		/// </summary>
		/// <param name="groupAddress">Spcified is address is group or mailbox address.</param>
		public Address(bool groupAddress)
		{
			IsGroupAddress = groupAddress;
		}


        /// <summary>
		/// Gets if address is group address or mailbox address.
		/// </summary>
		public bool IsGroupAddress { get; }


        /// <summary>
		/// Gets or sets owner of this address.
		/// </summary>
		internal object Owner { get; set; }
    }
}
