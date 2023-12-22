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
        private readonly SolutionFile _solution;
        private readonly AbsolutePath _path;
        private readonly IQuery _query;
        private readonly IProjectProvider _projectProvider;
        private const string SLN_EXT = "*.sln";

        public Solution() { }

        public Solution(IQuery query, IProjectProvider projectProvider, IPath path)
        {
            _query = query;
            string slnDir = Path.Combine(Directory.GetCurrentDirectory(), path.ToString());
            string? sln = _query.GetFileInDirectory(slnDir, SLN_EXT);
            string? slnPath = sln != null ? Path.GetFullPath(sln, slnDir) : null;
            if (slnPath == null || !File.Exists(slnPath))
            {
                throw new FileNotFoundException($"Could not find the solution file: {slnPath}");
            }

            _path = new(slnPath);
            _solution = SolutionFile.Parse(_path.ToString());
            _projectProvider = projectProvider;
        }

        public IPath GetPath() => _path;
        public SolutionFile GetSolutionFile() => _solution;
        public IEnumerable<IProject> GetProjects() => _projectProvider.GetProjects(this);
    }
}