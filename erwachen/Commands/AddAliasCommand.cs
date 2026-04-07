using System;
using System.ComponentModel;
using System.Threading;
using erwachen.Core;
using Spectre.Console;
using Spectre.Console.Cli;

namespace erwachen.Commands;

public sealed class AddAliasCommand : Command<AddAliasCommand.Settings>
{
    public sealed class Settings : CommandSettings
    {
        [Description("Name of device")]
        [CommandArgument(0, "<name>")]
        public string? DeviceName { get; init; }

        [Description("MAC address of device")]
        [CommandArgument(1, "<macAddress>")]
        public string? MacAddress { get; init; }
    }

    public override int Execute(CommandContext context, Settings settings, CancellationToken cancellationToken)
    {
        if (!FormatCheckers.IsValidAliasName(settings.DeviceName!))
        {
            AnsiConsole.MarkupLine(
                "[bold red]Invalid device name. Use only letters, numbers, hyphens and underscores[/]");
            return 1;
        }

        if (!FormatCheckers.IsValidMacAddress(settings.MacAddress!))
        {
            AnsiConsole.MarkupLine("[bold red]Invalid MAC address[/]");
            return 1;
        }

        try
        {
            AliasManager.AddAlias(new Alias(settings.DeviceName!, settings.MacAddress!));
            return 0;
        }
        catch (InvalidOperationException exception)
        {
            AnsiConsole.MarkupLine($"[bold red]{Markup.Escape(exception.Message)}[/]");
            return 1;
        }
    }
}