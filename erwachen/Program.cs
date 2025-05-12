using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.Intrinsics.X86;
using System.Text.RegularExpressions;

namespace erwachen
{
    internal partial class Program
    {
        #region Constants and Global Variables
        
            private static readonly Regex[] HardwareAddressRegex = new Regex[]
            {
                CanonicalHardwareAddress(),
                WindowsHardwareAddress(),
                HewlettPackardSwitchAddress(),
                IntelLandeskAddress()
            };

            private const string DefaultIp = "255.255.255.255";
            private const int DefaultPort = 9;
            
        #endregion
        
        private static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }

        #region Helper Functions

            private static bool IsValidIpAddress(string ip)
            {
                if (string.IsNullOrWhiteSpace(ip))
                    return false;

                try
                {
                    IPAddress.Parse(ip);
                    return true;
                }
                catch
                {
                    return false;
                }
            }

            private static bool IsValidPort(int port) => port >= 0 && port <= 65535;

            private static bool IsValidHardwareAddress(string hardwareAddress)
            {
                if (string.IsNullOrWhiteSpace(hardwareAddress))
                    return false;

                foreach (var re in HardwareAddressRegex)
                {
                    if (re.IsMatch(hardwareAddress))
                        return true;
                }
                return false;
            }

        #endregion

        #region Regex Functions

            [GeneratedRegex(@"^(?:[\da-f]{1,2}:){5}[\da-f]{1,2}$", RegexOptions.IgnoreCase)]
            private static partial Regex CanonicalHardwareAddress();
            [GeneratedRegex(@"^(?:[\da-f]{1,2}-){5}[\da-f]{1,2}$", RegexOptions.IgnoreCase)]
            private static partial Regex WindowsHardwareAddress();
            [GeneratedRegex(@"^[\da-f]{6}-[\da-f]{6}$", RegexOptions.IgnoreCase)]
            private static partial Regex HewlettPackardSwitchAddress();
            [GeneratedRegex(@"^[\da-f]{12}$", RegexOptions.IgnoreCase)]
            private static partial Regex IntelLandeskAddress();        

        #endregion

    }
}