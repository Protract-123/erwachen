using System;
using System.IO;

namespace erwachen.Core;

public static class AppPaths
{
    internal static readonly string ConfigDirectory;

    static AppPaths()
    {
        string configDir = Environment.GetEnvironmentVariable("XDG_CONFIG_HOME")                                                                                                                                                                
                           ?? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".config");                                                                                                                                       
        ConfigDirectory = Path.Combine(configDir, "erwachen");
    }
}
