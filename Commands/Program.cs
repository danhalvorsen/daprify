using Microsoft.Extensions.DependencyInjection;
using System.CommandLine;
using Microsoft.Extensions.Hosting;
using Spectre.Console;
using Serilog;

namespace Daprify
{
    public static class Program
    {
        static void Main(string[] args)
        {

            IHost host = DaprifyHost.CreateHostBuilder(args)
                                    .UseSerilog()
                                    .Build() ?? throw new ArgumentNullException(nameof(args));

            IEnumerable<Command> commands = host.Services.GetServices<Command>();
            RootCommand rootCommand = new("CLI for creating configuration files for Dapr services.");
            rootCommand.AddGlobalOption(new Option<bool>("--verbose", "Enable verbose output"));

            commands.ToList().ForEach(rootCommand.AddCommand);

            AnsiConsole.Write(new FigletText("DAPRIFY").Color(Color.Green));
            rootCommand.Invoke(args);
        }
    }
}
