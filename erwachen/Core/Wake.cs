using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace erwachen.Core;

public static class Wake
{
    public static void SendMagicPacket(string macAddress, string ip, int port)
    {
        if (!FormatCheckers.IsValidIpAddress(ip)) throw new ArgumentException("Invalid IP address");
        if (!FormatCheckers.IsValidPort(port)) throw new ArgumentException("Invalid port number");
        if (!FormatCheckers.IsValidHardwareAddress(macAddress)) throw new ArgumentException("Invalid MAC address");
        
        byte[] magicPacket = GenerateMagicPacket(macAddress);

        using UdpClient udpClient = new();
        udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);

        IPEndPoint endPoint = new(IPAddress.Parse(ip), port);
        udpClient.Send(magicPacket, magicPacket.Length, endPoint);
    }

    private static byte[] GenerateMagicPacket(string mac)
    {
        byte[] macBytes = Convert.FromHexString(ExtractHardwareAddress(mac));
        byte[] packet = new byte[6 + 16 * 6];

        packet.AsSpan(0, 6).Fill(0xFF);
        for (int i = 0; i < 16; i++) macBytes.CopyTo(packet, 6 + i * 6);
        
        return packet;
    }

    private static string ExtractHardwareAddress(string hardwareAddress) => new (hardwareAddress.Where(Uri.IsHexDigit).ToArray());
}