using System.Xml.Linq;
using Microsoft.Build.Construction;

namespace CLI.Services
{
    public class SolutionService()
    {
        private const string SLN_EXT = "*.sln";

        public static void GetServicesFromSolution(ref List<string> services, List<string> solutionPaths)
        {
            if (solutionPaths.Count > 0)
            {
                services = FindDaprDep(solutionPaths);
            }
        }

        public static List<string> FindDaprDep(List<string> solutionPaths)
        {
            List<string> services = [];
            DirectoryInfo currentDir = new(Directory.GetCurrentDirectory());

            foreach (string path in solutionPaths)
            {
                string solutionDir = Path.Combine(currentDir.FullName, path);
                string? slnPath = DirectoryService.GetFileInDirectory(solutionDir, SLN_EXT);
                if (slnPath == null || !File.Exists(slnPath))
                {
                    throw new FileNotFoundException($"Could not find the solution file: {slnPath}");
                }

                List<string> projectPaths = GetProjectPaths(slnPath);
                services.AddRange(GetDaprServices(projectPaths));
            }

            CheckServicesList(services);

            return services;
        }

        private static void CheckServicesList(List<string> services)
        {
            if (services.Count == 0)
            {
                Console.WriteLine("No Dapr services found in the solution! \nUse the --services option to specify the services to generate certificates for.");
            }
        }

        private static List<string> GetProjectPaths(string slnPath)
        {
            List<string> projectPaths = [];
            SolutionFile slnFile = SolutionFile.Parse(slnPath);

            foreach (ProjectInSolution project in slnFile.ProjectsInOrder)
            {
                // Only add projects that are not test projects
                if (project.ProjectType == SolutionProjectType.KnownToBeMSBuildFormat && !project.ProjectName.Contains("Test", StringComparison.OrdinalIgnoreCase))
                {
                    projectPaths.Add(project.AbsolutePath);
                }
            }

            return projectPaths;
        }

        private static List<string> GetDaprServices(List<string> projectPaths)
        {
            List<string> daprServices = [];

            foreach (string projectPath in projectPaths)
            {
                if (!File.Exists(projectPath))
                {
                    throw new FileNotFoundException($"Error occurred while trying to find the project file: '{projectPath}'");
                }

                XDocument project = XDocument.Load(projectPath);

                if (CheckDaprDependency(project))
                {
                    string projectName = Path.GetFileNameWithoutExtension(projectPath);
                    daprServices.Add(projectName);
                }
            }

            return daprServices;
        }

        private static bool CheckDaprDependency(XDocument project)
        {
            return project
                    .Descendants("PackageReference")
                    .Any(pr => pr.Attribute("Include")?.Value
                    .Contains("Dapr", StringComparison.OrdinalIgnoreCase) ?? false);
        }
    }
}