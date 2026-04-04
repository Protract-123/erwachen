using System;
using System.Linq;
using System.Net;

namespace erwachen.Core;

public static class FormatCheckers
{
    public static bool IsValidIpAddress(string ip) => IPAddress.TryParse(ip, out _);
    public static bool IsValidPort(int port) => port is >= 0 and <= 65535;
    public static bool IsValidHardwareAddress(string hardwareAddress) => hardwareAddress.Count(Uri.IsHexDigit) == 12;
}