﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Xml;
using LumiSoft.Net.UPnP.Client;

namespace LumiSoft.Net.UPnP.NAT
{
    /// <summary>
    ///     This class provides methods for managing UPnP NAT router.
    /// </summary>
    public class UPnP_NAT_Client
    {
        private readonly string _baseUrl;
        private readonly string _controlUrl;
        private readonly string _serviceType;

        /// <summary>
        ///     Default constructor.
        /// </summary>
        public UPnP_NAT_Client()
        {
            foreach (var adapter in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (adapter.OperationalStatus != OperationalStatus.Up)
                {
                    continue;
                }

                foreach (var gatewayIPAddressInformation in adapter.GetIPProperties().GatewayAddresses)
                {
                    ProcessDevices
                        (
                            UPnP_Client.Search(gatewayIPAddressInformation.Address, "urn:schemas-upnp-org:device:InternetGatewayDevice:1", 1200)
                            , out var baseUrl
                            , out var controlUrl
                            , out var serviceType
                    );
                    _baseUrl = baseUrl;
                    _controlUrl = controlUrl;
                    _serviceType = serviceType;
                    return;
                }
            }

            void ProcessDevices(UPnP_Device[] devices, out string baseUrl, out string controlUrl, out string serviceType)
            {
                baseUrl = null;
                controlUrl = null;
                serviceType = null;
                if (devices == null)
                {
                    return;
                }
                // Gateway no UPnP device, search for UPnP router.
                if (devices.Length == 0)
                {
                    devices = UPnP_Client.Search("urn:schemas-upnp-org:device:InternetGatewayDevice:1", 1200);
                }

                if (devices.Length <= 0)
                {
                    return;
                }

                var xml = new XmlDocument();
                xml.LoadXml(devices[0].DeviceXml);

                // Loop XML tree by nodes.
                var queue = new List<XmlNode>
                {
                    xml
                };
                while (queue.Count > 0)
                {
                    var currentNode = queue[0];
                    queue.RemoveAt(0);

                    if (string.Equals("urn:schemas-upnp-org:service:WANPPPConnection:1", currentNode.InnerText, StringComparison.InvariantCultureIgnoreCase))
                    {
                        if (currentNode.ParentNode == null)
                        {
                            continue;
                        }

                        foreach (XmlNode node in currentNode.ParentNode.ChildNodes)
                        {
                            if (!string.Equals("controlURL", node.Name, StringComparison.InvariantCultureIgnoreCase))
                            {
                                continue;
                            }

                            baseUrl = devices[0].BaseUrl;
                            serviceType = "urn:schemas-upnp-org:service:WANPPPConnection:1";
                            controlUrl = node.InnerText;

                            return;
                        }
                    }
                    else if (string.Equals("urn:schemas-upnp-org:service:WANIPConnection:1", currentNode.InnerText, StringComparison.InvariantCultureIgnoreCase))
                    {
                        if (currentNode.ParentNode == null)
                        {
                            continue;
                        }

                        foreach (XmlNode node in currentNode.ParentNode.ChildNodes)
                        {
                            if (!string.Equals("controlURL", node.Name, StringComparison.InvariantCultureIgnoreCase))
                            {
                                continue;
                            }

                            baseUrl = devices[0].BaseUrl;
                            serviceType = "urn:schemas-upnp-org:service:WANIPConnection:1";
                            controlUrl = node.InnerText;

                            return;
                        }
                    }
                    else if (currentNode.ChildNodes.Count > 0)
                    {
                        for (var i = 0; i < currentNode.ChildNodes.Count; i++)
                        {
                            queue.Insert(i, currentNode.ChildNodes[i]);
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Gets if UPnP NAT is supported.
        /// </summary>
        public bool IsSupported => _controlUrl != null;

        /// <summary>
        ///     This method creates a new port mapping or overwrites an existing mapping.
        /// </summary>
        /// <param name="enabled">Specifies if port mapping is enabled.</param>
        /// <param name="description">Port mapping description.</param>
        /// <param name="protocol">Port mapping protocol. Normally this value TCP or UDP.</param>
        /// <param name="remoteHost">Remote host IP address.</param>
        /// <param name="publicPort">Desired public port.</param>
        /// <param name="localEp">Local IP end point.</param>
        /// <param name="leaseDuration">Lease duration in seconds. Value null means never expires.</param>
        /// <exception cref="ArgumentNullException">
        ///     Is raised when <b>description</b>,<b>protocol</b> or <b>localEP</b> is null
        ///     reference.
        /// </exception>
        /// <exception cref="UPnP_Exception">Is raised when UPnP device returns error.</exception>
        public void AddPortMapping(bool enabled, string description, string protocol, string remoteHost, int publicPort, IPEndPoint localEp, int leaseDuration)
        {
            if (description == null)
            {
                throw new ArgumentNullException(nameof(description));
            }

            if (protocol == null)
            {
                throw new ArgumentNullException(nameof(protocol));
            }

            if (localEp == null)
            {
                throw new ArgumentNullException(nameof(localEp));
            }

            /* http://upnp.org AddPortMapping.
                This action creates a new port mapping or overwrites an existing mapping with the same internal
                client. If the ExternalPort and PortMappingProtocol pair is already mapped to another
                internal client, an error is returned.

                NOTE: Not all NAT implementations will support:
                    • Wildcard value (i.e. 0) for ExternalPort
                    • InternalPort values that are different from ExternalPort
                    • Dynamic port mappings i.e. with non-Infinite PortMappingLeaseDuration

                Arguments for AddPortMapping:
                    NewRemoteHost
                    NewExternalPort
                    NewProtocol
                    NewInternalPort
                    NewInternalClient
                    NewEnabled
                    NewPortMappingDescription
                    NewLeaseDuration
            */

            try
            {
                var soapBody = "<s:Envelope xmlns:s=\"http://schemas.xmlsoap.org/soap/envelope/\" s:encodingStyle=\"http://schemas.xmlsoap.org/soap/encoding/\">\r\n" +
                               "<s:Body>\r\n" +
                               "<u:AddPortMapping xmlns:u=\"" + _serviceType + "\">\r\n" +
                               "<NewRemoteHost>" + remoteHost + "</NewRemoteHost>\r\n" +
                               "<NewExternalPort>" + publicPort + "</NewExternalPort>\r\n" +
                               "<NewProtocol>" + protocol + "</NewProtocol>\r\n" +
                               "<NewInternalPort>" + localEp.Port + "</NewInternalPort>\r\n" +
                               "<NewInternalClient>" + localEp.Address + "</NewInternalClient>\r\n" +
                               "<NewEnabled>" + Convert.ToInt32(enabled) + "</NewEnabled>\r\n" +
                               "<NewPortMappingDescription>" + description + "</NewPortMappingDescription>\r\n" +
                               "<NewLeaseDuration>" + leaseDuration + "</NewLeaseDuration>\r\n" +
                               "</u:AddPortMapping>\r\n" +
                               "</s:Body>\r\n" +
                               "</s:Envelope>\r\n";

                SendCommand(nameof(AddPortMapping), soapBody);
            }
            catch (WebException x)
            {
                // We have UPnP exception.
                if (x.Response.ContentType.ToLower().IndexOf("text/xml", StringComparison.Ordinal) > -1)
                {
                    throw UPnP_Exception.Parse(x.Response.GetResponseStream());
                }
            }
        }

        /// <summary>
        ///     Deletes port mapping.
        /// </summary>
        /// <param name="map">NAT mapping entry to delete.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>map</b> is null reference.</exception>
        /// <exception cref="UPnP_Exception">Is raised when UPnP device returns error.</exception>
        public void DeletePortMapping(UPnP_NAT_Map map)
        {
            if (map == null)
            {
                throw new ArgumentNullException(nameof(map));
            }

            DeletePortMapping(map.Protocol, map.RemoteHost, Convert.ToInt32(map.ExternalPort));
        }

        /// <summary>
        ///     Deletes port mapping.
        /// </summary>
        /// <param name="protocol">Port mapping protocol. Normally this value TCP or UDP.</param>
        /// <param name="remoteHost">Remote host IP address.</param>
        /// <param name="publicPort">Public port number.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>protocol</b> is null reference.</exception>
        /// <exception cref="UPnP_Exception">Is raised when UPnP device returns error.</exception>
        public void DeletePortMapping(string protocol, string remoteHost, int publicPort)
        {
            /* http://upnp.org DeletePortMapping.
                This action deletes a previously instantiated port mapping. As each entry is deleted, the array is
                compacted, and the variable PortMappingNumberOfEntries is decremented.

                Arguments for DeletePortMapping:
                    NewRemoteHost
                    NewExternalPort
                    NewProtocol
            */

            if (protocol == null)
            {
                throw new ArgumentNullException(nameof(protocol));
            }

            try
            {
                var soapBody = "<s:Envelope xmlns:s=\"http://schemas.xmlsoap.org/soap/envelope/\" s:encodingStyle=\"http://schemas.xmlsoap.org/soap/encoding/\">\r\n" +
                               "<s:Body>\r\n" +
                               "<u:DeletePortMapping xmlns:u=\"" + _serviceType + "\">\r\n" +
                               "<NewRemoteHost>" + remoteHost + "</NewRemoteHost>\r\n" +
                               "<NewExternalPort>" + publicPort + "</NewExternalPort>\r\n" +
                               "<NewProtocol>" + protocol + "</NewProtocol>\r\n" +
                               "</u:DeletePortMapping>\r\n" +
                               "</s:Body>\r\n" +
                               "</s:Envelope>\r\n";

                SendCommand(nameof(DeletePortMapping), soapBody);
            }
            catch (WebException x)
            {
                // We have UPnP exception.
                if (x.Response.ContentType.ToLower().IndexOf("text/xml", StringComparison.Ordinal) > -1)
                {
                    throw UPnP_Exception.Parse(x.Response.GetResponseStream());
                }
            }
        }

        /// <summary>
        ///     Gets NAT public IP address.
        /// </summary>
        /// <returns>Returns NAT public IP address.</returns>
        public IPAddress GetExternalIPAddress()
        {
            /* http://upnp.org GetExternalIPAddress
                This action retrieves the value of the external IP address on this connection instance.

                Returns:
                    NewExternalIPAddress
            */

            var soapBody = "<s:Envelope xmlns:s=\"http://schemas.xmlsoap.org/soap/envelope/\" s:encodingStyle=\"http://schemas.xmlsoap.org/soap/encoding/\">\r\n" +
                           "<s:Body>\r\n" +
                           "<u:GetExternalIPAddress xmlns:u=\"" + _serviceType + "\"></u:GetExternalIPAddress>\r\n" +
                           "</s:Body>\r\n" +
                           "</s:Envelope>\r\n";

            var soapResponse = SendCommand(nameof(GetExternalIPAddress), soapBody);

            using (var stringReader = new StringReader(soapResponse))
            {
                using (var reader = XmlReader.Create(stringReader))
                {
                    while (reader.Read())
                    {
                        if (string.Equals("NewExternalIPAddress", reader.Name, StringComparison.InvariantCultureIgnoreCase))
                        {
                            return IPAddress.Parse(reader.ReadString());
                        }
                    }
                }

                return null;
            }
        }

        /// <summary>
        ///     Gets all existing port mappings.
        /// </summary>
        /// <returns>Returns all existing port mappings.</returns>
        public UPnP_NAT_Map[] GetPortMappings()
        {
            /* http://upnp.org GetGenericPortMappingEntry.
                This action retrieves NAT port mappings one entry at a time. Control points can call this action
                with an incrementing array index until no more entries are found on the gateway. If
                PortMappingNumberOfEntries is updated during a call, the process may have to start over.
                Entries in the array are contiguous. As entries are deleted, the array is compacted, and the
                variable PortMappingNumberOfEntries is decremented. Port mappings are logically
                stored as an array on the IGD and retrieved using an array index ranging from 0 to
                PortMappingNumberOfEntries-1.

                Arguments for GetGenericPortMappingEntry:
                    NewPortMappingIndex

                Returns:
                    NewRemoteHost
                    NewExternalPort
                    NewProtocol
                    NewInternalPort
                    NewInternalClient
                    NewEnabled
                    NewPortMappingDescription
                    NewLeaseDuration
            */

            var result = new List<UPnP_NAT_Map>();
            for (var index = 0; index < 100; index++)
            {
                try
                {
                    var soapBody = "<s:Envelope xmlns:s=\"http://schemas.xmlsoap.org/soap/envelope/\" s:encodingStyle=\"http://schemas.xmlsoap.org/soap/encoding/\">\r\n" +
                                   "<s:Body>\r\n" +
                                   "<u:GetGenericPortMappingEntry xmlns:u=\"" + _serviceType + "\">\r\n" +
                                   "<NewPortMappingIndex>" + index + "</NewPortMappingIndex>\r\n" +
                                   "</u:GetGenericPortMappingEntry>\r\n" +
                                   "</s:Body>\r\n" +
                                   "</s:Envelope>\r\n";

                    var soapResponse = SendCommand("GetGenericPortMappingEntry", soapBody);

                    var enabled = false;
                    var protocol = string.Empty;
                    var remoteHost = string.Empty;
                    var externalPort = string.Empty;
                    var internalHost = string.Empty;
                    var internalPort = 0;
                    var description = string.Empty;
                    var leaseDuration = 0;

                    using (var stringReader = new StringReader(soapResponse))
                    {
                        using (var reader = XmlReader.Create(stringReader))
                        {
                            while (reader.Read())
                            {
                                if (string.Equals("NewRemoteHost", reader.Name, StringComparison.InvariantCultureIgnoreCase))
                                {
                                    remoteHost = reader.ReadString();
                                }
                                else if (string.Equals("NewExternalPort", reader.Name, StringComparison.InvariantCultureIgnoreCase))
                                {
                                    externalPort = reader.ReadString();
                                }
                                else if (string.Equals("NewProtocol", reader.Name, StringComparison.InvariantCultureIgnoreCase))
                                {
                                    protocol = reader.ReadString();
                                }
                                else if (string.Equals("NewInternalPort", reader.Name, StringComparison.InvariantCultureIgnoreCase))
                                {
                                    internalPort = Convert.ToInt32(reader.ReadString());
                                }
                                else if (string.Equals("NewInternalClient", reader.Name, StringComparison.InvariantCultureIgnoreCase))
                                {
                                    internalHost = reader.ReadString();
                                }
                                else if (string.Equals("NewEnabled", reader.Name, StringComparison.InvariantCultureIgnoreCase))
                                {
                                    enabled = Convert.ToBoolean(Convert.ToInt32(reader.ReadString()));
                                }
                                else if (string.Equals("NewPortMappingDescription", reader.Name, StringComparison.InvariantCultureIgnoreCase))
                                {
                                    description = reader.ReadString();
                                }
                                else if (string.Equals("NewLeaseDuration", reader.Name, StringComparison.InvariantCultureIgnoreCase))
                                {
                                    leaseDuration = Convert.ToInt32(reader.ReadString());
                                }
                            }
                        }

                        result.Add(new UPnP_NAT_Map(enabled, protocol, remoteHost, externalPort, internalHost, internalPort, description, leaseDuration));
                    }
                }
                catch (WebException x)
                {
                    // We should see what error we got. We expect "Array out of index", other exceptions we must pass through.

                    // We have UPnP exception.
                    if (x.Response.ContentType.ToLower().IndexOf("text/xml", StringComparison.Ordinal) > -1)
                    {
                        using (var stream = x.Response.GetResponseStream())
                        {
                            var uX = UPnP_Exception.Parse(stream);
                            // Other error than "Index out of range", we pass it through.
                            if (uX.ErrorCode != 713)
                            {
                                throw uX;
                            }
                        }
                    }
                    // Unknown http error.
                    else
                    {
                        throw;
                    }

                    break;
                }
            }

            return result.ToArray();
        }

        /// <summary>
        ///     Sends command to UPnP device and reads response.
        /// </summary>
        /// <param name="method">Command method.</param>
        /// <param name="soapData">Soap xml.</param>
        /// <returns>Returns UPnP device response.</returns>
        private string SendCommand(string method, string soapData)
        {
            var requestBody = Encoding.UTF8.GetBytes(soapData);

            var request = WebRequest.Create(_baseUrl + _controlUrl);
            request.Method = "POST";
            request.Headers.Add("SOAPAction", _serviceType + "#" + method);
            request.ContentType = "text/xml; charset=\"utf-8\";";
            request.ContentLength = requestBody.Length;

            // Send SOAP body to server.
            request.GetRequestStream().Write(requestBody, 0, requestBody.Length);
            request.GetRequestStream().Close();

            var response = request.GetResponse();
            using (TextReader r = new StreamReader(response.GetResponseStream() ?? throw new InvalidOperationException()))
            {
                return r.ReadToEnd();
            }
        }
    }
}