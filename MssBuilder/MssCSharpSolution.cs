using MssBuilder.Projects;
using System.Text;

namespace MssBuilder
{
    public class MssCSharpSolution(string name)
    {
        public string Name { get; } = name;

        private readonly List<MssCSharpProject> projects = [];

        protected readonly string _formatVersionMajor = "12";
        protected readonly string _formatVersionMinor = "00";

        protected readonly string _vsVersionMajor = "17";
        protected readonly string _vsVersionMinor = "0";
        protected readonly string _vsVersionPatch = "31903";
        protected readonly string _vsVersionSubPatch = "59";

        protected readonly string _vsMinVersionMajor = "10";
        protected readonly string _vsMinVersionMinor = "0";
        protected readonly string _vsMinVersionPatch = "40291";
        protected readonly string _vsMinVersionSubPatch = "1";

        public void Add(MssCSharpProject project)
        {
            projects.Add(project);
        }

        private void WriteGlobalProperties(StringBuilder builder)
        {
            builder.AppendLine($"Microsoft Visual Studio Solution File, Format Version {_formatVersionMajor}.{_formatVersionMinor}");
            builder.AppendLine($"# Visual Studio Version {_vsVersionMajor}");
            builder.AppendLine($"VisualStudioVersion = {_vsVersionMajor}.{_vsVersionMinor}.{_vsVersionPatch}.{_vsVersionSubPatch}");
            builder.AppendLine($"MinimumVisualStudioVersion = {_vsMinVersionMajor}.{_vsMinVersionMinor}.{_vsMinVersionPatch}.{_vsMinVersionSubPatch}");
        }

        private void WriteProjectSection(StringBuilder builder)
        {
            foreach (var project in projects)
            {
                builder.AppendLine(@$"Project(""{{{project.TypeGuid}}}"") = ""{project.Name}"", ""{project.Name}\{project.Name}.csproj"", ""{{{project.Id}}}""");
                if (project.Dependencies.Any())
                {
                    builder.AppendLine("    ProjectSection(ProjectDependencies) = postProject");
                    foreach (string projectName in project.Dependencies)
                    {
                        Guid dependencyId = projects.Where(p => p.Name == projectName).Select(p => p.Id).FirstOrDefault()!;
                        builder.AppendLine($"{{{project.Id}}} = {{{dependencyId}}}");
                    }
                    builder.AppendLine("    EndProjectSection");
                }
            }
        }

        private void WriteGlobalSection(StringBuilder builder)
        {
            builder.AppendLine("Global");
            builder.AppendLine("    GlobalSection(SolutionConfigurationPlatforms) = preSolution");
            builder.AppendLine("        Debug|Any CPU = Debug|Any CPU");
            builder.AppendLine("        Release|Any CPU = Release|Any CPU");
            builder.AppendLine("    EndGlobalSection");
            builder.AppendLine("    GlobalSection(SolutionConfigurationPlatforms) = postSolution");
            foreach (var project in projects)
            {
                builder.AppendLine($"        {{{project.Id}}}.Debug|Any CPU.ActiveCfg = Debug|Any CPU");
                builder.AppendLine($"        {{{project.Id}}}.Debug|Any CPU.Build.0 = Debug|Any CPU");
                builder.AppendLine($"        {{{project.Id}}}.Release|Any CPU.ActiveCfg = Release|Any CPU");
                builder.AppendLine($"        {{{project.Id}}}.Release|Any CPU.Build.0 = Release|Any CPU");
            }
            builder.AppendLine("    EndGlobalSection");
            builder.AppendLine("    GlobalSection(SolutionProperties) = preSolution");
            builder.AppendLine("        HideSolutionNode = FALSE");
            builder.AppendLine("    EndGlobalSection");
            builder.AppendLine("    GlobalSection(ExstensibilityGlobals) = postSolution");
            builder.AppendLine($"        SolutionGuid = {{{Guid.NewGuid()}}}");
            builder.AppendLine("    EndGlobalSection");
            builder.AppendLine("EndGlobal");
        }

        public void Write(string path)
        {
            StringBuilder solutionBuilder = new();
            WriteGlobalProperties(solutionBuilder);
            WriteProjectSection(solutionBuilder);
            WriteGlobalSection(solutionBuilder);

            File.WriteAllText(Path.Combine(path, Name + ".sln"), solutionBuilder.ToString());

            foreach (var project in projects)
            {
                project.Write(path);
            }

        }
    }
}