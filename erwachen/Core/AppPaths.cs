using System;
using System.IO;

namespace erwachen.Core;

public static class AppPaths
{
    private static readonly string ConfigDirectory;
    public static readonly string AliasesPath;

    static AppPaths()
    {
        string configDir = Environment.GetEnvironmentVariable("XDG_CONFIG_HOME")
                           ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".config");

        ConfigDirectory = Path.Combine(configDir, "erwachen");
        Directory.CreateDirectory(ConfigDirectory);

        AliasesPath = Path.Combine(ConfigDirectory, "aliases.toml");
    }
}