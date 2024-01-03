using Daprify.Commands;
using Daprify.Services;
using Daprify.Settings;
using Microsoft.Extensions.DependencyInjection;
using System.CommandLine;
using System.Reflection;

namespace Daprify
{
    public static class DaprifyServices
    {

        // TODO: Add the commands to implement Command and not AsSelf()
        public static void ScanAssemblies(this IServiceCollection services)
        {
            string[] projectAssemblyNames = ["Commands", "Models", "Services", "Settings", "Templates", "Validators"];

            Assembly[] loadedAssemblies = projectAssemblyNames
                .Select(Assembly.Load)
                .Where(assembly => assembly != null)
                .ToArray();

            _ = services.Scan(scan => scan
                .FromAssemblies(loadedAssemblies)
                .AddClasses()
                .AsSelf()
                .WithTransientLifetime()

                .AddClasses()
                .AsImplementedInterfaces()
                .WithTransientLifetime());
        }

        public static void AddCommands(this IServiceCollection services)
        {
            services.AddTransient<Command, DaprifyCommand<GenAllService, GenAllSettings>>();
            services.AddTransient<Command, DaprifyCommand<CertificateService, CertificateSettings>>();
            services.AddTransient<Command, DaprifyCommand<ConfigService, ConfigSettings>>();
            services.AddTransient<Command, DaprifyCommand<ComponentService, ComponentSettings>>();
            services.AddTransient<Command, DaprifyCommand<DockerfileService, DockerfileSettings>>();
            services.AddTransient<Command, DaprifyCommand<ComposeService, ComposeSettings>>();
        }
    }
}