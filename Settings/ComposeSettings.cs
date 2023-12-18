using System.CommandLine;

namespace CLI.Settings
{
    public class ComposeSettings : ISettings
    {
        public static List<string> AvailableComponents => ["dashboard", "placement", "rabbitmq", "redis", "sentry", "zipkin"];
        public static List<string> AvailableSettings => ["https", "mtls"];
        public static readonly List<string> DefaultValue = ["dashboard", "placement", "zipkin"];

        public string CommandName => "gen_compose";
        public string CommandDescription => "Generates docker-compose.yml from your Dapr directory or for the specified service(s).";
        public string CommandExample => "\n\nExamples:\n  dotnet run gen_compose --components rabbitmq sentry\n  dotnet run gen_compose --settings mtls https --services ServiceA\n  dotnet run gen_compose --solution_path ../../Backend\n  dotnet run gen_compose --components rabbitmq sentry --services Frontend Backend --solution_path ../../Backend";

        public static string[] SettingOptionName { get; set; } = ["--s", "--settings"];
        public static string SettingOptionDescription { get; set; } = "Additional settings for your services.";
        public static string[] ServiceOptionName { get; set; } = ["--se", "--services"];
        public static string ServiceOptionDescription { get; set; } = "The specific service(s) to generate docker-compose content to.";
        public static string[] ComponentOptionName { get; set; } = ["--c", "--components"];
        public static string ComponentOptionDescription { get; set; } = "The specific component(s) to generate docker-compose content to.";
        public static string[] SolutionOptionName { get; set; } = ["--so", "--solution_path"];
        public static string SolutionOptionDescription { get; set; } = "The path to your .Net solution file (from executing path). Used to generate certificates for all services dependent on Dapr.";

        private static readonly Option<List<string>> ComponentOption = new(ComponentOptionName, () => DefaultValue, ComponentOptionDescription) { AllowMultipleArgumentsPerToken = true };
        private static readonly Option<List<string>> SettingOption = new(SettingOptionName, SettingOptionDescription) { AllowMultipleArgumentsPerToken = true };
        private static readonly Option<List<string>> ServiceOption = new(ServiceOptionName, ServiceOptionDescription) { AllowMultipleArgumentsPerToken = true };
        private static readonly Option<List<string>> SolutionOption = new(SolutionOptionName, SolutionOptionDescription);
        public List<Option<List<string>>> Options { get; set; } = [ComponentOption, SettingOption, ServiceOption, SolutionOption];

        public ComposeSettings()
        {
            ComponentOption.FromAmong([.. AvailableComponents]);
        }
    }
}