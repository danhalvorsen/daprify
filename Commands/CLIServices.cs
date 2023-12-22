using CLI.Commands;
using CLI.Services;
using CLI.Settings;
using CLI.Validation;
using Microsoft.Extensions.DependencyInjection;
using System.CommandLine;
using System.Reflection;

namespace CLI
{
    public static class CLIServices
    {

        // TODO: Add the commands to implement Command and not AsSelf()
        public static void ScanAssemblies(this IServiceCollection services)
        {
            string[] projectAssemblyNames = ["Commands", "Services", "Settings", "Templates", "Validators"];

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
            services.AddTransient<Command, CLICommand<GenAllService, GenAllSettings, GenAllValidator>>();
            services.AddTransient<Command, CLICommand<CertificateService, CertificateSettings, CertificateValidator>>();
            services.AddTransient<Command, CLICommand<ConfigService, ConfigSettings, ConfigValidator>>();
            services.AddTransient<Command, CLICommand<ComponentService, ComponentSettings, ComponentValidator>>();
            services.AddTransient<Command, CLICommand<DockerfileService, DockerfileSettings, DockerfileValidator>>();
            services.AddTransient<Command, CLICommand<ComposeService, ComposeSettings, ComposeValidator>>();
        }
    }
}