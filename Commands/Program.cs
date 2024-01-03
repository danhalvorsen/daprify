using Microsoft.Extensions.DependencyInjection;
using System.CommandLine;
using Microsoft.Extensions.Hosting;

namespace Daprify
{
    public static class Program
    {
        static void Main(string[] args)
        {
            IHost host = DaprifyHost.CreateHostBuilder(args).Build() ?? throw new ArgumentNullException(nameof(args));

            IEnumerable<Command> commands = host.Services.GetServices<Command>();
            RootCommand rootCommand = new("CLI for creating configuration files for Dapr services.");
            commands.ToList().ForEach(rootCommand.AddCommand);

            rootCommand.Invoke(args);
        }
    }
}
