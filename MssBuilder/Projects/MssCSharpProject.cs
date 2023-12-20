using System.Xml.Linq;

namespace MssBuilder.Projects
{
    public abstract class MssCSharpProject(string name, string relPath)
    {
        public readonly string Name = name;
        public readonly string RelPath = relPath;

        private readonly List<MssCSharpFile> _files = [];

        protected readonly List<string> _projectReferences = [];

        protected readonly int _dotnet_major_version = 8;
        protected readonly int _dotnet_minor_version = 0;

        public void AddFile(MssCSharpFile file) => _files.Add(file);
        public void AddFiles(IEnumerable<MssCSharpFile> file) => _files.AddRange(file);

        public void AddProjectReference(string projectName)
        {
            _projectReferences.Add(projectName);
        }

        protected XElement CreateProjectReferences()
        {
            var result = new XElement("ItemGroup");
            foreach (var reference in _projectReferences)
            {
                result.Add(new XElement("ProjectReference",
                                        new XAttribute("Include", $"..\\{reference}\\{reference}.csproj")));
            }
            return result;
        }

        protected static XElement CreateProjectHeader()
        {
            return new XElement("Project", new XAttribute("Sdk", "Microsoft.NET.Sdk"));
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