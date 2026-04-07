using System;
using System.Collections.Generic;
using erwachen.Core;
using Spectre.Console;

namespace erwachen;

public static class InteractiveShell
{
    private sealed record Action(string Key, string Label, string Description);

    private static readonly List<Action> ActionList =
    [
        new("wake", "Wake a device", "Send a magic packet to a registered or arbitrary device"),
        new("add", "Add a device", "Register a new device"),
        new("remove", "Remove a device", "Remove a registered device"),
        new("list", "List devices", "Show all registered devices"),
        new("exit", "Exit", "Leave the shell"),
    ];

    private static readonly FigletFont FigletFont = FigletFont.Load(ThisAssembly.Resources.Assets.Terminus.GetStream());
    private const int FigletCharWidth = 6;
    private const string BroadcastAddress = "255.255.255.255";
    private const int WakeOnLanPort = 9;

    public static void RunInteractive()
    {
        while (true)
        {
            AnsiConsole.Clear();
            RenderBanner();

            AnsiConsole.MarkupLine("[fuchsia]What would you like to do?[/]");
            Action selectedAction = AnsiConsole.Prompt(new SelectionPrompt<Action>()
                .WrapAround()
                .HighlightStyle(new Style(foreground: Color.Fuchsia, decoration: Decoration.Bold))
                .UseConverter(action => $"{action.Label} [grey]- {action.Description}[/]")
                .AddChoices(ActionList));

            if (selectedAction.Key == "exit")
            {
                AnsiConsole.MarkupLine("Goodbye.");
                return;
            }

            AnsiConsole.Clear();
            RenderBanner();
            AnsiConsole.Write(new Rule($"[fuchsia]{selectedAction.Label}[/]").LeftJustified().RuleStyle("grey"));
            AnsiConsole.WriteLine();

            try
            {
                RunCommand(selectedAction.Key);
            }
            catch (Exception exception)
            {
                AnsiConsole.MarkupLine($"[red]{Markup.Escape(exception.Message)}[/]");
            }

            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine("Press any key to return to menu...");
            Console.ReadKey(true);
        }
    }

    private static void RenderBanner()
    {
        const string title = "erwachen";
        AnsiConsole.Write(new FigletText(FigletFont, title).LeftJustified().Color(Color.Fuchsia));

        const string subtitle = $"Wake-on-LAN for humans · v{Program.Version}";
        int bannerWidth = title.Length * FigletCharWidth;
        int padding = Math.Max(0, (bannerWidth - subtitle.Length) / 2);
        AnsiConsole.MarkupLine(new string(' ', padding) + subtitle);
        AnsiConsole.WriteLine();
    }

    private static void RunCommand(string command)
    {
        switch (command)
        {
            case "wake": HandleWake(); break;
            case "add": HandleAdd(); break;
            case "remove": HandleRemove(); break;
            case "list": HandleList(); break;
        }
    }

    private static void HandleWake()
    {
        List<Alias> aliases = AliasManager.GetAllAliases();
        Alias manualSentinel = new("__manual__", "");

        Alias selectedAlias;

        if (aliases.Count > 0)
        {
            List<Alias> aliasChoices = [..aliases, manualSentinel];

            AnsiConsole.MarkupLine("[fuchsia]Which device would you like to wake?[/]");
            selectedAlias = AnsiConsole.Prompt(new SelectionPrompt<Alias>()
                .HighlightStyle(new Style(foreground: Color.Fuchsia, decoration: Decoration.Bold))
                .UseConverter(alias => ReferenceEquals(alias, manualSentinel)
                    ? "[grey]- Enter MAC manually -[/]"
                    : $"{alias.Name} [grey]{alias.MacAddress}[/]")
                .AddChoices(aliasChoices)
                .WrapAround());
        }
        else selectedAlias = manualSentinel;


        string deviceName;
        string macAddress;

        if (ReferenceEquals(selectedAlias, manualSentinel))
        {
            deviceName = "Arbitrary device";
            macAddress = AnsiConsole.Prompt(new TextPrompt<string>("[bold]MAC address of device[/]:")
                .Validate(FormatCheckers.IsValidMacAddress, "[bold red]Invalid MAC address[/]"));
        }
        else
        {
            deviceName = selectedAlias.Name;
            macAddress = selectedAlias.MacAddress;
        }

        Table detailsTable = new Table()
            .HideHeaders()
            .RoundedBorder()
            .AddColumn(new TableColumn(""))
            .AddColumn(new TableColumn(""))
            .AddRow("[bold]Name[/]", $"[cyan]{Markup.Escape(deviceName)}[/]")
            .AddRow("[bold]MAC[/]", $"[yellow]{Markup.Escape(macAddress)}[/]");

        AnsiConsole.Write(detailsTable);
        AnsiConsole.WriteLine();

        if (!AnsiConsole.Confirm("Send magic packet to this device?"))
        {
            AnsiConsole.MarkupLine("[grey]Cancelled.[/]");
            return;
        }

        AnsiConsole.Status()
            .Spinner(Spinner.Known.Dots)
            .SpinnerStyle(new Style(foreground: Color.Fuchsia))
            .Start("Sending magic packet...", _ => Wake.SendMagicPacket(macAddress, BroadcastAddress, WakeOnLanPort));

        AnsiConsole.MarkupLine($"[green]Magic packet sent to [cyan]{Markup.Escape(deviceName)}[/][/]");
    }

