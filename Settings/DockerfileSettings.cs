using System.CommandLine;

namespace CLI.Settings
{
    public class DockerfileSettings : ISettings
    {
        public string CommandName => "gen_dockerfiles";
        public string CommandDescription => "Generates dockerfiles for all your projects in the specified solutions.";
        public string CommandExample => "\n\nExamples:\n  dotnet run gen_dockerfiles --solution_path ../../Service1 ../../Service2";

        public static string[] SolutionOptionName { get; set; } = ["--so", "--solution_path"];
        public static string SolutionOptionDescription { get; set; } = "The path to your .Net solution file (from executing path). Used to generate dockerfile for all projects in the sln file.";

        private static readonly Option<List<string>> SolutionOption = new(SolutionOptionName, SolutionOptionDescription);
        public List<Option<List<string>>> Options { get; set; } = [SolutionOption];
    }
}