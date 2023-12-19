using CLI.Models;
using Mss;
using Mss.Parsing;

namespace CLI.Services
{
    public class CompileToPumlService() : CommandService(PUML_DIRECTORY)
    {
        private const string PUML_DIRECTORY = "PlantUML";
        private const string MSS_EXT = ".mss";
        private const string WILDCARD = "*" + MSS_EXT;

        private static void PrintError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        private static string? FindInputFile(OptionDictionary options)
        {
            var inFile = DirectoryService.GetInputFile(options, WILDCARD);
            if (inFile == null)
            {
                PrintError("No " + WILDCARD + " files found.");
            }
            else if (!File.Exists(inFile))
            {
                PrintError(inFile + " does not exist.");
                return null;
            }

            return inFile;
        }

        private static string FindOutputFile(OptionDictionary options, string inFile)
        {
            var output = options.GetAllPairValues("out");
            if (output != null && output.Count == 1)
            {
                return output[0];
            }
            else
            {
                string dir = Path.GetDirectoryName(Path.GetFullPath(inFile))!;
                string filename = Path.GetFileNameWithoutExtension(inFile);
                return Path.Combine(dir, filename + ".plantuml");
            }
        }

        public override void Generate(OptionDictionary options)
        {
            string? inFile = FindInputFile(options);
            if (inFile != null)
            {
                string outFile = FindOutputFile(options, inFile);

                Console.Write("Parsing: ");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(inFile);
                Console.ResetColor();

                string content = File.ReadAllText(inFile);
                MssSpec? spec = MssParser.ParseMss(inFile, content);

                if (spec != null)
                {
                    string pumlName = Path.GetFileNameWithoutExtension(inFile);
                    string puml = PumlGenerator.Generate(pumlName, spec, true);
                    Console.Write("Writing PlantUML to: ");
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine(outFile);
                    Console.ResetColor();
                    File.WriteAllText(outFile, puml);
                }
            }
        }
    }
}