#pragma warning disable RCS1194 // Implement exception constructors.

using System;
using System.IO;
using System.Collections.Generic;
using System.Xml;

namespace LumiSoft.Net.UPnP
{
   /// <summary>
   /// This class represents UPnP error.
   /// </summary>
    public class UPnPException : Exception
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="errorCode">UPnP error code.</param>
        /// <param name="errorText">UPnP error text.</param>
        public UPnPException(int errorCode, string errorText)
            : base("UPnP error: " + errorCode + " " + errorText + ".")
        {
            ErrorCode = errorCode;
            ErrorText = errorText;
        }

        /// <summary>
        /// Gets UPnP error code.
        /// </summary>
        public int ErrorCode { get; }

        /// <summary>
        /// Gets UPnP error text.
        /// </summary>
        public string ErrorText { get; }

        /// <summary>
        /// Parses UPnP exception from UPnP xml error.
        /// </summary>
        /// <param name="stream">Error xml stream.</param>
        /// <returns>Returns UPnP exception.</returns>
        /// <exception cref="ArgumentNullException">Is raised when <b>stream</b> is null reference.</exception>
        /// <exception cref="ParseException">Is raised when parsing fails.</exception>
        public static UPnPException Parse(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            var errorCode = -1;
            string errorText = null;
            var xml = new XmlDocument();
            xml.Load(stream);

            // Loop XML tree by nodes.
            var queue = new List<XmlNode>
            {
                xml
            };
            while (queue.Count > 0)
            {
                var currentNode = queue[0];
                queue.RemoveAt(0);

                if (string.Equals("UPnPError", currentNode.Name, StringComparison.InvariantCultureIgnoreCase))
                {
                    foreach (XmlNode node in currentNode.ChildNodes)
                    {
                        if (string.Equals(nameof(errorCode), node.Name, StringComparison.InvariantCultureIgnoreCase))
                        {
                            errorCode = Convert.ToInt32(node.InnerText);
                        }
                        else if (string.Equals("errorDescription", node.Name, StringComparison.InvariantCultureIgnoreCase))
                        {
                            errorText = node.InnerText;
                        }
                    }

                    break;
                }

                if (currentNode.ChildNodes.Count <= 0)
                {
                    continue;
                }

                for (var i = 0; i < currentNode.ChildNodes.Count; i++)
                {
                    queue.Insert(i, currentNode.ChildNodes[i]);
                }
            }

            if (errorCode == -1 || errorText == null)
            {
                throw new ParseException("Failed to parse UPnP error.");
            }

            return new UPnPException(errorCode, errorText);
        }
    }
}
