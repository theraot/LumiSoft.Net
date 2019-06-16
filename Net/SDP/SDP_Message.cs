using System;
using System.Collections.Generic;
using System.Text;

namespace LumiSoft.Net.SDP
{
    /// <summary>
    /// Session Description Protocol. Defined in RFC 4566.
    /// </summary>
    public class SDP_Message
    {
        private string m_SessionName = "";
        private string m_Version = "0";

        /// <summary>
        /// Default constructor.
        /// </summary>
        public SDP_Message()
        {
            Times = new List<SDP_Time>();
            Attributes = new List<SDP_Attribute>();
            MediaDescriptions = new List<SDP_MediaDescription>();
        }

        /// <summary>
        /// Gets attributes collection. This is optional value, Count == 0 means not specified.
        /// </summary>
        public List<SDP_Attribute> Attributes { get; }

        /// <summary>
        /// Gets or sets connection data. This is optional value if each media part specifies this value,
        /// null means not specified.
        /// </summary>
        public SDP_Connection Connection { get; set; }

        /// <summary>
        /// Gets media descriptions.
        /// </summary>
        public List<SDP_MediaDescription> MediaDescriptions { get; }

        /// <summary>
        /// Gets or sets session originator.
        /// </summary>
        public SDP_Origin Origin { get; set; }

        /// <summary>
        /// Gets or sets repeat times for a session. This is optional value, null means not specified.
        /// </summary>
        public string RepeatTimes { get; set; } = "";

        /// <summary>
        /// Gets or sets textual information about the session. This is optional value, null means not specified.
        /// </summary>
        public string SessionDescription { get; set; } = "";

        /// <summary>
        /// Gets or sets textual session name.
        /// </summary>
        public string SessionName
        {
            get { return m_SessionName; }

            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentException("Property SessionName can't be null or empty !");
                }

