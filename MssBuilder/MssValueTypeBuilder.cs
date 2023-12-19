using CLI.Templates;
using Mss.Types;
using System.Diagnostics;

namespace MssBuilder
{
    public class MssValueTypeBuilder
    {
        public readonly string ProjectName = "ValueTypes";
        public readonly string ProjectPath;

        private readonly string _solutionPath;
        private readonly ValueClassTemplate _classTemplate = new();
        private readonly ClassLibraryProjectTemplate _projectTemplate = new();

        public MssValueTypeBuilder(string solutionPath)
        {
            _solutionPath = solutionPath;
            ProjectPath = Path.Combine(solutionPath, ProjectName);
        }

        private void GenerateClassFile(MssClassType valueType, DirectoryInfo projectDir)
        {
            string classFile = Path.Combine(projectDir.FullName, valueType.Name + ".cs");
            string classContent = _classTemplate.Render(ProjectName, valueType.Name, valueType.Field.Name,
                                                        valueType.Field.Type.ToString());
            File.WriteAllText(classFile, classContent);
        }

        private void GenerateProjectFile(DirectoryInfo projectDir)
        {
            string projectContent = _projectTemplate.Render();
            File.WriteAllText(projectDir.FullName + "/" + ProjectName + ".csproj", projectContent);
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
