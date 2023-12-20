using CLI.Templates;
using Mss.Types;
using System.Diagnostics;
using MssBuilder.Projects;

namespace MssBuilder
{
    public class MssValueTypeBuilder
    {
        public readonly string ProjectName = "ValueTypes";

        private readonly ValueClassTemplate _classTemplate = new();

        private MssCSharpFile GenerateClassFile(MssClassType valueType)
        {
            string classFile = valueType.Name + ".cs";
            string classContent = _classTemplate.Render(ProjectName, valueType.Name, valueType.Field.Name,
                                                        valueType.Field.Type.ToString());
            return new(classFile, classContent);
        }

        public MssCSharpProject Build(IEnumerable<MssClassType> classes)
        {
            Debug.Assert(classes.Any());
            MssClassLibraryProject project = new(ProjectName, ProjectName);

            foreach (var valueType in classes)
            {
                project.AddFile(GenerateClassFile(valueType));
            }

            return project;
        }
    }
}
