using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Tomlyn;

namespace erwachen.Core;

public sealed record Alias(string Name, string MacAddress);

public static class AliasManager
{
    private static readonly string AliasConfigPath = Path.Combine(AppPaths.ConfigDirectory, "aliasList.toml");
    private static readonly TomlSerializerOptions SerializerOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };


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
        if (!File.Exists(AliasConfigPath))
            return [];

        string toml = File.ReadAllText(AliasConfigPath);
        return TomlSerializer.Deserialize<List<Alias>>(toml, SerializerOptions) ?? [];
    }

    private static void WriteAliases(List<Alias> aliases)
    {
        string toml = TomlSerializer.Serialize(aliases, SerializerOptions);
        File.WriteAllText(AliasConfigPath, toml);
    }
}
