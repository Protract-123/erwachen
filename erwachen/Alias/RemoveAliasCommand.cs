using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text.Json;
using erwachen.Utils;
using JetBrains.Annotations;
using Spectre.Console.Cli;

namespace erwachen.Alias;

[UsedImplicitly]
public class RemoveAliasCommand : Command<RemoveAliasCommand.Settings>
{
    [UsedImplicitly]
    public sealed class Settings : CommandSettings
    {
        [Description("Name of alias")]
        [CommandArgument(0, "<alias>")]
        public string? AliasName { get; init; }
    }

    public override int Execute(CommandContext context, Settings settings)
    {
        string aliasConfigPath = Path.Combine(AppPaths.WriteDirectory, "alias.json");
        
        if (File.Exists(aliasConfigPath))
        { 
            Dictionary<string, string>? aliases = JsonSerializer.Deserialize<Dictionary<string, string>>
                (File.ReadAllText(aliasConfigPath));

            if (!aliases!.TryGetValue(settings.AliasName!, out _))
                throw new SystemException("An alias with this name does not exist!");
            
            aliases!.Remove(settings.AliasName!);
            
            string data = JsonSerializer.Serialize(aliases, new JsonSerializerOptions{WriteIndented = true});
            
            File.WriteAllText(aliasConfigPath,data);
        }
        else throw new SystemException("Could not find alias config file");

        return 0;
    }
}