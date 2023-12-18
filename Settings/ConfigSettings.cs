using System.CommandLine;

namespace CLI.Settings
{
    public class ConfigSettings : ISettings
    {
        public static List<string>? AvailableSettings => ["logging", "metric", "middleware", "mtls", "tracing"];

        public string CommandName => "gen_config";
        public string CommandDescription => "Generates configuration file used by every Dapr service";
        public string CommandExample => "\n\nExamples:\n  dotnet run gen_config\n  dotnet run gen_config --settings mtls tracing";

        public static string[] OptionName { get; set; } = ["--s", "--settings"];
        public static string OptionDescription { get; set; } = "The specific setting(s) to add to the configuration";

        private static readonly Option<List<string>> SettingOption = new(OptionName, OptionDescription) { AllowMultipleArgumentsPerToken = true };
        public List<Option<List<string>>> Options { get; set; } = [SettingOption];

        public ConfigSettings()
        {
            SettingOption.FromAmong([.. AvailableSettings]);
        }
    }
}