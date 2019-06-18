﻿using System;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Collections.Generic;
using System.Text;
using System.Xml;

using LumiSoft.Net.UPnP.Client;

namespace LumiSoft.Net.UPnP.NAT
{
    /// <summary>
    /// This class provides methods for managing UPnP NAT router.
    /// </summary>
    public class UPnPnatClient
    {
        private static string mBaseUrl;
        private static string mControlUrl;
        private static string mServiceType;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public UPnPnatClient()
        {
            Init();
        }

        /// <summary>
        /// Gets if UPnP NAT is supported.
        /// </summary>
        public bool IsSupported => mControlUrl != null;

        /// <summary>
        /// This method creates a new port mapping or overwrites an existing mapping.
        /// </summary>
        /// <param name="enabled">Specifies if port mapping is enabled.</param>
        /// <param name="description">Port mapping description.</param>
        /// <param name="protocol">Port mapping protocol. Normally this value TCP or UDP.</param>
        /// <param name="remoteHost">Remote host IP address.</param>
        /// <param name="publicPort">Desired public port.</param>
        /// <param name="localEp">Local IP end point.</param>
        /// <param name="leaseDuration">Lease duration in seconds. Value null means never expires.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>description</b>,<b>protocol</b> or <b>localEP</b> is null reference.</exception>
        /// <exception cref="UPnPException">Is raised when UPnP device returns error.</exception>
        public void AddPortMapping(bool enabled, string description, string protocol, string remoteHost, int publicPort, IPEndPoint localEp, int leaseDuration)
        {
            if (description == null)
            {
                throw new ArgumentNullException("description");
            }
            if (protocol == null)
            {
                throw new ArgumentNullException("protocol");
            }
            if (localEp == null)
            {
                throw new ArgumentNullException("localEp");
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
                "<u:AddPortMapping xmlns:u=\"" + mServiceType + "\">\r\n" +
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

                SendCommand("AddPortMapping", soapBody);
            }
            catch (WebException x)
            {
                // We have UPnP exception.
                if (x.Response.ContentType.ToLower().IndexOf("text/xml", StringComparison.Ordinal) > -1)
                {
                    throw UPnPException.Parse(x.Response.GetResponseStream());
                }
            }
        }

        /// <summary>
        /// Deletes port mapping.
        /// </summary>
        /// <param name="map">NAT mapping entry to delete.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>map</b> is null reference.</exception>
        /// <exception cref="UPnPException">Is raised when UPnP device returns error.</exception>
        public void DeletePortMapping(UPnPnatMap map)
        {
            if (map == null)
            {
                throw new ArgumentNullException("map");
            }

            DeletePortMapping(map.Protocol, map.RemoteHost, Convert.ToInt32(map.ExternalPort));
        }

        /// <summary>
        /// Deletes port mapping.
        /// </summary>
        /// <param name="protocol">Port mapping protocol. Normally this value TCP or UDP.</param>
        /// <param name="remoteHost">Remote host IP address.</param>
        /// <param name="publicPort">Public port number.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>protocol</b> is null reference.</exception>
        /// <exception cref="UPnPException">Is raised when UPnP device returns error.</exception>
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
                throw new ArgumentNullException("protocol");
            }

