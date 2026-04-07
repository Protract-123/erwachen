using System;
using System.IO;

namespace erwachen.Core;

public static class AppPaths
{
    public static readonly string AliasesPath;

    static AppPaths()
    {
        string userConfigDirectory = Environment.GetEnvironmentVariable("XDG_CONFIG_HOME")
                                     ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                                         ".config");

        string configDirectory = Path.Combine(userConfigDirectory, "erwachen");
        Directory.CreateDirectory(configDirectory);

        AliasesPath = Path.Combine(configDirectory, "aliases.toml");
    }
}