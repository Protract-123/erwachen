using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using System.Text.RegularExpressions;
using Spectre.Console.Cli;
using erwachen.Utils;
using JetBrains.Annotations;

namespace erwachen;

[UsedImplicitly]
public class WakeCommand : Command<WakeCommand.Settings>
{
    [UsedImplicitly]
    public sealed class Settings : CommandSettings
    {
        [Description("Alias of device to wake")]
        [CommandArgument(0, "[alias]")]
        public string? Alias { get; init; }

        [Description("IP Address to send to")]
        [CommandOption("-i|--ip")]
        [DefaultValue("255.255.255.255")]
        public string? Ip { get; set; }

        [Description("Port number to use")]
        [CommandOption("-p|--port")]
        [DefaultValue(9)]
        public int Port { get; set; }
        
        [Description("Arbitrary hardware address to use")]
        [CommandOption("-h|--hw")]
        public string? HardwareAddress { get; set; }
    }

    private static int SendMagicPacket(string macAddress, string ip, int port)
    {
        // Validate IP
        if (!FormatCheckers.IsValidIpAddress(ip))
        {
            Console.WriteLine($"Invalid IP address: {ip}");
            return 1;
        }

        // Validate Port
        if (!FormatCheckers.IsValidPort(port))
        {
            Console.WriteLine($"Invalid port number: {port}");
            return 1;
        }
        
        // Validate Hardware Address
        if (!FormatCheckers.IsValidHardwareAddress(macAddress))
        {
            Console.WriteLine($"Invalid MAC address: {macAddress}");
            return 1;
        }

        byte[] magicPacket = GenerateMagicPacket(macAddress);

        using UdpClient udpClient = new();
        // Enable broadcasting
        udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);

        IPEndPoint endPoint = new(IPAddress.Parse(ip), port);
        udpClient.Send(magicPacket, magicPacket.Length, endPoint);

        return 0;
    }

    private static byte[] GenerateMagicPacket(string mac)
    {
        string cleanedMac = Regex.Replace(mac, @"[-:]", "");
        if (cleanedMac.Length != 12)
        {
            throw new ArgumentException("Invalid MAC address format after cleaning.");
        }

        List<byte> packet = new();

        // Add 6 bytes of 0xFF
        for (int i = 0; i < 6; i++)
        {
            packet.Add(0xFF);
        }

        // Repeat the MAC address 16 times
        byte[] macBytes = HexStringToByteArray(cleanedMac);
        for (int i = 0; i < 16; i++)
        {
            foreach (byte b in macBytes)
            {
                packet.Add(b);
            }
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

    private static string GetMacFromAlias(string alias)
    {
        string aliasConfigPath = Path.Combine(AppPaths.WriteDirectory, "alias.json");

        if (!File.Exists(aliasConfigPath)) throw new SystemException("Could not find alias config file");
        
        Dictionary<string, string>? aliases = JsonSerializer.Deserialize<Dictionary<string, string>>
            (File.ReadAllText(aliasConfigPath));

        if (!aliases!.TryGetValue(alias, out string? hwAddr))
            throw new SystemException("An alias with this name does not exist!");
            
        return hwAddr;
    } 

    public override int Execute(CommandContext context, Settings settings)
    {
        string macAddress = "";
        if (settings.Alias != null)
        {
            macAddress = GetMacFromAlias(settings.Alias);
        }
        
        else if (settings.HardwareAddress != null)
        {
            macAddress = settings.HardwareAddress;
            if (!FormatCheckers.IsValidHardwareAddress(macAddress))
            {
                Console.WriteLine($"Invalid MAC address: {macAddress}");
                return 1;
            }
        }
        
        return SendMagicPacket(macAddress, settings.Ip!, settings.Port);
    }
}