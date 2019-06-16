﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Xml;

namespace LumiSoft.Net.WebDav
{
    /// <summary>
    /// This class represent WeDav 'DAV:multistatus' element. Defined RFC 4918 13.
    /// </summary>
    public class WebDav_MultiStatus
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public WebDav_MultiStatus()
        {
            Responses = new List<WebDav_Response>();
        }

        /// <summary>
        /// Gets responses collection.
        /// </summary>
        public List<WebDav_Response> Responses { get; }

        /// <summary>
        /// Parses WebDav_MultiResponse from 'DAV:multistatus' element.
        /// </summary>
        /// <param name="stream">DAV:multistatus response stream.</param>
        /// <returns>Returns DAV multistatus.</returns>
        /// <exception cref="ArgumentNullException">Is raised when <b>stream</b> is null reference.</exception>
        /// <exception cref="ParseException">Is raised when there are any parsing error.</exception>
        internal static WebDav_MultiStatus Parse(Stream stream)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            var response = new XmlDocument();
            response.Load(stream);

            // Invalid response.
            if (!string.Equals(response.ChildNodes[1].NamespaceURI + response.ChildNodes[1].LocalName, "DAV:multistatus", StringComparison.InvariantCultureIgnoreCase))
            {
                throw new ParseException("Invalid DAV:multistatus value.");
            }

            var retVal = new WebDav_MultiStatus();

            // Parse responses.
            foreach (XmlNode responseNode in response.ChildNodes[1].ChildNodes)
            {
                retVal.Responses.Add(WebDav_Response.Parse(responseNode));
            }

            return retVal;
        }
    }
}
