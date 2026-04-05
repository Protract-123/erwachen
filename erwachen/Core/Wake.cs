using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace erwachen.Core;

public static class Wake
{
    public static void SendMagicPacket(string macAddress, string ipAddress, int port)
    {
        if (!FormatCheckers.IsValidIpAddress(ipAddress)) throw new ArgumentException("Invalid IP address");
        if (!FormatCheckers.IsValidPort(port)) throw new ArgumentException("Invalid port number");
        if (!FormatCheckers.IsValidMacAddress(macAddress)) throw new ArgumentException("Invalid MAC address");

        byte[] magicPacket = GenerateMagicPacket(macAddress);

        using UdpClient udpClient = new();
        udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);

        IPEndPoint endPoint = new(IPAddress.Parse(ipAddress), port);
        udpClient.Send(magicPacket, magicPacket.Length, endPoint);
    }

    private static byte[] GenerateMagicPacket(string macAddress)
    {
        byte[] macBytes = Convert.FromHexString(ExtractMacAddress(macAddress));
        byte[] packet = new byte[6 + 16 * 6];

        packet.AsSpan(0, 6).Fill(0xFF);
        for (int repetitionIndex = 0; repetitionIndex < 16; repetitionIndex++)
            macBytes.CopyTo(packet, 6 + repetitionIndex * 6);

        return packet;
    }

    private static string ExtractMacAddress(string macAddress) =>
        new(macAddress.Where(Uri.IsHexDigit).ToArray());
}