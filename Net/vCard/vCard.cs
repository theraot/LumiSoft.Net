using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace LumiSoft.Net.Mime.vCard
{
    /// <summary>
    /// RFC 2426 vCard implementation.
    /// </summary>
    public class vCard
    {
        private DeliveryAddressCollection m_pAddresses;
        private EmailAddressCollection m_pEmailAddresses;
        private PhoneNumberCollection m_pPhoneNumbers;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public vCard()
        {
            Items = new ItemCollection();
            Version = "3.0";
            UID = Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Gets addresses collection.
        /// </summary>
        public DeliveryAddressCollection Addresses
        {
            get
            {
                // Delay collection creation, create it when needed.
                if (m_pAddresses == null)
                {
                    m_pAddresses = new DeliveryAddressCollection(this);
                }

                return m_pAddresses;
            }
        }

        /// <summary>
        /// Gets or sets birth date. Returns DateTime.MinValue if not set.
        /// </summary>
        public DateTime BirthDate
        {
            get
            {
                var item = Items.GetFirst("BDAY");
                if (item != null)
                {
                    var date = item.DecodedValue.Replace("-", "");
                    var dateFormats = new[]{
                        "yyyyMMdd",
                        "yyyyMMddz"
                    };
                    return DateTime.ParseExact(date, dateFormats, System.Globalization.DateTimeFormatInfo.InvariantInfo, System.Globalization.DateTimeStyles.None);
                }

                return DateTime.MinValue;
            }

            set
            {
                if (value != DateTime.MinValue)
                {
                    Items.SetValue("BDAY", value.ToString("yyyyMMdd"));
                }
                else
                {
                    Items.SetValue("BDAY", null);
                }
            }
        }

        /// <summary>
        /// Gets email addresses collection.
        /// </summary>
        public EmailAddressCollection EmailAddresses
        {
            get
            {
                // Delay collection creation, create it when needed.
                if (m_pEmailAddresses == null)
                {
                    m_pEmailAddresses = new EmailAddressCollection(this);
                }

                return m_pEmailAddresses;
            }
        }

        /// <summary>
        /// Gets or sets formatted(Display name) name.  Returns null if FN: item doesn't exist.
        /// </summary>
        public string FormattedName
        {
            get
            {
                var item = Items.GetFirst("FN");
                return item?.DecodedValue;
            }

            set { Items.SetDecodedValue("FN", value); }
        }

        /// <summary>
        /// Gets or sets vCard home URL.
        /// </summary>
        public string HomeURL
        {
            get
            {
                var items = Items.Get("URL");
                foreach (Item item in items)
                {
                    if (item.ParametersString == "" || item.ParametersString.ToUpper().IndexOf("HOME") > -1)
                    {
                        return item.DecodedValue;
                    }
                }

                return null;
            }

            set
            {
                var items = Items.Get("URL");
                foreach (Item item in items)
                {
                    if (item.ParametersString.ToUpper().IndexOf("HOME") > -1)
                    {
                        if (value != null)
                        {
                            item.Value = value;
                        }
                        else
                        {
                            Items.Remove(item);
                        }
                        return;
                    }
                }

                if (value != null)
                {
                    // If we reach here, URL;Work  doesn't exist, add it.
                    Items.Add("URL", "HOME", value);
                }
            }
        }

        /// <summary>
        /// Gets reference to vCard items.
        /// </summary>
        public ItemCollection Items { get; }

        /// <summary>
        /// Gets or sets name info.  Returns null if N: item doesn't exist.
        /// </summary>
        public Name Name
        {
            get
            {
                var item = Items.GetFirst("N");
                if (item != null)
                {
                    return Name.Parse(item);
                }

                return null;
            }

            set
            {
                if (value != null)
                {
                    Items.SetDecodedValue("N", value.ToValueString());
                }
                else
                {
                    Items.SetDecodedValue("N", null);
                }
            }
        }

        /// <summary>
        /// Gets or sets nick name. Returns null if NICKNAME: item doesn't exist.
        /// </summary>
        public string NickName
        {
            get
            {
                var item = Items.GetFirst("NICKNAME");
                return item?.DecodedValue;
            }

            set { Items.SetDecodedValue("NICKNAME", value); }
        }

        /// <summary>
        /// Gets or sets note text. Returns null if NOTE: item doesn't exist.
        /// </summary>
        public string NoteText
        {
            get
            {
                var item = Items.GetFirst("NOTE");
                return item?.DecodedValue;
            }

            set { Items.SetDecodedValue("NOTE", value); }
        }

        /// <summary>
        /// Gets or sets organization name. Usually this value is: comapny;department;office. Returns null if ORG: item doesn't exist.
        /// </summary>
        public string Organization
        {
            get
            {
                var item = Items.GetFirst("ORG");
                return item?.DecodedValue;
            }

            set { Items.SetDecodedValue("ORG", value); }
        }

        /// <summary>
        /// Gets phone number collection.
        /// </summary>
        public PhoneNumberCollection PhoneNumbers
        {
            get
            {
                // Delay collection creation, create it when needed.
                if (m_pPhoneNumbers == null)
                {
                    m_pPhoneNumbers = new PhoneNumberCollection(this);
                }

                return m_pPhoneNumbers;
            }
        }

        /// <summary>
        /// Gets or sets person photo. Returns null if PHOTO: item doesn't exist.
        /// </summary>
        public Image Photo
        {
            get
            {
                var item = Items.GetFirst("PHOTO");
                if (item != null)
                {
                    return Image.FromStream(new MemoryStream(Encoding.Default.GetBytes(item.DecodedValue)));
                }

                return null;
            }

            set
            {
                if (value != null)
                {
                    var ms = new MemoryStream();
                    value.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);

                    Items.SetValue("PHOTO", "ENCODING=b;TYPE=JPEG", Convert.ToBase64String(ms.ToArray()));
                }
                else
                {
                    Items.SetValue("PHOTO", null);
                }
            }
        }

        /// <summary>
        /// Gets or sets role. Returns null if ROLE: item doesn't exist.
        /// </summary>
        public string Role
        {
            get
            {
                var item = Items.GetFirst("ROLE");
                return item?.DecodedValue;
            }

            set { Items.SetDecodedValue("ROLE", value); }
        }

        /// <summary>
        /// Gets or sets job title. Returns null if TITLE: item doesn't exist.
        /// </summary>
        public string Title
        {
            get
            {
                var item = Items.GetFirst("TITLE");
                return item?.DecodedValue;
            }

            set { Items.SetDecodedValue("TITLE", value); }
        }

        /// <summary>
        /// Gets or sets vCard unique ID. Returns null if UID: item doesn't exist.
        /// </summary>
        public string UID
        {
            get
            {
                var item = Items.GetFirst("UID");
                return item?.DecodedValue;
            }

            set { Items.SetDecodedValue("UID", value); }
        }

        /// <summary>
        /// Gets or sets vCard version. Returns null if VERSION: item doesn't exist.
        /// </summary>
        public string Version
        {
            get
            {
                var item = Items.GetFirst("VERSION");
                return item?.DecodedValue;
            }

            set { Items.SetDecodedValue("VERSION", value); }
        }

        /// <summary>
        /// Gets or sets vCard Work URL.
        /// </summary>
        public string WorkURL
        {
            get
            {
                var items = Items.Get("URL");
                foreach (Item item in items)
                {
                    if (item.ParametersString.ToUpper().IndexOf("WORK") > -1)
                    {
                        return item.DecodedValue;
                    }
                }

                return null;
            }

            set
            {
                var items = Items.Get("URL");
                foreach (Item item in items)
                {
                    if (item.ParametersString.ToUpper().IndexOf("WORK") > -1)
                    {
                        if (value != null)
                        {
                            item.Value = value;
                        }
                        else
                        {
                            Items.Remove(item);
                        }
                        return;
                    }
                }

                if (value != null)
                {
                    // If we reach here, URL;Work  doesn't exist, add it.
                    Items.Add("URL", "WORK", value);
                }
            }
        }

        /// <summary>
        /// Parses multiple vCards from the specified file (Apple Address Book and Gmail exports)
        /// </summary>
        /// <param name="file">vCard file with path.</param>
        public static List<vCard> ParseMultiple(string file)
        {
            var vCards = new List<vCard>();
            var fileStrings = new List<string>();
            var line = "";
            bool hasBeginTag = false;
            using (FileStream fs = File.OpenRead(file))
            {
                TextReader r = new StreamReader(fs, Encoding.Default);
                while (line != null)
                {
                    line = r.ReadLine();
                    if (line != null && line.ToUpper() == "BEGIN:VCARD")
                    {
                        hasBeginTag = true;
                    }
                    if (hasBeginTag)
                    {
                        fileStrings.Add(line);
                        if (line != null && line.ToUpper() == "END:VCARD")
                        {
                            // on END line process the Vcard, reinitialize, and will repeat the same thing for any others.
                            var singleVcard = new vCard();
                            singleVcard.ParseStrings(fileStrings);
                            vCards.Add(singleVcard);
                            fileStrings.Clear();
                            hasBeginTag = false;
                        }
                    }
                }
            }
            return vCards;
        }

        /// <summary>
        /// Parses single vCard from the specified file.
        /// </summary>
        /// <param name="file">vCard file with path.</param>
        public void Parse(string file)
        {
            var fileStrings = new List<string>();
            var fileStringArray = File.ReadAllLines(file, Encoding.Default);
            foreach (string fileString in fileStringArray)
            {
                fileStrings.Add(fileString);
            }
            ParseStrings(fileStrings);
        }

        /// <summary>
        /// Parses single vCard from the specified stream.
        /// </summary>
        /// <param name="stream">Stream what contains vCard.</param>
        public void Parse(FileStream stream)
        {
            var fileStrings = new List<string>();
            var line = "";
            TextReader r = new StreamReader(stream, Encoding.Default);
            while (line != null)
            {
                line = r.ReadLine();
                fileStrings.Add(line);
            }
            ParseStrings(fileStrings);
        }

        /// <summary>
        /// Parses vCard from the specified stream.
        /// </summary>
        /// <param name="fileStrings">List of strings that contains vCard.</param>
        public void ParseStrings(List<string> fileStrings)
        {
            Items.Clear();
            m_pPhoneNumbers = null;
            m_pEmailAddresses = null;

            int lineCount = 0;
            var line = fileStrings[lineCount];
            // Find row BEGIN:VCARD
            while (line != null && line.ToUpper() != "BEGIN:VCARD")
            {
                line = fileStrings[lineCount++];
            }
            // Read first vCard line after BEGIN:VCARD
            line = fileStrings[lineCount++];
            while (line != null && line.ToUpper() != "END:VCARD")
            {
                var item = new StringBuilder();
                item.Append(line);
                // Get next line, see if item continues (folded line).
                line = fileStrings[lineCount++];
                while (line != null && (line.StartsWith("\t") || line.StartsWith(" ")))
                {
                    item.Append(line.Substring(1));
                    line = fileStrings[lineCount++];
                }

                var name_value = item.ToString().Split(new[] { ':' }, 2);

                // Item syntax: name[*(;parameter)]:value
                var name_params = name_value[0].Split(new[] { ';' }, 2);
                var name = name_params[0];
                var parameters = "";
                if (name_params.Length == 2)
                {
                    parameters = name_params[1];
                }
                var value = "";
                if (name_value.Length == 2)
                {
                    value = name_value[1];
                }
                Items.Add(name, parameters, value);
            }
        }

        /// <summary>
        /// Stores vCard structure to byte[].
        /// </summary>
        /// <returns></returns>
        public byte[] ToByte()
        {
            var ms = new MemoryStream();
            ToStream(ms);
            return ms.ToArray();
        }

        /// <summary>
        /// Stores vCard to the specified file.
        /// </summary>
        /// <param name="file">File name with path where to store vCard.</param>
        public void ToFile(string file)
        {
            using (FileStream fs = File.Create(file))
            {
                ToStream(fs);
            }
        }

        /// <summary>
        /// Stores vCard structure to the specified stream.
        /// </summary>
        /// <param name="stream">Stream where to store vCard structure.</param>
        public void ToStream(Stream stream)
        {
            /* 
                BEGIN:VCARD<CRLF>
                ....
                END:VCARD<CRLF>
            */

            var retVal = new StringBuilder();
            retVal.Append("BEGIN:VCARD\r\n");
            foreach (Item item in Items)
            {
                retVal.Append(item.ToItemString() + "\r\n");
            }
            retVal.Append("END:VCARD\r\n");

            var data = Encoding.UTF8.GetBytes(retVal.ToString());
            stream.Write(data, 0, data.Length);
        }
    }
}