    private static void HandleAdd()
    {
        string aliasName = AnsiConsole.Prompt(new TextPrompt<string>("[bold]Name of device[/]:")
            .Validate(FormatCheckers.IsValidAliasName,
                "[bold red]Name can only contain letters, numbers and separators (- or _)[/]"));

        string macAddress = AnsiConsole.Prompt(new TextPrompt<string>("[bold]MAC address of device[/]:")
            .Validate(FormatCheckers.IsValidMacAddress, "[bold red]Invalid MAC address[/]"));

        Table detailsTable = new Table()
            .HideHeaders()
            .RoundedBorder()
            .AddColumn(new TableColumn(""))
            .AddColumn(new TableColumn(""))
            .AddRow("[bold]Name[/]", $"[cyan]{Markup.Escape(aliasName)}[/]")
            .AddRow("[bold]MAC[/]", $"[yellow]{Markup.Escape(macAddress)}[/]");

        AnsiConsole.Write(detailsTable);
        AnsiConsole.WriteLine();

        if (!AnsiConsole.Confirm("Register this device?"))
        {
            AnsiConsole.MarkupLine("[grey]Cancelled.[/]");
            return;
        }

        AliasManager.AddAlias(new Alias(aliasName, macAddress));
        AnsiConsole.MarkupLine(
            $"[green]Added [cyan]{Markup.Escape(aliasName)}[/] [grey]({Markup.Escape(macAddress)})[/][/]");
    }

    private static void HandleRemove()
    {
        List<Alias> aliases = AliasManager.GetAllAliases();

        if (aliases.Count == 0)
        {
            AnsiConsole.MarkupLine("[yellow]No devices registered yet.[/]");
            return;
        }

        Alias selectedAlias;

        if (aliases.Count > 1)
        {
            AnsiConsole.MarkupLine("[fuchsia]Which device would you like to remove?[/]");
            selectedAlias = AnsiConsole.Prompt(new SelectionPrompt<Alias>()
                .HighlightStyle(new Style(foreground: Color.Fuchsia, decoration: Decoration.Bold))
                .UseConverter(alias => $"{alias.Name} [grey]{alias.MacAddress}[/]")
                .AddChoices(aliases)
                .WrapAround());
        }
        else selectedAlias = aliases[0];


        Table detailsTable = new Table()
            .HideHeaders()
            .RoundedBorder()
            .AddColumn(new TableColumn(""))
            .AddColumn(new TableColumn(""))
            .AddRow("[bold]Name[/]", $"[cyan]{Markup.Escape(selectedAlias.Name)}[/]")
            .AddRow("[bold]MAC[/]", $"[yellow]{Markup.Escape(selectedAlias.MacAddress)}[/]");

        AnsiConsole.Write(detailsTable);
        AnsiConsole.WriteLine();

        if (!AnsiConsole.Confirm($"Remove [cyan]{Markup.Escape(selectedAlias.Name)}[/]?"))
        {
            AnsiConsole.MarkupLine("[grey]Cancelled.[/]");
            return;
        }

        AliasManager.RemoveAlias(selectedAlias.Name);
        AnsiConsole.MarkupLine($"[green]Removed [cyan]{Markup.Escape(selectedAlias.Name)}[/][/]");
    }

    private static void HandleList()
    {
        List<Alias> aliases = AliasManager.GetAllAliases();

        if (aliases.Count == 0)
        {
            AnsiConsole.MarkupLine("[yellow]No devices registered yet.[/]");
            return;
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
    }
}