using Mss;
using Mss.Types;
using MssBuilder.Projects;
using System.Text;

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

                var solution = new MssCSharpSolution(solutionName);

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

                MssValueTypeBuilder valueBuilder = new();
                MssCSharpProject valueTypeProject = valueBuilder.Build(classes);
                solution.Add(valueTypeProject);

                MssServiceBuilder serviceBuilder = new();
                foreach (var service in spec.Services)
                {
                    solution.Add(serviceBuilder.Build(service));
                }

                solution.Write(solutionPath);
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
