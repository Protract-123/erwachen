using System;
using System.Linq;
using System.Net;

namespace erwachen.Core;

public static class FormatCheckers
{
    public static bool IsValidIpAddress(string ipAddress) => IPAddress.TryParse(ipAddress, out _);
    public static bool IsValidPort(int port) => port is >= 0 and <= 65535;
    public static bool IsValidMacAddress(string macAddress) => macAddress.Count(Uri.IsHexDigit) == 12;

    public static bool IsValidAliasName(string aliasName) =>
        aliasName.Length > 0 &&
        aliasName.All(character => char.IsLetterOrDigit(character) || character == '_' || character == '-');
}