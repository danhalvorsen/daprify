using System.CommandLine;

namespace Daprify.Settings
{
    public class DockerfileSettings : ISettings
    {
        public string CommandName => "gen_dockerfiles";
        public string CommandDescription => "Generates dockerfiles for all your projects in the specified solutions.";
        public string CommandExample => "\n\nExamples:\n  dotnet run gen_dockerfiles --solution_paths ../../Service1 ../../Service2";

        public static string[] ProjectOptionName { get; set; } = ["--pp", "--project_path"];
        public static string ProjectOptionDescription { get; set; } = "The path to the root of your project (from executing path). Not needed if it's a git project. Used to find correct paths.";
        public static string[] ServiceOptionName { get; set; } = ["--se", "--services"];
        public static string ServiceOptionDescription { get; set; } = "The specific service(s) to generate docker-compose content to.";
        public static string[] SolutionOptionName { get; set; } = ["--sp", "--solution_paths"];
        public static string SolutionOptionDescription { get; set; } = "The path to your .Net solution file (from executing path). Used to generate dockerfile for all projects in the sln file.";

        private static readonly Option<List<string>> ProjectOption = new(ProjectOptionName, ProjectOptionDescription);
        private static readonly Option<List<string>> ServiceOption = new(ServiceOptionName, ServiceOptionDescription) { AllowMultipleArgumentsPerToken = true };
        private static readonly Option<List<string>> SolutionOption = new(SolutionOptionName, SolutionOptionDescription) { AllowMultipleArgumentsPerToken = true };
        public List<Option<List<string>>> Options { get; set; } = [ProjectOption, ServiceOption, SolutionOption];
    }
}