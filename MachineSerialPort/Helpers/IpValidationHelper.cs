using System.Net;
using System.Text.RegularExpressions;

namespace MachineSerialPort.Helpers
{
    public static class IpValidationHelper
    {
        private static readonly Regex IpRegex = new(@"^((25[0-5]|(2[0-4]|1\d|[1-9]|)\d)\.){3}(25[0-5]|(2[0-4]|1\d|[1-9]|)\d)$");

        public static bool IsValidIp(string ipAddress)
        {
            if (string.IsNullOrWhiteSpace(ipAddress)) return false;

            // Checks string pattern and valid byte ranges (0-255)
            return IpRegex.IsMatch(ipAddress.Trim()) && IPAddress.TryParse(ipAddress.Trim(), out _);
        }
    }
}
