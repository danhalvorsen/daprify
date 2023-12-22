using System.Xml.Linq;

namespace CLI.Models
{
    public interface IProject
    {
        IPath GetPath();
        XDocument GetProject();
        RelativePath GetRelativeProjPath();
        RelativePath GetRelativeSlnPath();
        ISolution GetSolution();
        void SetPath(IPath path);
        void SetRelativeProjectPath(IPath basePath);
        bool CheckPackageReference(string dependency);
        Name GetName();
        bool Contains(string target);
    }

    public class Project : IProject
    {
        private readonly IQuery _query;
        private readonly XDocument _project;
        private IPath _path;
        private readonly RelativePath _relativeSlnPath;
        private RelativePath _relativeProjPath;
        private readonly ISolution _solution;
        private readonly Name _name;

        public Project(IQuery query, Name name)
        {
            _query = query;
            _name = name;
            _path = new MyPath();
        }

        public Project(IQuery query, ISolution sln, string projectPath)
        {
            _query = query;
            _solution = sln;
            if (!File.Exists(projectPath))
            {
                throw new FileNotFoundException($"Error occurred while trying to find the project file: '{projectPath}'");
            }

            _project = XDocument.Load(projectPath);
            _path = new AbsolutePath(projectPath);
            _relativeSlnPath = new RelativePath(_solution.GetPath(), _path);
            _name = new(Path.GetFileNameWithoutExtension(_path.ToString()));
        }

        public static IEnumerable<Project> FromStringList(List<string> names)
        {
            Query query = new();
            return names.Select(name => new Project(query, new(name)));
        }

        public Name GetName() => _name;
        public IPath GetPath() => _path;
        public XDocument GetProject() => _project;
        public RelativePath GetRelativeProjPath() => _relativeProjPath;
        public RelativePath GetRelativeSlnPath() => _relativeSlnPath;
        public ISolution GetSolution() => _solution;

        public void SetPath(IPath path)
        {
            _path = path ?? throw new ArgumentNullException(nameof(path));
        }

        public void SetRelativeProjectPath(IPath basePath)
        {
            _relativeProjPath = new RelativePath(basePath, _path);
        }

        public bool CheckPackageReference(string dependency) => _query.CheckPackageReference(this, dependency);

        public bool Contains(string target)
        {
            if (string.IsNullOrEmpty(target))
            {
                throw new ArgumentNullException(nameof(target));
            }

            if (_name.ToString().Contains(target, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            return false;
        }
    }
}