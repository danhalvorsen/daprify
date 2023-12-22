using System.Xml.Linq;

namespace CLI.Models
{

    public interface IProject
    {
        MyPath GetPath();
        XDocument GetProject();
        RelativePath GetRelativeProjPath();
        Solution GetSolution();
        void SetPath(MyPath path);
        void SetRelativeProjectPath(MyPath basePath);
        bool CheckPackageReference(string dependency);
        Name GetName();
        bool Contains(string target);
    }

    public class Project : IProject
    {
        private readonly IQuery query;
        private readonly XDocument project;
        private MyPath path;
        private readonly RelativePath relativeSlnPath;
        private RelativePath relativeProjPath;
        private readonly Solution solution;
        private readonly Name name;

        public Project(IQuery query, Name name)
        {
            this.query = query;
            this.name = name;
            this.path = new MyPath();
        }

        public Project(IQuery query, Solution sln, string projectPath)
        {
            this.query = query;
            this.solution = sln;
            if (!File.Exists(projectPath))
            {
                throw new FileNotFoundException($"Error occurred while trying to find the project file: '{projectPath}'");
            }

            this.project = XDocument.Load(projectPath);
            this.path = new MyPath(projectPath);
            this.relativeSlnPath = new RelativePath(solution.GetPath(), this.path);
            this.name = new(Path.GetFileNameWithoutExtension(this.path.ToString()));
        }

        public static IEnumerable<Project> FromStringList(List<string> names)
        {
            Query query = new();
            return names.Select(name => new Project(query, new(name)));
        }

        public MyPath GetPath() => this.path;
        public XDocument GetProject() => this.project;
        public RelativePath GetRelativeProjPath() => this.relativeProjPath;
        public Solution GetSolution() => this.solution;

        public void SetPath(MyPath path)
        {
            this.path = path ?? throw new ArgumentNullException(nameof(path));
        }


        public void SetRelativeProjectPath(MyPath basePath)
        {
            this.relativeProjPath = new RelativePath(basePath, this.path);
        }

        public bool CheckPackageReference(string dependency) => this.query.CheckPackageReference(this, dependency);

        public Name GetName() => this.name;

        public bool Contains(string target)
        {
            if (string.IsNullOrEmpty(target))
            {
                throw new ArgumentNullException(nameof(target));
            }

            if (this.name.ToString().Contains(target, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            return false;
        }
    }
}