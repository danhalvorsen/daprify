using Daprify.Models;
using Serilog;

namespace Daprify.Services
{
    public static class SolutionService
    {
        private const string DAPR = "Dapr";

        public static IEnumerable<IProject> GetDaprServicesFromSln(MyPath projectRoot, IEnumerable<ISolution> solutions)
        {
            if (!solutions.Any())
            {
                return [];
            }

            List<IProject> slnProjects = FindProjects(solutions);
            slnProjects = GetDaprServices(slnProjects);

            return UpdateProjectPaths(projectRoot, slnProjects);
        }

        private static List<IProject> FindProjects(IEnumerable<ISolution> solutions)
        {
            return solutions.SelectMany(sln => sln.GetProjects()).ToList();
        }

        private static List<IProject> GetDaprServices(List<IProject> projects)
        {
            Log.Verbose("Searching for projects with Dapr package reference...");
            var daprProjects = projects.Where(project => project.CheckPackageReference(DAPR)).ToList();

            if (daprProjects.Count == 0)
            {
                Log.Error("No projects with Dapr package reference found!");
                Log.Information("Use the --services option to specify the services");
                Log.Information("Shutting down...");
                Environment.Exit(1);
            }
            else
            {
                Log.Verbose("Found projects with Dapr package reference:{NewLine}{Paths}", Environment.NewLine, string.Join(Environment.NewLine, daprProjects.Select(project => project.GetPath())));
            }
            return daprProjects;
        }

        private static List<IProject> UpdateProjectPaths(MyPath projectRoot, List<IProject> projects)
        {
            foreach (IProject project in projects.ToList())
            {
                if (string.IsNullOrEmpty(projectRoot?.ToString()))
                {
                    projectRoot = DirectoryService.CheckProjectType(project.GetSolution().GetPath());
                }

                project.SetRelativeProjectPath(projectRoot);
            }
            return projects;
        }
    }
}