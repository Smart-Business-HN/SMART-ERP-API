using System.Net;
using System.Net.Sockets;

namespace SMART.ERP.Infrastructure.Helpers
{
    public class IpHelper
    {
        public static string GetIpAddress()
        {
            var hots = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in hots.AddressList)
            {
                if(ip.AddressFamily == AddressFamily.InterNetwork) { return ip.ToString(); }
            }

            return String.Empty;
        }
    }
}
