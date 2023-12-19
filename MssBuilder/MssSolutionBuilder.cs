using Mss;
using Mss.Types;

namespace MssBuilder
{
    public static class MssSolutionBuilder
    {
        public static void GenerateServices(MssSpec spec, string path, string solutionName)
        {
            Console.WriteLine("Generating solution: " + solutionName);
            if (Path.Exists(path))
            {
                string solutionPath = Path.Combine(path, solutionName);
                if (Path.Exists(solutionPath))
                {
                    Directory.Delete(solutionPath, true);
                }
                DirectoryInfo dir = Directory.CreateDirectory(solutionPath);

                List<MssClassType> classes = [];
                List<MssExternType> externs = [];
                foreach (var type in spec.Types)
                {
                    if (type is MssClassType classType)
                    {
                        classes.Add(classType);
                    }
                    if (type is MssExternType externType)
                    {
                        externs.Add(externType);
                    }
                }

                List<MssCSharpProject> projects = [];

                MssValueTypeBuilder valueBuilder = new();
                MssCSharpProject valueTypeProject = valueBuilder.Build(classes);
                projects.Add(valueTypeProject);

                foreach (var project in projects)
                {
                    project.Write(dir.FullName);
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("Invalid Path: ");
                Console.ResetColor();
                Console.WriteLine(path);
            }
        }
    }
}
