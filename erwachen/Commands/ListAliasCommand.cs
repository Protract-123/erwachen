using System.Collections.Generic;
using System.Threading;
using erwachen.Core;
using Spectre.Console;
using Spectre.Console.Cli;

namespace erwachen.Commands;

public sealed class ListAliasCommand : Command
{
    public override int Execute(CommandContext context, CancellationToken cancellationToken)
    {
        List<Alias> aliases = AliasManager.GetAllAliases();

        if (aliases.Count == 0)
        {
            AnsiConsole.MarkupLine("[yellow]No devices registered yet.[/]");
            return 0;
        }

        Table devicesTable = new Table()
            .Title("[fuchsia]Registered Devices[/]")
            .RoundedBorder()
            .AddColumn(new TableColumn("[bold]#[/]").RightAligned())
            .AddColumn(new TableColumn("[bold]Name[/]"))
            .AddColumn(new TableColumn("[bold]MAC Address[/]"));

        for (int rowIndex = 0; rowIndex < aliases.Count; rowIndex++)
        {
            Alias alias = aliases[rowIndex];
            devicesTable.AddRow(
                $"[grey]{rowIndex + 1}[/]",
                $"[cyan]{Markup.Escape(alias.Name)}[/]",
                $"[yellow]{Markup.Escape(alias.MacAddress)}[/]");
        }

        AnsiConsole.Write(devicesTable);

        return 0;
    }
}