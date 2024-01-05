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
            string description = "CLI for creating all the necessary files for your Dapr project." +
                                 "\nThis includes generating certificates, config, components, dockerfiles and docker-compose file(s)" +
                                 "\nPrefer to use gen_all command and specify options in Daprify/Commands/config.json";
            RootCommand rootCommand = new(description);

            commands.ToList().ForEach(rootCommand.AddCommand);

            AnsiConsole.Write(new FigletText("DAPRIFY").Color(Color.Green));
            rootCommand.Invoke(args);
        }
    }
}
