using System.Xml.Linq;

namespace MssBuilder.Projects
{
    public abstract class MssCSharpProject(string name, string relPath)
    {
        public readonly string Name = name;
        public readonly string RelPath = relPath;

        private readonly List<MssCSharpFile> _files = [];

        protected readonly List<XElement> _projectReferences = [];
        protected readonly List<XElement> _packageReferences = [];

        protected readonly int _dotnet_major_version = 8;
        protected readonly int _dotnet_minor_version = 0;

        public void AddFile(MssCSharpFile file) => _files.Add(file);
        public void AddFiles(IEnumerable<MssCSharpFile> file) => _files.AddRange(file);
        /*
          <ItemGroup>
            <PackageReference Include="Microsoft.EntityFrameworkCore" Version="8.0.0" />
            <PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="8.0.0">
              <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
              <PrivateAssets>all</PrivateAssets>
            </PackageReference>
            <PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="8.0.0" />
            <PackageReference Include="Microsoft.Extensions.Configuration" Version="8.0.0" />
            <PackageReference Include="Microsoft.Extensions.Configuration.FileExtensions" Version="8.0.0" />
            <PackageReference Include="Microsoft.Extensions.Configuration.Json" Version="8.0.0" />
          </ItemGroup>
          */

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
            _projectReferences.Add(new XElement("ProjectReference",
                                                new XAttribute("Include", $"..\\{projectName}\\{projectName}.csproj")));
        }

        protected XElement CreateProjectReferences()
        {
            var result = new XElement("ItemGroup");
            foreach (var reference in _projectReferences)
            {
                result.Add(reference);
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