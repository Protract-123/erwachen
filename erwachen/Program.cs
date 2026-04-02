using Spectre.Console.Cli;

namespace erwachen;
    internal static class Program
    {
        private const string Version = "1.0.0";

        private static int Main(string[] args)
        {
            CommandApp erwachen = new();
            
            erwachen.Configure(config =>
            {
                config.SetApplicationName("erwachen");
                config.SetApplicationVersion(Version);
            });
            
            return erwachen.Run(args);
        }
    }
