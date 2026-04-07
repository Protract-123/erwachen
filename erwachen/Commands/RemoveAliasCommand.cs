using System;
using System.ComponentModel;
using System.Threading;
using erwachen.Core;
using Spectre.Console;
using Spectre.Console.Cli;

namespace erwachen.Commands;

public sealed class RemoveAliasCommand : Command<RemoveAliasCommand.Settings>
{
    public sealed class Settings : CommandSettings
    {
        [Description("Name of device")]
        [CommandArgument(0, "<name>")]
        public string? DeviceName { get; init; }
    }

    public override int Execute(CommandContext context, Settings settings, CancellationToken cancellationToken)
    {
        try
        {
            AliasManager.RemoveAlias(settings.DeviceName!);
            return 0;
        }
        catch (InvalidOperationException exception)
        {
            AnsiConsole.MarkupLine($"[bold red]{Markup.Escape(exception.Message)}[/]");
            return 1;
        }
    }
}