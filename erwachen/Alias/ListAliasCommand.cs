using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using erwachen.Utils;
using JetBrains.Annotations;
using Spectre.Console;
using Spectre.Console.Cli;

namespace erwachen.Alias;

[UsedImplicitly]
public class ListAliasCommand : Command<ListAliasCommand.Settings>
{
    [UsedImplicitly]
    public sealed class Settings : CommandSettings;

    public override int Execute(CommandContext context, Settings settings)
    {
        string aliasConfigPath = Path.Combine(AppPaths.WriteDirectory, "alias.json");

        Table aliasTable = new();
        aliasTable.AddColumn(new TableColumn("Alias").Centered());
        aliasTable.AddColumn(new TableColumn("Hardware Address").Centered());

        if (!File.Exists(aliasConfigPath))
        {
            throw new SystemException("Could not find alias config file");
        }

        Dictionary<string, string>? aliases = JsonSerializer.Deserialize<Dictionary<string, string>>(File.ReadAllText(aliasConfigPath));
        foreach (KeyValuePair<string, string> alias in aliases!)
        {
            aliasTable.AddRow(alias.Key, alias.Value);
        }
        AnsiConsole.Write(aliasTable);
        
        return 0;
    }
}