                m_SessionName = value;
            }
        }

        /// <summary>
        /// Gets start and stop times for a session. If Count = 0, t field not written dot SDP data.
        /// </summary>
        public List<SDP_Time> Times { get; }

        /// <summary>
        /// Gets or sets Uniform Resource Identifier. The URI should be a pointer to additional information 
        /// about the session. This is optional value, null means not specified.
        /// </summary>
        public string Uri { get; set; } = "";

        /// <summary>
        /// Gets or sets version of the Session Description Protocol.
        /// </summary>
        public string Version
        {
            get { return m_Version; }

            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentException("Property Version can't be null or empty !");
                }

                m_Version = value;
            }
        }

        /// <summary>
        /// Parses SDP from raw data.
        /// </summary>
        /// <param name="data">Raw SDP data.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>data</b> is null reference.</exception>
        public static SDP_Message Parse(string data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            var sdp = new SDP_Message();

            var r = new System.IO.StringReader(data);

            var line = r.ReadLine();

            //--- Read global fields ---------------------------------------------       
            while (line != null)
            {
                line = line.Trim();

                // We reached to media descriptions
                if (line.ToLower().StartsWith("m"))
                {
                    /*
                        m=  (media name and transport address)
                        i=* (media title)
                        c=* (connection information -- optional if included at session level)
                        b=* (zero or more bandwidth information lines)
                        k=* (encryption key)
                        a=* (zero or more media attribute lines)
                    */

                    var media = SDP_MediaDescription.Parse(line);
                    sdp.MediaDescriptions.Add(media);
                    line = r.ReadLine();
                    // Pasrse media fields and attributes
                    while (line != null)
                    {
                        line = line.Trim();

                        // Next media descrition, just stop active media description parsing, 
                        // fall through main while, allow next while loop to process it.
                        if (line.ToLower().StartsWith("m"))
                        {
                            break;
                        }
                        // i media title

                        if (line.ToLower().StartsWith("i"))
                        {
                            media.Information = line.Split(new[] { '=' }, 2)[1].Trim();
                        }
                        // c connection information
                        else if (line.ToLower().StartsWith("c"))
                        {
                            media.Connection = SDP_Connection.Parse(line);
                        }
                        // a Attributes
                        else if (line.ToLower().StartsWith("a"))
                        {
                            media.Attributes.Add(SDP_Attribute.Parse(line));
                        }

                        line = r.ReadLine();
                    }

                    if (line == null)
                    {
                        break;
                    }

                    continue;
                }
                // v Protocol Version

                if (line.ToLower().StartsWith("v"))
                {
                    sdp.Version = line.Split(new[] { '=' }, 2)[1].Trim();
                }
                // o Origin
                else if (line.ToLower().StartsWith("o"))
                {
                    sdp.Origin = SDP_Origin.Parse(line);
                }
                // s Session Name
                else if (line.ToLower().StartsWith("s"))
                {
                    sdp.SessionName = line.Split(new[] { '=' }, 2)[1].Trim();
                }
                // i Session Information
                else if (line.ToLower().StartsWith("i"))
                {
                    sdp.SessionDescription = line.Split(new[] { '=' }, 2)[1].Trim();
                }
                // u URI
                else if (line.ToLower().StartsWith("u"))
                {
                    sdp.Uri = line.Split(new[] { '=' }, 2)[1].Trim();
                }
                // c Connection Data
                else if (line.ToLower().StartsWith("c"))
                {
                    sdp.Connection = SDP_Connection.Parse(line);
                }
                // t Timing
                else if (line.ToLower().StartsWith("t"))
                {
                    sdp.Times.Add(SDP_Time.Parse(line));
                }
                // a Attributes
                else if (line.ToLower().StartsWith("a"))
                {
                    sdp.Attributes.Add(SDP_Attribute.Parse(line));
                }

                line = r.ReadLine().Trim();
            }

            return sdp;
        }

        /// <summary>
        /// Clones this SDP message.
        /// </summary>
        /// <returns>Returns cloned SDP message.</returns>
        public SDP_Message Clone()
        {
            return (SDP_Message)MemberwiseClone();
        }

        /// <summary>
        /// Returns SDP as byte[] data.
        /// </summary>
        /// <returns>Returns SDP as byte[] data.</returns>
        public byte[] ToByte()
        {
            return Encoding.UTF8.GetBytes(ToStringData());
        }

        /// <summary>
        /// Stores SDP data to specified file. Note: official suggested file extention is .sdp.
        /// </summary>
        /// <param name="fileName">File name with path where to store SDP data.</param>
        public void ToFile(string fileName)
        {
            System.IO.File.WriteAllText(fileName, ToStringData());
        }

        /// <summary>
        /// Returns SDP as string data.
        /// </summary>
        /// <returns></returns>
        public string ToStringData()
        {
            var retVal = new StringBuilder();

            // v Protocol Version
            retVal.AppendLine("v=" + Version);
            // o Origin
            if (Origin != null)
            {
                retVal.Append(Origin.ToString());
            }
            // s Session Name
            if (!string.IsNullOrEmpty(SessionName))
            {
                retVal.AppendLine("s=" + SessionName);
            }
            // i Session Information
            if (!string.IsNullOrEmpty(SessionDescription))
            {
                retVal.AppendLine("i=" + SessionDescription);
            }
            // u URI
            if (!string.IsNullOrEmpty(Uri))
            {
                retVal.AppendLine("u=" + Uri);
            }
            // c Connection Data
            if (Connection != null)
            {
                retVal.Append(Connection.ToValue());
            }
            // t Timing
            foreach (SDP_Time time in Times)
            {
                retVal.Append(time.ToValue());
            }
            // a Attributes
            foreach (SDP_Attribute attribute in Attributes)
            {
                retVal.Append(attribute.ToValue());
            }
            // m media description(s)
            foreach (SDP_MediaDescription media in MediaDescriptions)
            {
                retVal.Append(media.ToValue());
            }

            return retVal.ToString();
        }
    }
}
