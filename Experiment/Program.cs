using LumiSoft.Net.STUN.Client;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Experiment
{
    internal static class Program
    {
        public delegate bool TryParse<T>(string input, out T item);

        private static T Ask<T>(string prompt, TryParse<T> tryParse)
        {
            while (true)
            {
                Console.WriteLine(prompt);
                if (tryParse(Console.ReadLine(), out var value))
                {
                    return value;
                }
            }
        }

        private static T Ask<T>(string prompt, TryParse<T> tryParse, T @default)
        {
            while (true)
            {
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
            do
            {
                Console.WriteLine(prompt);
                input = Console.ReadLine();

                if (validDefault && string.IsNullOrEmpty(input))
                {
                    return @default;
                }
            } while (Array.FindIndex(answers, check => check.Equals(input, StringComparison.InvariantCultureIgnoreCase)) == -1);
            return input;
        }

        private static void Main()
        {
            var stunEndpoint = GetEndPoint("STUN Server: ", "STUN Port (empty for default = 3478)", 3478);
            var local = GetEndPoint("Specify local IP?", "Local IP Address: ", "Local Port: ");
            var result = STUN_Client.Query(stunEndpoint, local);
            var publicEndPoint = result.PublicEndPoint;
            if (publicEndPoint != null)
            {
                Console.WriteLine(publicEndPoint.ToString());
            }
            var netType = result.NetType;
            Console.WriteLine(netType.ToString());
            if (netType == STUN_NetType.FullCone || netType == STUN_NetType.OpenInternet)
            {
                if (Ask("Listen?", new[] { "Y", "N" }, "Y") == "Y")
                {
                    Server(publicEndPoint);
                    return;
                }
            }

            Client();
        }

        private static void Client()
        {
            var endPoint = GetEndPoint("Listener IP Address:", "Listener Port:");
            using (var udpClient = new UdpClient())
            {
                udpClient.Connect(endPoint.Address, endPoint.Port);
                Console.WriteLine("Text to send (empty to exit):");
                while (true)
                {
                    var input = Console.ReadLine();
                    if (string.IsNullOrEmpty(input))
                    {
                        break;
                    }
                    var data = Encoding.ASCII.GetBytes(input);
                    udpClient.Send(data, data.Length);
                }
            }
            Console.WriteLine("The End");
        }

        private static void Server(IPEndPoint publicEndPoint)
        {
            using (var cancellationTokenSource = new CancellationTokenSource())
            {
                var token = cancellationTokenSource.Token;
                var serverThread = new Thread
                (
                    () =>
                    {
                        var udpClient = new UdpClient(publicEndPoint.Port);
                        Console.WriteLine($"Listening on {udpClient.Client.LocalEndPoint as IPEndPoint}");
                        while (!token.IsCancellationRequested)
                        {
                            var receiveTask = udpClient.ReceiveAsync();
                            receiveTask.Wait(token);
                            if (receiveTask.IsCanceled)
                            {
                                break;
                            }

                            var result = receiveTask.Result;
                            var receiveBytes = result.Buffer;
                            var remoteIpEndPoint = result.RemoteEndPoint;
                            var data = Encoding.ASCII.GetString(receiveBytes);
                            Console.Write(remoteIpEndPoint.Address + ":" + data);
                        }
                    }
                );
                Console.WriteLine("Press any key to exit");
                serverThread.Start();
                Console.ReadKey();
                cancellationTokenSource.Cancel();
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
            var localPort = -1;
            var localIPAddress = Ask
            (
                ipAddressPrompt,
                (string input, out IPAddress ipAddress) =>
                {
                    var index = input.IndexOf(':');
                    if (index == -1)
                    {
                        return TryParseIPAddress(input, out ipAddress);
                    }
                    var pending = input.Substring(index + 1);
                    if (TryParsePositiveInt(pending, out localPort))
                    {
                        input = input.Substring(0, index);
                        return TryParseIPAddress(input, out ipAddress);
                    }
                    ipAddress = default;
                    return false;
                }
            );
            if (localPort != -1)
            {
                return new IPEndPoint(localIPAddress, localPort);
            }
            if (defaultPort.HasValue)
            {
                localPort = Ask
                (
                    portPrompt,
                    TryParsePositiveInt,
                    defaultPort.Value
                );
            }
            else
            {
                localPort = Ask<int>
                (
                    portPrompt,
                    TryParsePositiveInt
                );
            }

            return new IPEndPoint(localIPAddress, localPort);

            bool TryParseIPAddress(string input, out IPAddress ipAddress)
            {
                if (IPAddress.TryParse(input, out ipAddress))
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
                    ipAddress = hostInfo.AddressList[0];
                    return true;
                }
                catch (SocketException exception)
                {
                    GC.KeepAlive(exception);
                    return false;
                }
            }

            bool TryParsePositiveInt(string input, out int port)
            {
                return int.TryParse(input, out port) && port > 0;
            }
        }
    }
}