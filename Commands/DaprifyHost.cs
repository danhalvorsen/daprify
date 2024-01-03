using Microsoft.Extensions.Hosting;

namespace Daprify
{
    public static class DaprifyHost
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