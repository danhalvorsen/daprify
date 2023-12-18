using System.CommandLine;

namespace CLI.Settings
{
    public class ComponentSettings : ISettings
    {
        public static List<string>? AvailableComponents => ["bindings", "configstore", "crypto", "lock", "pubsub", "secretstore", "statestore"];
        public static List<string> DefaultValue { get; set; } = ["pubsub", "statestore"];

        public string CommandName => "gen_components";
        public string CommandDescription => "Generates the desired Dapr components YAML files.";
        public string CommandExample => "\n\nExamples:\n  dotnet run gen_components --components statestore\n  dotnet run gen_components --components statestore pubsub";

        public static string[] OptionName { get; set; } = ["--c", "--components"];
        public static string OptionDescription { get; set; } = "The specific component(s) to generate config files for.";

        private static readonly Option<List<string>> ComponentOption = new(OptionName, () => DefaultValue, OptionDescription) { AllowMultipleArgumentsPerToken = true };
        public List<Option<List<string>>> Options { get; set; } = [ComponentOption];

        public ComponentSettings()
        {
            ComponentOption.FromAmong([.. AvailableComponents]);
        }
    }
}