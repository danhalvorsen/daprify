
using Microsoft.Build.Construction;
using Serilog;

namespace Daprify.Models
{
    public interface IProjectProvider
    {
        IEnumerable<Project> GetProjects(Solution solution);
    }

    public class ProjectProvider(IQuery query) : IProjectProvider
    {
        private readonly IQuery _query = query;

        public IEnumerable<Project> GetProjects(Solution solution)
        {
            SolutionFile file = solution.GetSolutionFile();
            Log.Verbose("Trying to get projects from solution: {solution}", solution.GetPath());

            var filteredProjects = file.ProjectsInOrder.Where(project =>
                                        project.ProjectType == SolutionProjectType.KnownToBeMSBuildFormat &&
                                        !project.ProjectName.Contains("Test", StringComparison.OrdinalIgnoreCase));

            var projects = filteredProjects.Select(project => new Project(_query, solution, project.AbsolutePath));
            Log.Verbose("Found projects: {NewLine}{Paths}", Environment.NewLine, string.Join(Environment.NewLine, projects.Select(project => project.GetPath())));

            return projects;
        }
    }
}