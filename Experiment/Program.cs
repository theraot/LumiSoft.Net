using LumiSoft.Net.STUN.Client;
using System;
using System.Net;

namespace Experiment
{
    internal static class Program
    {
        public delegate bool TryParse<T>(string input, out T item);

        private static T Ask<T>(string prompt, TryParse<T> tryParse)
        {
            while(true)
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
            while(true)
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

        private static string Ask(string prompt)
        {
            string input;
            do
            {
                Console.WriteLine(prompt);
                input = Console.ReadLine();
            } while (string.IsNullOrWhiteSpace(input));
            return input;
        }

        private static string Ask(string query, string[] answers)
        {
            string prompt = $"{query} ({string.Join("/", answers)})";
            string input;
            do
            {
                Console.WriteLine(prompt);
                input = Console.ReadLine();
            } while (Array.FindIndex(answers, check => check.Equals(input, StringComparison.InvariantCultureIgnoreCase)) == -1);
            return input;
        }

        private static void Main()
        {
            var server = Ask("STUN Server: ");
            var serverPort = Ask("STUN Port (empty for default = 3478): ", int.TryParse, 3478);
            var local = GetEndPoint("Specify local IP?", "Local IP Address: ", "Local Port: ");
            var result = STUN_Client.Query(server, serverPort, local);
            var netType = result.NetType;
            Console.WriteLine(netType.ToString());
            var publicEndPoint = result.PublicEndPoint;
            if (publicEndPoint != null)
            {
                Console.WriteLine(publicEndPoint.ToString());
            }
        }

        private static IPEndPoint GetEndPoint(string specifyPrompt, string ipAddressPrompt, string portPrompt)
        {
            IPEndPoint endPoint;
            if (Ask(specifyPrompt, new[] { "Y", "N" }) == "Y")
            {
                var localIPAddress = Ask<IPAddress>(ipAddressPrompt, IPAddress.TryParse);
                var localPort = Ask<int>(portPrompt, int.TryParse);
                endPoint = new IPEndPoint(localIPAddress, localPort);
            }
            else
            {
                endPoint = new IPEndPoint(IPAddress.Any, 0);
            }
            return endPoint;
        }
    }
}
