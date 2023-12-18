using Microsoft.Extensions.Hosting;

namespace CLI
{
    public static class CLIHost
    {
        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var builder = Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.ScanAssemblies();
                    services.AddCommands();

                });
            return builder;
        }
    }
}