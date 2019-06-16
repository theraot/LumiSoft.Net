using System;
using System.IO;
using System.Xml;

namespace LumiSoft.Net.UPnP
{
    /// <summary>
    /// This class represents UPnP device.
    /// </summary>
    public class UPnP_Device
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="url">Device URL.</param>
        internal UPnP_Device(string url)
        {
            if(url == null){
                throw new ArgumentNullException("url");
            }

            Init(url);
        }

        private void Init(string url)
        {
            var xml = new XmlDocument();
            xml.Load(url);

            var xmlString = new StringWriter();
            xml.WriteTo(new XmlTextWriter(xmlString));
            DeviceXml = xmlString.ToString();

            // Set up namespace manager for XPath   
            var ns = new XmlNamespaceManager(xml.NameTable);
            ns.AddNamespace("n",xml.ChildNodes[1].NamespaceURI);

            BaseUrl          = xml.SelectSingleNode("n:root/n:URLBase",ns).InnerText;
            DeviceType       = xml.SelectSingleNode("n:root/n:device/n:deviceType",ns).InnerText;
            FriendlyName     = xml.SelectSingleNode("n:root/n:device/n:friendlyName",ns).InnerText;
            Manufacturer     = xml.SelectSingleNode("n:root/n:device/n:manufacturer",ns).InnerText;
            ManufacturerUrl  = xml.SelectSingleNode("n:root/n:device/n:manufacturerURL",ns).InnerText;
            ModelDescription = xml.SelectSingleNode("n:root/n:device/n:modelDescription",ns).InnerText;
            ModelName        = xml.SelectSingleNode("n:root/n:device/n:modelName",ns).InnerText;
            ModelNumber      = xml.SelectSingleNode("n:root/n:device/n:modelNumber",ns).InnerText;
            ModelUrl         = xml.SelectSingleNode("n:root/n:device/n:modelURL",ns).InnerText;
            SerialNumber     = xml.SelectSingleNode("n:root/n:device/n:serialNumber",ns).InnerText;
            UDN              = xml.SelectSingleNode("n:root/n:device/n:UDN",ns).InnerText;
            PresentationUrl  = xml.SelectSingleNode("n:root/n:device/n:presentationURL",ns).InnerText;
        }

        /// <summary>
        /// Gets device base URL.
        /// </summary>
        public string BaseUrl { get; private set; } = "";

        /// <summary>
        /// Gets device type.
        /// </summary>
        public string DeviceType { get; private set; } = "";

        /// <summary>
        /// Gets device short name.
        /// </summary>
        public string FriendlyName { get; private set; } = "";

        /// <summary>
        /// Gets manufacturer's name.
        /// </summary>
        public string Manufacturer { get; private set; } = "";

        /// <summary>
        /// Gets web site for Manufacturer.
        /// </summary>
        public string ManufacturerUrl { get; private set; } = "";

        /// <summary>
        /// Gets device long description.
        /// </summary>
        public string ModelDescription { get; private set; } = "";

        /// <summary>
        /// Gets model name.
        /// </summary>
        public string ModelName { get; private set; } = "";

        /// <summary>
        /// Gets model number.
        /// </summary>
        public string ModelNumber { get; private set; } = "";

        /// <summary>
        /// Gets web site for model.
        /// </summary>
        public string ModelUrl { get; private set; } = "";

        /// <summary>
        /// Gets serial number.
        /// </summary>
        public string SerialNumber { get; private set; } = "";

        /// <summary>
        /// Gets unique device name.
        /// </summary>
        public string UDN { get; private set; } = "";

        // iconList
        // serviceList
        // deviceList

        /// <summary>
        /// Gets device UI url.
        /// </summary>
        public string PresentationUrl { get; private set; } = "";

        /// <summary>
        /// Gets UPnP device XML description.
        /// </summary>
        public string DeviceXml { get; private set; }
    }
}
