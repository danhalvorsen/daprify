using CLI.Models;

namespace CLI.Services
{
    public class SolutionService()
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
            CheckDaprProjects(slnProjects);

            return UpdateProjectPaths(projectRoot, slnProjects);
        }

        private static List<IProject> FindProjects(IEnumerable<ISolution> solutions)
        {
            return solutions.SelectMany(sln => sln.GetProjects()).ToList();
        }

        private static List<IProject> GetDaprServices(List<IProject> projects)
        {
            return projects.Where(project => project.CheckPackageReference(DAPR)).ToList();
        }

        private static void CheckDaprProjects(List<IProject> daprProjects)
        {
            if (daprProjects.Count == 0)
            {
                Console.WriteLine("No Dapr services found in the solution! \nUse the --services option to specify the services to generate certificates for.");
            }
        }

        private static List<IProject> UpdateProjectPaths(MyPath projectRoot, List<IProject> projects)
        {
            foreach (IProject project in projects.ToList())
            {
                if (string.IsNullOrEmpty(projectRoot?.ToString()))
                {
                    projectRoot = new(DirectoryService.CheckProjectType(project.GetSolution().GetPath().ToString()));
                }

                project.SetRelativeProjectPath(projectRoot);
            }
            return projects;
        }
    }
}