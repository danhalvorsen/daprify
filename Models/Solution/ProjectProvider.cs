
using Microsoft.Build.Construction;

namespace CLI.Models
{
    public interface IProjectProvider
    {
        IEnumerable<Project> GetProjects(Solution solution);
    }

    public class ProjectProvider : IProjectProvider
    {
        private readonly IQuery query;

        public ProjectProvider(IQuery query)
        {
            this.query = query;
        }

        public IEnumerable<Project> GetProjects(Solution solution)
        {
            SolutionFile file = solution.GetSolutionFile();

            var filteredProjects = file.ProjectsInOrder.Where(project =>
                                        project.ProjectType == SolutionProjectType.KnownToBeMSBuildFormat &&
                                        !project.ProjectName.Contains("Test", StringComparison.OrdinalIgnoreCase));

            return filteredProjects.Select(project => new Project(query, solution, project.AbsolutePath));
        }
    }
}