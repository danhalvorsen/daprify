using Microsoft.Build.Construction;
using Serilog;

namespace Daprify.Models
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
        private readonly IProjectProvider _projectProvider;
        private const string SLN_EXT = "*.sln";

        public Solution() { }

        public Solution(IQuery query, IProjectProvider projectProvider, IPath path)
        {
            MyPath slnDir = MyPath.Combine(Directory.GetCurrentDirectory(), path.ToString());
            string? sln = query.GetFileInDirectory(slnDir, SLN_EXT);
            string? slnPath = sln != null ? Path.GetFullPath(sln, slnDir.ToString()) : null;
            if (slnPath == null || !File.Exists(slnPath))
            {
                throw new FileNotFoundException($"Could not find the solution file: {slnPath}");
            }

            _path = new(slnPath);
            _solution = SolutionFile.Parse(_path.ToString());
            _projectProvider = projectProvider;
            Log.Verbose("Found solution: {solution}", this.GetPath());
        }

        public IPath GetPath() => _path;
        public SolutionFile GetSolutionFile() => _solution;
        public IEnumerable<IProject> GetProjects() => _projectProvider.GetProjects(this);
    }
}