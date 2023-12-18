using System.CommandLine;

namespace CLI.Settings
{
    public class CompileToPumlSettings : ISettings
    {
        public string CommandName => "gen_puml";
        public string CommandDescription => "Generates a plantuml diagram of the supplied micro service spec.";
        public string CommandExample => "\n\nExample:\n  dotnet run gen_puml\n  dotnet run gen_puml -i test.mss -o out.plantuml";

        public static string[] InputOptionName { get; } = ["-i", "--in"];
        public static string InputOptionDescription { get; } = "The input file name.";
        public static string[] OutputOptionName { get; } = ["-o", "--out"];
        public static string OutputOptionDescription { get; } = "The output file name.";

        public static readonly Option<List<string>> InputOption = new(InputOptionName, () => [], InputOptionDescription);
        public static readonly Option<List<string>> OutputOption = new(OutputOptionName, () => [], OutputOptionDescription);
        public List<Option<List<string>>> Options { get; set; } = [InputOption, OutputOption];
    }
}