using System;

namespace LumiSoft.Net.Mime
{
	/// <summary>
	/// Header field parameter.
	/// </summary>
    [Obsolete("See LumiSoft.Net.MIME or LumiSoft.Net.Mail namepaces for replacement.")]
	public class HeaderFieldParameter
	{
        /// <summary>
		/// Default constructor.
		/// </summary>
		/// <param name="parameterName">Header field parameter name.</param>
		/// <param name="parameterValue">Header field parameter value.</param>
		public HeaderFieldParameter(string parameterName,string parameterValue)
		{
			Name = parameterName;
			Value = parameterValue;
		}


		#region Properties Implementation

		/// <summary>
		/// Gets header field parameter name.
		/// </summary>
		public string Name { get; } = "";

        /// <summary>
		/// Gets header field parameter name.
		/// </summary>
		public string Value { get; } = "";

#endregion

	}
}
