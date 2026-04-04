using System;
using System.IO;

namespace erwachen.Core;

public static class AppPaths
{
    private static readonly string ConfigDirectory;
    internal static readonly string AliasConfigPath;


    static AppPaths()
    {
        string configDir = Environment.GetEnvironmentVariable("XDG_CONFIG_HOME")
                           ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".config");
        
        ConfigDirectory = Path.Combine(configDir, "erwachen");
        Directory.CreateDirectory(Path.GetDirectoryName(ConfigDirectory)!);
        
        AliasConfigPath = Path.Combine(ConfigDirectory, "aliasList.toml");
    }
}
