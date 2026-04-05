using System;
using System.Linq;
using Spectre.Console;

namespace erwachen;

internal static class Program
{
    public const string Version = "2.0.0";

    private static int Main(string[] args)
    {
        AnsiConsole.Profile.Capabilities.ColorSystem = ColorSystem.Standard;

        bool noInteractive = args.Contains("--no-interactive") || args.Contains("-n");
        string[] strippedArgs = args.Where(arg => arg != "--no-interactive" && arg != "-n").ToArray();

        if (strippedArgs.Length == 0 && !noInteractive)
        {
            InteractiveShell.RunInteractive();
        }

        return 0;
    }
}