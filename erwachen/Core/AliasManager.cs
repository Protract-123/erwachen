using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Tomlyn;
using Tomlyn.Model;

namespace erwachen.Core;

public sealed record Alias(string Name, string MacAddress);

public static class AliasManager
{
    private static readonly TomlSerializerOptions SerializerOptions = new()
        { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public static void AddAlias(Alias alias)
    {
        if (!FormatCheckers.IsValidHardwareAddress(alias.MacAddress))
            throw new ArgumentException("Invalid hardware address");

        List<Alias> aliases = ReadAliases();

        if (aliases.Exists(a => a.Name == alias.Name))
            throw new InvalidOperationException("An alias with this name already exists");

        aliases.Add(alias);
        WriteAliases(aliases);
    }

    public static string GetMacFromAlias(string aliasName)
    {
        List<Alias> aliases = ReadAliases();
        Alias? match = aliases.Find(a => a.Name == aliasName);

        if (match is null)
            throw new InvalidOperationException($"No alias found with name '{aliasName}'");

        return match.MacAddress;
    }

    public static List<Alias> GetAllAliases()
    {
        return ReadAliases();
    }

    public static void RemoveAlias(string aliasName)
    {
        List<Alias> aliases = ReadAliases();
        int index = aliases.FindIndex(a => a.Name == aliasName);

        if (index == -1)
            throw new InvalidOperationException($"No alias found with name '{aliasName}'");

        aliases.RemoveAt(index);
        WriteAliases(aliases);
    }

    private static List<Alias> ReadAliases()
    {
        if (!File.Exists(AppPaths.AliasConfigPath))
            return [];

        string toml = File.ReadAllText(AppPaths.AliasConfigPath);
        TomlTable doc = TomlSerializer.Deserialize<TomlTable>(toml)!;

        if (!doc.TryGetValue("aliases", out object value) || value is not TomlTableArray tableArray)
            return [];

        return tableArray
            .Select(t => new Alias((string)t["name"], (string)t["macAddress"]))
            .ToList();    
    }

    private static void WriteAliases(List<Alias> aliases)
    {
        TomlTable doc = new();
        TomlTableArray tableArray = [];

        foreach (Alias alias in aliases)
        {
            TomlTable table = new()
            {
                ["name"] = alias.Name,
                ["macAddress"] = alias.MacAddress
            };
            tableArray.Add(table);
        }

        doc["aliases"] = tableArray;

        string toml = TomlSerializer.Serialize(doc, SerializerOptions);
        File.WriteAllText(AppPaths.AliasConfigPath, toml);
    }
}