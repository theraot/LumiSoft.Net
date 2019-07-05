using LumiSoft.Net.STUN.Client;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Experiment
{
    internal static class Program
    {
        private delegate bool TryParse<T>(string input, out T item);

        private static List<object> StaticObjects { get; } = new List<object>();

        private static T Ask<T>(string prompt, TryParse<T> tryParse)
        {
            if (tryParse == null)
            {
                throw new ArgumentNullException(nameof(tryParse));
            }
            while (true)
            {
                Clear();
                Console.WriteLine(prompt);
                if (tryParse(Console.ReadLine(), out var value))
                {
                    return value;
                }
            }
        }

        private static T Ask<T>(string prompt, TryParse<T> tryParse, T @default)
        {
            if (tryParse == null)
            {
                throw new ArgumentNullException(nameof(tryParse));
            }
            while (true)
            {
                Clear();
                Console.WriteLine(prompt);
                var input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input))
                {
                    return @default;
                }
                if (tryParse(input, out var value))
                {
                    return value;
                }
            }
        }

        private static string Ask(string query, string[] answers, string @default = null)
        {
            var validDefault = @default != null && Array.IndexOf(answers, @default) != -1;
            var prompt = $"{query} ({string.Join("/", answers)}{(validDefault ? ", empty for default = " + @default : string.Empty)})";
            string input;
            while (true)
            {
                Clear();
                Console.WriteLine(prompt);
                input = Console.ReadLine();

                if (validDefault && string.IsNullOrEmpty(input))
                {
                    return @default;
                }

                var index = Array.FindIndex(answers, check => check.Equals(input, StringComparison.InvariantCultureIgnoreCase));
                if (index != -1)
                {
                    return answers[index];
                }
            }
        }

        private static void Clear()
        {
            Console.Clear();
            foreach (var staticObject in StaticObjects)
            {
                Console.WriteLine(staticObject);
            }

            Console.WriteLine();
        }

        private static void Client(Socket socket)
        {
            var endPoint = GetEndPoint("Listener IP Address:", "Listener Port:");
            using (var udpClient = new UdpClient { Client = socket })
            {
                udpClient.Connect(endPoint.Address, endPoint.Port);
                Console.WriteLine($"Text to send to {endPoint}. (empty to exit):");
                while (true)
                {
                    var input = Console.ReadLine();
                    if (string.IsNullOrEmpty(input))
                    {
                        if (Ask("Exit?", new[] { "Y", "N" }, "N") == "Y")
                        {
                            break;
                        }

                        continue;
                    }
                    var data = Encoding.ASCII.GetBytes(input);
                    udpClient.Send(data, data.Length);
                }
            }
            Console.WriteLine("The End");
        }

        private static IPEndPoint GetEndPoint(string specifyPrompt, string ipAddressPrompt, string portPrompt, int? defaultPort = null)
        {
            return Ask(specifyPrompt, new[] { "Y", "N" }, "N") == "Y"
                ? GetEndPoint(ipAddressPrompt, portPrompt, defaultPort)
                : new IPEndPoint(IPAddress.Any, 0);
        }

        private static IPEndPoint GetEndPoint(string ipAddressPrompt, string portPrompt, int? defaultPort = null)
        {
            var port = -1;
            var ipAddress = Ask
            (
                ipAddressPrompt,
                (string input, out IPAddress address) =>
                {
                    var index = input.IndexOf(':');
                    if (index == -1)
                    {
                        if (!string.IsNullOrWhiteSpace(input))
                        {
                            return TryParseIPAddress(input, out address);
                        }
                        address = default;
                        return false;
                    }
                    var pending = input.Substring(index + 1);
                    if (TryParsePositiveInt(pending, out port))
                    {
                        input = input.Substring(0, index);
                        if (!string.IsNullOrWhiteSpace(input))
                        {
                            return TryParseIPAddress(input, out address);
                        }
                    }
                    address = default;
                    return false;
                }
            );
            if (port != -1)
            {
                return new IPEndPoint(ipAddress, port);
            }

            var staticObject = $"\nIP Address: {ipAddress}";
            StaticObjects.Add($"\nIP Address: {ipAddress}");
            port = defaultPort.HasValue
                ? Ask
                (
                    portPrompt,
                    TryParsePositiveInt,
                    defaultPort.Value
                ) : Ask<int>
                (
                    portPrompt,
                    TryParsePositiveInt
                );
            StaticObjects.Remove(staticObject);

            return new IPEndPoint(ipAddress, port);

            bool TryParseIPAddress(string input, out IPAddress address)
            {
                if (IPAddress.TryParse(input, out address))
                {
                    return true;
                }

                if (input.Length > 255)
                {
                    return false;
                }

                try
                {
                    var hostInfo = Dns.GetHostEntry(input);
                    address = hostInfo.AddressList[0];
                    return true;
                }
                catch (SocketException exception)
                {
                    GC.KeepAlive(exception);
                    return false;
                }
            }

            bool TryParsePositiveInt(string input, out int result)
            {
                return int.TryParse(input, out result) && result > 0;
            }
        }

        private static void TraceRoute(IPAddress destination)
        {
            var ipAddresses = new List<IPAddress>();
            Theraot.Net.TraceRoute.Trace
            (
                destination,
                (_, node) =>
                {
                    ipAddresses.Add(node.Address);
                    return true;
                },
                () => StaticObjects.Add( "Trace Route: " + string.Join(" -> ", ipAddresses.ConvertAll(input => input.ToString())))
            );
        }

        private static void Main()
        {
            var externalIP = IPAddress.Parse(ExternalIP.Get());
            TraceRoute(externalIP);
            var stunEndpoint = GetEndPoint("STUN Server: ", "STUN Port (empty for default = 3478)", 3478);
            var local = GetEndPoint("Specify local IP?", "Local IP Address: ", "Local Port: ");
            using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
            {
                socket.Bind(local);
                var result = STUN_Client.Query(stunEndpoint, socket);
                var publicEndPoint = result.PublicEndPoint;
                if (publicEndPoint != null)
                {
                    StaticObjects.Add($"Public End Point: {publicEndPoint}");
                }
                var netType = result.NetType;
                StaticObjects.Add($"Network Type: {netType}");
                if (netType == STUN_NetType.FullCone || netType == STUN_NetType.OpenInternet)
                {
                    if (Ask("Listen?", new[] { "Y", "N" }, "Y") == "Y")
                    {
                        Server(socket);
                        return;
                    }
                }

                Client(socket);
            }
        }

        private static void Server(Socket socket)
        {
            using (var cancellationTokenSource = new CancellationTokenSource())
            {
                var token = cancellationTokenSource.Token;
                var serverThread = new Thread
                (
                    () =>
                    {
                        var udpClient = new UdpClient
                        {
                            Client = socket
                        };
                        while (!token.IsCancellationRequested)
                        {
                            var receiveTask = udpClient.ReceiveAsync();
                            try
                            {
                                receiveTask.Wait(token);
                                var result = receiveTask.Result;
                                var receiveBytes = result.Buffer;
                                var remoteIpEndPoint = result.RemoteEndPoint;
                                var data = Encoding.ASCII.GetString(receiveBytes);
                                Console.Write(remoteIpEndPoint.Address + ":" + data);
                            }
                            catch (OperationCanceledException exception)
                            {
                                GC.KeepAlive(exception);
                            }
                        }
                    }
                );
                StaticObjects.Add("Listening");
                serverThread.Start();
                IPEndPoint target = null;
                object staticObject = null;
                while (true)
                {
                    if (target == null)
                    {
                        target = GetEndPoint("Target IP: ", "Target Port: ");
                        staticObject = $"Text to send to: {target}. (empty to exit)";
                        StaticObjects.Add(staticObject);
                    }
                    else
                    {
                        var text = Console.ReadLine();
                        if (string.IsNullOrWhiteSpace(text))
                        {
                            if (Ask("Exit?", new[] { "Y", "N" }, "N") == "Y")
                            {
                                break;
                            }
                        }
                        else if (text == ".")
                        {
                            target = null;
                            StaticObjects.Remove(staticObject);
                            staticObject = null;
                        }
                        else
                        {
                            socket.SendTo(Encoding.ASCII.GetBytes(text), target);
                        }
                    }
                }
                cancellationTokenSource.Cancel(false);
            }

            Console.WriteLine("The End");
        }
    }
}