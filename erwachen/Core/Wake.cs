using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace erwachen.Core;

public static class Wake
{
    public static void SendMagicPacket(string macAddress, string ipAddress, int port)
    {
        if (!FormatCheckers.IsValidIpAddress(ipAddress))
            throw new ArgumentException("Invalid IP address");

        if (!FormatCheckers.IsValidPort(port))
            throw new ArgumentException("Invalid port number");

        if (!FormatCheckers.IsValidMacAddress(macAddress))
            throw new ArgumentException("Invalid MAC address");

        byte[] magicPacket = GenerateMagicPacket(macAddress);

        using UdpClient udpClient = new();
        udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);

        IPEndPoint endPoint = new(IPAddress.Parse(ipAddress), port);
        udpClient.Send(magicPacket, magicPacket.Length, endPoint);
    }

    private static byte[] GenerateMagicPacket(string macAddress)
    {
        // Magic Packet format is 6 * 0xFF + 16 * macAddress
        byte[] magicPacket = new byte[6 + 16 * 6];
        byte[] macAddressBytes = Convert.FromHexString(ExtractMacAddress(macAddress));

        magicPacket.AsSpan(0, 6).Fill(0xFF);
        for (int i = 0; i < 16; i++)
            macAddressBytes.CopyTo(magicPacket, 6 + i * 6);

        return magicPacket;
    }

    private static string ExtractMacAddress(string macAddress) =>
        new(macAddress.Where(Uri.IsHexDigit).ToArray());
}