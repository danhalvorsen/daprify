using Microsoft.Build.Construction;

namespace CLI.Models
{
    public interface ISolution
    {
        IPath GetPath();
        IEnumerable<IProject> GetProjects();
    }

    public class Solution : ISolution
    {
        private readonly SolutionFile solution;
        private readonly AbsolutePath path;
        private readonly IQuery query;
        private readonly IProjectProvider projectProvider;
        private const string SLN_EXT = "*.sln";

        public Solution(IQuery query, IProjectProvider projectProvider, IPath path)
        {
            this.query = query;
            string slnDir = Path.Combine(Directory.GetCurrentDirectory(), path.ToString());
            string? sln = query.GetFileInDirectory(slnDir, SLN_EXT);
            string? slnPath = sln != null ? Path.GetFullPath(sln, slnDir) : null;
            if (slnPath == null || !File.Exists(slnPath))
            {
                throw new FileNotFoundException($"Could not find the solution file: {slnPath}");
            }

            this.path = new(slnPath);
            solution = SolutionFile.Parse(this.path.ToString());
            this.projectProvider = projectProvider;
        }

        public IPath GetPath() => this.path;
        public SolutionFile GetSolutionFile() => solution;
        public IEnumerable<IProject> GetProjects() => projectProvider.GetProjects(this);
    }
}