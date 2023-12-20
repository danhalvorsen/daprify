using System.Xml.Linq;
using Microsoft.Build.Construction;

namespace CLI.Services
{
    public class SolutionService()
    {
        private const string SLN_EXT = "*.sln";

        public static List<string> GetDaprServicesFromSln(ref List<string> services, List<string> solutionPaths)
        {
            List<string> projectPaths = [];
            if (solutionPaths.Count > 0)
            {
                List<string> slnProjects = FindProjects(solutionPaths);
                List<string> daprProjects = GetDaprServices(slnProjects);
                CheckDaprProjects(daprProjects);

                AddToList(services, projectPaths, slnProjects, daprProjects);
            }
            return projectPaths;
        }

        private static void AddToList(List<string> services, List<string> projectPaths, List<string> projects, List<string> daprProjects)
        {
            services.AddRange(daprProjects);
            projectPaths.AddRange(projects.Where(project => daprProjects.Any(daprProject => project.Contains(daprProject))));
        }


        public static List<string> FindProjects(List<string> solutionPaths)
        {
            List<string> projectPaths = [];
            DirectoryInfo currentDir = new(Directory.GetCurrentDirectory());

            foreach (string path in solutionPaths)
            {
                string solutionDir = Path.Combine(currentDir.FullName, path);
                string? slnPath = DirectoryService.GetFileInDirectory(solutionDir, SLN_EXT);
                if (slnPath == null || !File.Exists(slnPath))
                {
                    throw new FileNotFoundException($"Could not find the solution file: {slnPath}");
                }

                projectPaths.AddRange(GetProjectPaths(slnPath));
            }

            return projectPaths;
        }

        private static void CheckDaprProjects(List<string> daprProjects)
        {
            if (daprProjects.Count == 0)
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
                    string relPath = Path.GetRelativePath(Directory.GetCurrentDirectory(), project.AbsolutePath);
                    projectPaths.Add(relPath);
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

                if (CheckDependency(project, "Dapr"))
                {
                    AddProjNameToList(daprServices, projectPath);
                }
            }

            return daprServices;
        }

        private static bool CheckDependency(XDocument project, string dependency)
        {
            return project
                    .Descendants("PackageReference")
                    .Any(pr => pr.Attribute("Include")?.Value
                    .Contains(dependency, StringComparison.OrdinalIgnoreCase) ?? false);
        }

        private static void AddProjNameToList(List<string> services, string projectPath)
        {
            string projectName = Path.GetFileNameWithoutExtension(projectPath);
            services.Add(projectName);
        }
    }
}