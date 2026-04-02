using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;

namespace erwachen.Core;

public static partial class Wake
{
    public static void SendMagicPacket(string macAddress, string ip, int port)
    {
        if (!FormatCheckers.IsValidIpAddress(ip))
        {
            throw new ArgumentException("Invalid IP address");
        }

        if (!FormatCheckers.IsValidPort(port))
        {
            throw new ArgumentException("Invalid port number");
        }
        
        if (!FormatCheckers.IsValidHardwareAddress(macAddress))
        {
            throw new ArgumentException("Invalid MAC address");
        }

        byte[] magicPacket = GenerateMagicPacket(macAddress);

        using UdpClient udpClient = new();
        udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);

        IPEndPoint endPoint = new(IPAddress.Parse(ip), port);
        udpClient.Send(magicPacket, magicPacket.Length, endPoint);
    }

    private static byte[] GenerateMagicPacket(string mac)
    {
        string cleanedMac = MacSeparatorRegex().Replace(mac, string.Empty);

        List<byte> packet = [];

        for (int i = 0; i < 6; i++)
        {
            packet.Add(0xFF);
        }

        byte[] macBytes = HexStringToByteArray(cleanedMac);
        for (int i = 0; i < 16; i++)
        {
            packet.AddRange(macBytes);
        }

        return packet.ToArray();
    }

    private static byte[] HexStringToByteArray(string hex)
    {
        int length = hex.Length;
        byte[] result = new byte[length / 2];

        for (int i = 0; i < length; i += 2)
        {
            string pair = hex.Substring(i, 2);
            result[i / 2] = Convert.ToByte(pair, 16);
        }

        return result;
    }

    [GeneratedRegex("[-:]")]
    private static partial Regex MacSeparatorRegex();
}