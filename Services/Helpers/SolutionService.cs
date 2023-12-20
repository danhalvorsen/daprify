using System.Xml.Linq;
using Microsoft.Build.Construction;

namespace CLI.Services
{
    public class SolutionService()
    {
        private const string SLN_EXT = "*.sln";
        private const string DAPR = "Dapr";

        public static List<string> GetDaprServicesFromSln(ref List<string> services, List<string> slnPaths)
        {
            List<string> projectPaths = [];
            if (slnPaths.Count > 0)
            {
                List<string> slnProjects = FindProjects(slnPaths);
                List<string> daprProjects = GetDaprServices(slnProjects);
                CheckDaprProjects(daprProjects);

                slnProjects = FilterDaprProjects(slnProjects, daprProjects);
                slnProjects = UpdateProjectPaths(slnPaths, slnProjects);

                services.AddRange(daprProjects);
                projectPaths.AddRange(slnProjects);
            }
            return projectPaths;
        }


        public static List<string> FindProjects(List<string> slnPaths)
        {
            List<string> projectPaths = [];
            DirectoryInfo currentDir = new(Directory.GetCurrentDirectory());

            foreach (string path in slnPaths)
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
                string fullPath = Path.GetFullPath(projectPath);
                if (!File.Exists(fullPath))
                {
                    throw new FileNotFoundException($"Error occurred while trying to find the project file: '{projectPath}'");
                }

                XDocument project = XDocument.Load(projectPath);

                if (CheckDependency(project, DAPR))
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

        private static void CheckDaprProjects(List<string> daprProjects)
        {
            if (daprProjects.Count == 0)
            {
                Console.WriteLine("No Dapr services found in the solution! \nUse the --services option to specify the services to generate certificates for.");
            }
        }

        private static List<string> FilterDaprProjects(List<string> projects, List<string> daprProjects)
        {
            return projects.Where(project => daprProjects.Any(daprProject => project.Contains(daprProject))).ToList();
        }


        private static List<string> UpdateProjectPaths(List<string> slnPaths, List<string> projects)
        {
            foreach (string slnPath in slnPaths)
            {
                string gitPath = DirectoryService.CheckProjectType(slnPath);
                for (int i = 0; i < projects.Count; i++)
                {
                    projects[i] = Path.GetRelativePath(gitPath, projects[i]);
                }
            }
            return projects;
        }
    }
}