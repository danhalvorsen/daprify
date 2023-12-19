using System.CommandLine;

namespace CLI.Settings
{
    public class MssBuilderSettings : ISettings
    {
        public string CommandName => "gen_mss";
        public string CommandDescription => "Generates a solution from the supplied micro service spec.";
        public string CommandExample => "\n\nExamples:\n  dotnet run gen_mss -i test.mss\n  dotnet run gen_mss -i test.mss -o ./out";

        public static string[] InputOptionName { get; } = ["-i", "--in"];
        public static string InputOptionDescription { get; } = "The input file name.";
        public static string[] OutputOptionName { get; } = ["-o", "--out"];
        public static string OutputOptionDescription { get; } = "The output path.";

        public static readonly Option<List<string>> InputOption = new(InputOptionName, () => [], InputOptionDescription);
        public static readonly Option<List<string>> OutputOption = new(OutputOptionName, () => [], OutputOptionDescription);
        public List<Option<List<string>>> Options { get; set; } = [InputOption, OutputOption];
    }
}