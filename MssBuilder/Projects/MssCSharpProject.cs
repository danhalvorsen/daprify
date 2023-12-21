using System.Xml.Linq;

namespace MssBuilder.Projects
{
    public class ProjectDependency(string name)
    {
        public readonly string Name = name;
        public readonly XElement Xml = new("ProjectReference",
                                           new XAttribute("Include", $"..\\{name}\\{name}.csproj"));
    }

    public abstract class MssCSharpProject(string name, string relPath)
    {
        // solution file uuid for c# project/class library
        protected Guid _csharpProjectUUID = new("FAE04EC0-301F-11D3-BF4B-00C04F79EFBC");
        protected Guid _aspCoreProjectUUID = new("9A19103F-16F7-4668-BE54-9A1E7A4F7556");

        public readonly string Name = name;
        public readonly string RelPath = relPath;
        public readonly Guid Id = Guid.NewGuid();

        protected bool _useWebSdk = false;

        public virtual Guid TypeGuid { get => _csharpProjectUUID; }

        public IEnumerable<string> Dependencies { get => _projectReferences.Select(d => d.Name); }

        private readonly List<MssCSharpFile> _files = [];

        protected readonly List<ProjectDependency> _projectReferences = [];
        protected readonly List<XElement> _packageReferences = [];

        protected readonly int _dotnet_major_version = 8;
        protected readonly int _dotnet_minor_version = 0;

        public void AddFile(MssCSharpFile file) => _files.Add(file);
        public void AddFiles(IEnumerable<MssCSharpFile> file) => _files.AddRange(file);

        public void AddPackageReference(string packageName, string packageVersion)
        {
            _packageReferences.Add(new XElement("PackageReference",
                                                new XAttribute("Include", packageName),
                                                new XAttribute("Version", packageVersion)));
        }

        protected XElement CreatePackageReferences()
        {
            var result = new XElement("ItemGroup");
            foreach (var reference in _packageReferences)
            {
                result.Add(reference);
            }
            return result;
        }

        public void AddProjectReference(string projectName)
        {
            _projectReferences.Add(new ProjectDependency(projectName));
        }

        protected XElement CreateProjectReferences()
        {
            var result = new XElement("ItemGroup");
            foreach (var reference in _projectReferences)
            {
                result.Add(reference.Xml);
            }
            return result;
        }

        protected XElement CreateProjectHeader()
        {
            if (_useWebSdk)
            {
                return new XElement("Project", new XAttribute("Sdk", "Microsoft.NET.Sdk.Web"));
            }
            else
            {
                return new XElement("Project", new XAttribute("Sdk", "Microsoft.NET.Sdk"));
            }
        }

        protected XElement CreatePropertyGroup()
        {
            return new XElement("PropertyGroup",
                                new XElement("TargetFramework", $"net{_dotnet_major_version}.{_dotnet_minor_version}"),
                                new XElement("ImplicitUsings", "enable"),
                                new XElement("Nullable", "enable"));
        }

        protected abstract string CreateProjectFile();

        public void Write(string path)
        {
            path = Path.Combine(path, RelPath);
            if (!Path.Exists(path))
            {
                _ = Directory.CreateDirectory(path);
            }

            File.WriteAllText(Path.Combine(path, Name + ".csproj"), CreateProjectFile());

            foreach (var file in _files)
            {
                file.Write(path);
            }
        }
    }
}