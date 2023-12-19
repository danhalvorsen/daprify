using HandlebarsDotNet;
using Mss;
using Mss.Types;
using System.Diagnostics;

namespace MssBuilder
{
    public class MssValueTypeBuilder
    {
        private readonly string _solutionPath;
        public readonly string ProjectName = "ValueTypes";
        public readonly string ProjectPath;

        private readonly string _valueClassTemplate =
@"namespace {{Namespace}}
{
    class {{ClassName}}
    {
        public {{TypeName}} {{PropName}} { get; set; }

        public {{ClassName}}() { }
        public {{ClassName}}({{TypeName}} value) => {{PropName}} = value;
    }
}";

        public MssValueTypeBuilder(string solutionPath)
        {
            _solutionPath = solutionPath;
            ProjectPath = Path.Combine(solutionPath, ProjectName);
        }

        private void GenerateClassFile(MssClassType valueType, DirectoryInfo projectDir)
        {
            string classFile = Path.Combine(projectDir.FullName, valueType.Name + ".cs");
            var template = Handlebars.Compile(_valueClassTemplate);
            var data = new
            {
                Namespace = ProjectName,
                ClassName = valueType.Name,
                TypeName = valueType.Field.Type.ToString(),
                PropName = valueType.Field.Name
            };
            var result = template(data);

            Console.WriteLine(classFile);
            Console.WriteLine(result);

            File.WriteAllText(classFile, result);
        }

        private void GenerateProjectFile(DirectoryInfo projectDir)
        {
            var projectTemplate =
@"<Project Sdk=""Microsoft.NET.Sdk"">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

</Project>";
            File.WriteAllText(projectDir.FullName + "/" + ProjectName + ".csproj", projectTemplate);
        }

        public void Generate(IEnumerable<MssClassType> classes)
        {
            Debug.Assert(classes.Any());
            string projectPath = Path.Combine(_solutionPath, ProjectName);
            DirectoryInfo projectDir = Directory.CreateDirectory(projectPath);
            foreach (var valueType in classes)
            {
                GenerateClassFile(valueType, projectDir);
            }
            GenerateProjectFile(projectDir);
        }
    }
}
