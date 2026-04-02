using System.Net;
using System.Text.RegularExpressions;

namespace erwachen.Core;

public static class FormatCheckers
{
    private static readonly Regex[] HardwareAddressRegexs =
    [
        new(@"^(?:[\da-f]{2}:){5}[\da-f]{2}$", RegexOptions.IgnoreCase),
        new(@"^(?:[\da-f]{2}-){5}[\da-f]{2}$", RegexOptions.IgnoreCase),
        new(@"^[\da-f]{6}-[\da-f]{6}$", RegexOptions.IgnoreCase),
        new(@"^[\da-f]{12}$", RegexOptions.IgnoreCase)
    ];
    
    public static bool IsValidIpAddress(string ip)
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

    public static bool IsValidPort(int port) => port is >= 0 and <= 65535;

    public static bool IsValidHardwareAddress(string hardwareAddress)
    {
        if (string.IsNullOrWhiteSpace(hardwareAddress))
            return false;

        foreach (Regex regex in HardwareAddressRegexs)
        {
            if (regex.IsMatch(hardwareAddress))
                return true;
        }
        return false;
    }
}