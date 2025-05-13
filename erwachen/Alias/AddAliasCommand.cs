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
public class AddAliasCommand : Command<AddAliasCommand.Settings>
{
    [UsedImplicitly]
    public sealed class Settings : CommandSettings
    {
        [Description("Name of alias")]
        [CommandArgument(0, "<alias>")]
        public string? AliasName { get; init; }
        
        [Description("Hardware address to alias")]
        [CommandArgument(1, "<hardwareAddress>")]
        public string? HardwareAddress { get; init; }
    }

    public override int Execute(CommandContext context, Settings settings)
    {
        string aliasConfigPath = Path.Combine(AppPaths.WriteDirectory, "alias.json");

        if (!FormatCheckers.IsValidHardwareAddress(settings.HardwareAddress!)) 
            throw new SystemException("This is an invalid hardware address!");
        
        if (File.Exists(aliasConfigPath))
        { 
            Dictionary<string, string>? aliases = JsonSerializer.Deserialize<Dictionary<string, string>>
                (File.ReadAllText(aliasConfigPath));

            if (aliases!.TryGetValue(settings.AliasName!, out _))
                throw new SystemException("An alias with this name already exists!");
            
            aliases!.Add(settings.AliasName!, settings.HardwareAddress!);
            
            string data = JsonSerializer.Serialize(aliases, new JsonSerializerOptions{WriteIndented = true});
            
            File.WriteAllText(aliasConfigPath,data);
        }
        else
        {
            File.Create(aliasConfigPath).Close();
            Dictionary<string, string> aliases = new()
            {
                [settings.AliasName!] = settings.HardwareAddress!,
            };
            
            string data = JsonSerializer.Serialize(aliases, new JsonSerializerOptions{WriteIndented = true});
            
            File.WriteAllText(aliasConfigPath,data);
        }

        return 0;
    }
}