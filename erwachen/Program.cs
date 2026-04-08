using System.Threading;
using erwachen.Commands;
using Spectre.Console;
using Spectre.Console.Cli;
using Spectre.Console.Cli.Help;

namespace erwachen;

public static class Program
{
    public const string Version = "2.0.0";

    private static int Main(string[] args)
    {
        AnsiConsole.Profile.Capabilities.ColorSystem = ColorSystem.Standard;

        CommandApp erwachen = new();

        erwachen.Configure(config =>
        {
            config.Settings.HelpProviderStyles = new HelpProviderStyle
            {
                Description = new DescriptionStyle
                {
                    Header = "fuchsia",
                },
                Usage = new UsageStyle
                {
                    Header = "fuchsia",
                    CurrentCommand = "underline",
                    Command = "aqua",
                    Options = "grey",
                    RequiredArgument = "aqua",
                    OptionalArgument = "silver",
                },
                Examples = new ExampleStyle
                {
                    Header = "fuchsia",
                    Arguments = "grey",
                },
                Arguments = new ArgumentStyle
                {
                    Header = "fuchsia",
                    RequiredArgument = "silver",
                    OptionalArgument = "silver",
                },
                Commands = new CommandStyle
                {
                    Header = "fuchsia",
                    ChildCommand = "silver",
                    RequiredArgument = "silver",
                },
                Options = new OptionStyle
                {
                    Header = "fuchsia",
                    DefaultValueHeader = "lime",
                    DefaultValue = "bold",
                    RequiredOptionValue = "silver",
                    OptionalOptionValue = "grey",
                },
            };

            config.SetApplicationName("erwachen");
            config.SetApplicationVersion(Version);

            config.AddCommand<WakeCommand>("wake")
                .WithDescription("Send a magic packet to a registered or arbitrary device");

            config.AddCommand<AddAliasCommand>("add")
                .WithDescription("Register a new device");

            config.AddCommand<RemoveAliasCommand>("remove")
                .WithDescription("Remove a registered device");

            config.AddCommand<ListAliasCommand>("list")
                .WithDescription("Show all registered devices");
            
            config.AddCommand<InteractiveInterfaceCommand>("interactive")
                .WithDescription("Launch the interactive terminal interface");
        });

        return erwachen.Run(args);
    }

    private sealed class InteractiveInterfaceCommand : Command
    {
        public override int Execute(CommandContext context, CancellationToken cancellationToken) =>
            InteractiveInterface.Run();
    }
}