using erwachen.Alias;
using JetBrains.Annotations;
using Spectre.Console.Cli;

namespace erwachen
{
    [UsedImplicitly]
    internal class Program
    {
        private const string Version = "0.1.0";

        private static int Main(string[] args)
        {
            CommandApp erwachen = new();
            
            erwachen.Configure(config =>
            {
                config.SetApplicationName("erwachen");
                config.SetApplicationVersion(Version);

                config.AddBranch("alias", alias =>
                {
                    alias.AddCommand<AddAliasCommand>("add")
                        .WithDescription("Add a new hardware address alias")
                        .WithExample("alias add", "<alias>", "<hardwareAddress>");

                    alias.AddCommand<RemoveAliasCommand>("remove")
                        .WithDescription("Remove a hardware address alias")
                        .WithExample("alias remove", "<alias>");
                    
                    alias.AddCommand<ListAliasCommand>("list")
                        .WithDescription("List all available hardware address aliases")
                        .WithExample("alias list");
                });
            });
            
            return erwachen.Run(args);
        }
    }
}