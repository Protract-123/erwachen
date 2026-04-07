using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Tomlyn;
using Tomlyn.Model;

namespace erwachen.Core;

public sealed record Alias(string Name, string MacAddress);

public static class AliasManager
{
    public static void AddAlias(Alias alias)
    {
        if (!FormatCheckers.IsValidMacAddress(alias.MacAddress))
            throw new ArgumentException("Invalid MAC address");

        List<Alias> aliases = ReadAliases();

        if (aliases.Exists(existingAlias => existingAlias.Name == alias.Name))
            throw new InvalidOperationException("An alias with this name already exists");

        aliases.Add(alias);
        WriteAliases(aliases);
    }

    public static bool TryGetMacFromAlias(string aliasName, out string macAddress)
    {
        List<Alias> aliases = ReadAliases();
        Alias? match = aliases.Find(alias => alias.Name == aliasName);

        if (match is null)
        {
            macAddress = string.Empty;
            return false;
        }

        macAddress = match.MacAddress;
        return true;
    }

    public static List<Alias> GetAllAliases() => ReadAliases();

    public static void RemoveAlias(string aliasName)
    {
        List<Alias> aliases = ReadAliases();
        int index = aliases.FindIndex(alias => alias.Name == aliasName);

        if (index == -1)
            throw new InvalidOperationException($"No alias found with name '{aliasName}'");

        aliases.RemoveAt(index);
        WriteAliases(aliases);
    }

    private static List<Alias> ReadAliases()
    {
        if (!File.Exists(AppPaths.AliasesPath))
            return [];

        string toml = File.ReadAllText(AppPaths.AliasesPath);
        TomlTable tomlDocument = TomlSerializer.Deserialize<TomlTable>(toml)!;

        if (!tomlDocument.TryGetValue("aliases", out object aliasesValue) ||
            aliasesValue is not TomlTableArray aliasTableArray)
            return [];

        return aliasTableArray
            .Select(aliasTable => new Alias((string)aliasTable["name"], (string)aliasTable["macAddress"]))
            .ToList();
    }

    private static void WriteAliases(List<Alias> aliases)
    {
        TomlTable tomlDocument = new();
        TomlTableArray aliasTableArray = [];

        foreach (Alias alias in aliases)
        {
            TomlTable aliasTable = new()
            {
                ["name"] = alias.Name,
                ["macAddress"] = alias.MacAddress
            };
            aliasTableArray.Add(aliasTable);
        }

        tomlDocument["aliases"] = aliasTableArray;

        string toml = TomlSerializer.Serialize(tomlDocument);
        File.WriteAllText(AppPaths.AliasesPath, toml);
    }
}