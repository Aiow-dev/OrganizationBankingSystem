using System.Net.NetworkInformation;

namespace OrganizationBankingSystem.Core.Helpers
{
    public static class NetworkHelper
    {
        public static bool CheckInternetConnection()
        {
            try
            {
                if (!NetworkInterface.GetIsNetworkAvailable())
                {
                    return false;
                }

                Ping ping = new();
                PingReply pingReply = ping.Send("google.com", 1000, new byte[32], new PingOptions());

                return (pingReply.Status == IPStatus.Success);
            }
            catch (PingException)
            {
                return false;
            }
        }
    }
}