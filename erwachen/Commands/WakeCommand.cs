using System;
using System.ComponentModel;
using System.Threading;
using erwachen.Core;
using Spectre.Console;
using Spectre.Console.Cli;

namespace erwachen.Commands;

public sealed class WakeCommand : Command<WakeCommand.Settings>
{
    public sealed class Settings : CommandSettings
    {
        [Description("Device name or MAC address to wake")]
        [CommandArgument(0, "<identifier>")]
        public string? Identifier { get; init; }

        [Description("IP Address to send to")]
        [CommandOption("-i|--ip")]
        [DefaultValue("255.255.255.255")]
        public string? Ip { get; init; }

        [Description("Port number to use")]
        [CommandOption("-p|--port")]
        [DefaultValue(9)]
        public int Port { get; init; }
    }

    public override int Execute(CommandContext context, Settings settings, CancellationToken cancellationToken)
    {
        string? macAddress = null;

        if (AliasManager.TryGetMacFromAlias(settings.Identifier!, out string resolvedMac)) macAddress = resolvedMac;
        else if (FormatCheckers.IsValidMacAddress(settings.Identifier!)) macAddress = settings.Identifier;

        if (macAddress is null)
        {
            AnsiConsole.MarkupLine($"[bold red]Invalid identifier: {Markup.Escape(settings.Identifier!)}[/]");
            return 1;
        }

        try
        {
            Wake.SendMagicPacket(macAddress, settings.Ip!, settings.Port);
            return 0;
        }
        catch (Exception exception)
        {
            AnsiConsole.MarkupLine($"[bold red]Failed to send magic packet: {Markup.Escape(exception.Message)}[/]");
            return 1;
        }
    }
}