            try
            {
                var soapBody = "<s:Envelope xmlns:s=\"http://schemas.xmlsoap.org/soap/envelope/\" s:encodingStyle=\"http://schemas.xmlsoap.org/soap/encoding/\">\r\n" +
                "<s:Body>\r\n" +
                "<u:DeletePortMapping xmlns:u=\"" + mServiceType + "\">\r\n" +
                "<NewRemoteHost>" + remoteHost + "</NewRemoteHost>\r\n" +
                "<NewExternalPort>" + publicPort + "</NewExternalPort>\r\n" +
                "<NewProtocol>" + protocol + "</NewProtocol>\r\n" +
                "</u:DeletePortMapping>\r\n" +
                "</s:Body>\r\n" +
                "</s:Envelope>\r\n";

                SendCommand("DeletePortMapping", soapBody);
            }
            catch (WebException x)
            {
                // We have UPnP exception.
                if (x.Response.ContentType.ToLower().IndexOf("text/xml", StringComparison.Ordinal) > -1)
                {
                    throw UPnPException.Parse(x.Response.GetResponseStream());
                }
            }
        }

        /// <summary>
        /// Gets NAT public IP address.
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
            "<u:GetExternalIPAddress xmlns:u=\"" + mServiceType + "\"></u:GetExternalIPAddress>\r\n" +
            "</s:Body>\r\n" +
            "</s:Envelope>\r\n";

            var soapResponse = SendCommand("GetExternalIPAddress", soapBody);

            var reader = XmlReader.Create(new StringReader(soapResponse));
            while (reader.Read())
            {
                if (string.Equals("NewExternalIPAddress", reader.Name, StringComparison.InvariantCultureIgnoreCase))
                {
                    return IPAddress.Parse(reader.ReadString());
                }
            }

            return null;
        }

        /// <summary>
        /// Gets all existing port mappings.
        /// </summary>
        /// <returns>Returns all existing port mappings.</returns>
        public UPnPnatMap[] GetPortMappings()
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

            var retVal = new List<UPnPnatMap>();
            for (var i = 0; i < 100; i++)
            {
                try
                {
                    var soapBody = "<s:Envelope xmlns:s=\"http://schemas.xmlsoap.org/soap/envelope/\" s:encodingStyle=\"http://schemas.xmlsoap.org/soap/encoding/\">\r\n" +
                    "<s:Body>\r\n" +
                    "<u:GetGenericPortMappingEntry xmlns:u=\"" + mServiceType + "\">\r\n" +
                    "<NewPortMappingIndex>" + i + "</NewPortMappingIndex>\r\n" +
                    "</u:GetGenericPortMappingEntry>\r\n" +
                    "</s:Body>\r\n" +
                    "</s:Envelope>\r\n";

                    var soapResponse = SendCommand("GetGenericPortMappingEntry", soapBody);

                    var enabled = false;
                    var protocol = "";
                    var remoteHost = "";
                    var externalPort = "";
                    var internalHost = "";
                    var internalPort = 0;
                    var description = "";
                    var leaseDuration = 0;

                    var reader = XmlReader.Create(new StringReader(soapResponse));
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

                    retVal.Add(new UPnPnatMap(enabled, protocol, remoteHost, externalPort, internalHost, internalPort, description, leaseDuration));
                }
                catch (WebException x)
                {
                    // We should see what error we got. We expect "Array out of index", other exceptions we must pass through.

                    // We have UPnP exception.
                    if (x.Response.ContentType.ToLower().IndexOf("text/xml", StringComparison.Ordinal) > -1)
                    {
                        var uX = UPnPException.Parse(x.Response.GetResponseStream());
                        // Other error than "Index out of range", we pass it through.
                        if (uX.ErrorCode != 713)
                        {
                            throw uX;
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

            return retVal.ToArray();
        }

        /// <summary>
        /// Initializes UPnP NAT info.
        /// </summary>
        private static void Init()
        {
            /* First try to get default LAN adapter gateway and check if it's UPnP nat.
               If this fails try UPnP search.
            */

            try
            {
                UPnPDevice[] devices = null;

                // Try to get gateway UPnP info, if it supports it.
                try
                {
                    IPAddress gwIP = null;
                    foreach (var adapter in NetworkInterface.GetAllNetworkInterfaces())
                    {
                        if (adapter.OperationalStatus != OperationalStatus.Up)
                        {
                            continue;
                        }

                        foreach (var gwInformation in adapter.GetIPProperties().GatewayAddresses)
                        {
                            gwIP = gwInformation.Address;
                            break;
                        }
                        break;
                    }

                    devices = UPnPClient.Search(gwIP, "urn:schemas-upnp-org:device:InternetGatewayDevice:1", 1200);
                }
                catch
                {
                    // We don't care about errors here.
                }

                if (devices == null)
                {
                    return;
                }

                // Gateway no UPnP device, search for UPnP router.
                if (devices.Length == 0)
                {
                    devices = UPnPClient.Search("urn:schemas-upnp-org:device:InternetGatewayDevice:1", 1200);
                }

                if (devices.Length <= 0)
                {
                    return;
                }

                var xml = new XmlDocument();
                xml.LoadXml(devices[0].DeviceXml);

                // Loop XML tree by nodes.
                var queue = new List<XmlNode>();
                queue.Add(xml);
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

                            mBaseUrl = devices[0].BaseUrl;
                            mServiceType = "urn:schemas-upnp-org:service:WANPPPConnection:1";
                            mControlUrl = node.InnerText;

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

                            mBaseUrl = devices[0].BaseUrl;
                            mServiceType = "urn:schemas-upnp-org:service:WANIPConnection:1";
                            mControlUrl = node.InnerText;

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
            catch
            {
                // ignored
            }
        }

        /// <summary>
        /// Sends command to UPnP device and reads response.
        /// </summary>
        /// <param name="method">Command method.</param>
        /// <param name="soapData">Soap xml.</param>
        /// <returns>Returns UPnP device response.</returns>
        private static string SendCommand(string method, string soapData)
        {
            var requestBody = Encoding.UTF8.GetBytes(soapData);

            var request = WebRequest.Create(mBaseUrl + mControlUrl);
            request.Method = "POST";
            request.Headers.Add("SOAPAction", mServiceType + "#" + method);
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
