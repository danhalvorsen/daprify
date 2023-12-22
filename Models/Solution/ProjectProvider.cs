
using Microsoft.Build.Construction;

namespace CLI.Models
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

            var filteredProjects = file.ProjectsInOrder.Where(project =>
                                        project.ProjectType == SolutionProjectType.KnownToBeMSBuildFormat &&
                                        !project.ProjectName.Contains("Test", StringComparison.OrdinalIgnoreCase));

            return filteredProjects.Select(project => new Project(_query, solution, project.AbsolutePath));
        }
    }
}