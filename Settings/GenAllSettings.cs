using System.CommandLine;

namespace Daprify.Settings
{
    public class GenAllSettings : ISettings
    {
        public static List<string>? AvailableSettings => ["https", "logging", "metric", "middleware", "mtls", "tracing"];
        public static List<string>? AvailableComponents => ["bindings", "configstore", "crypto", "lock", "pubsub", "secretstore", "statestore"];
        public static readonly List<string> DefaultValue = ["statestore", "secretstore"];

        public string CommandName => "gen_all";
        public string CommandDescription => "Generates everything needed to use Dapr with docker-compose (components, config, certificates and docker-compose file(s))" +
                                            "\nEasiest to specify all your project needs in at CLI/Commands/config.json and execute with --config config.json" +
                                            "\nIf all your services has a PackageReference to Dapr use --solution_paths instead of --services." +
                                            "\nUse dotnet run gen_all -- -help to see all available options.";
        public string CommandExample => "\n\nExamples:\n  dotnet run gen_all --config config.json \n  dotnet run gen_all --components rabbitmq redis --services ServiceA ServiceB --settings mtls tracing --solution_paths ../../Backend";

        public static string[] ComponentOptionName { get; set; } = ["--c", "--components"];
        public static string ComponentOptionDescription { get; set; } = "The specific component(s) to generate docker-compose content to.";
        public static string[] ConfigOptionName { get; set; } = ["--config"];
        public static string ConfigOptionDescription { get; set; } = "Path to your config file (from executing path).";
        public static string[] ProjectOptionName { get; set; } = ["--pp", "--project_path"];
        public static string ProjectOptionDescription { get; set; } = "The path to the root of your project (from executing path). Not needed if it's a git project. Used to find correct paths.";
        public static string[] SettingOptionName { get; set; } = ["--s", "--settings"];
        public static string SettingOptionDescription { get; set; } = "Additional settings for your services.";
        public static string[] ServiceOptionName { get; set; } = ["--se", "--services"];
        public static string ServiceOptionDescription { get; set; } = "The specific service(s) to generate docker-compose content to.";
        public static string[] SolutionOptionName { get; set; } = ["--sp", "--solution_paths"];
        public static string SolutionOptionDescription { get; set; } = "Path to your .Net sln file (from executing path). Adds all projects dependent on Dapr to docker-compose.";

        private static readonly Option<List<string>> ComponentOption = new(ComponentOptionName, () => DefaultValue, ComponentOptionDescription) { AllowMultipleArgumentsPerToken = true };
        private static readonly Option<List<string>> ConfigOption = new(ConfigOptionName, ConfigOptionDescription);
        private static readonly Option<List<string>> ProjectOption = new(ProjectOptionName, ProjectOptionDescription);
        private static readonly Option<List<string>> ServiceOption = new(ServiceOptionName, ServiceOptionDescription) { AllowMultipleArgumentsPerToken = true };
        private static readonly Option<List<string>> SettingOption = new(SettingOptionName, SettingOptionDescription) { AllowMultipleArgumentsPerToken = true };
        private static readonly Option<List<string>> SolutionOption = new(SolutionOptionName, SolutionOptionDescription) { AllowMultipleArgumentsPerToken = true };
        public List<Option<List<string>>> Options { get; set; } = [ConfigOption, ComponentOption, ProjectOption, ServiceOption, SettingOption, SolutionOption];

        public GenAllSettings()
        {
            SettingOption.FromAmong([.. AvailableSettings]);
            ComponentOption.FromAmong([.. AvailableComponents]);
        }
    }
}