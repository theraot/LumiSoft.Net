using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Experiment
{
    internal static class ExternalIP
    {
        private static string _externalIP;

        public static string Get()
        {
            return _externalIP ?? (_externalIP = Get(700).Result);
        }

        private static async Task<string> Get(int timeoutMilliseconds)
        {
            var sites = new[] {
                "http://api.ipify.org",         // free, claims to have no limit
                "http://ifconfig.me/ip",        // free, no information on whatever it has a limit
                "http://ipecho.net/plain",      // free, asks to not abuse
                "http://icanhazip.com",         // free, asks to not abuse 
                "http://ifconfig.co/ip",        // free, rate limited
                "https://myexternalip.com/raw", // free, rate limited, slow
                "http://ipinfo.io/ip",          // commercial, rate limited, slow
            };
            using (var httpClient = new HttpClient{Timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds)})
            {
                foreach (var site in sites)
                {
                    try
                    {
                        return (await httpClient.GetStringAsync(site).ConfigureAwait(false)).Trim();
                    }
                    catch (HttpRequestException exception)
                    {
                        Console.WriteLine($"Error trying to get external IP address from {site}: {exception}");
                    }
                }
            }

            return null;
        }
    }
}