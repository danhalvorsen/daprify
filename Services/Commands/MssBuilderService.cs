using CLI.Models;
using MssBuilder;

namespace CLI.Services
{
    public class MssBuilderService() : CommandService(MSS_DIRECTORY)
    {
        private const string MSS_DIRECTORY = "MssDir";
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

        private static string? GetOutputDirectory(OptionDictionary options)
        {
            var output = options.GetAllPairValues("out");
            if (output != null && output.Count == 1)
            {
                if (Directory.Exists(output[0]))
                {
                    return output[0];
                }
                else
                {
                    PrintError(output[0] + " does not exist.");
                    return null;
                }
            }
            else
            {
                return "./";
            }
        }

        public override void Generate(OptionDictionary options)
        {
            string? inFile = FindInputFile(options);
            string? outDir = GetOutputDirectory(options);
            if (inFile != null && outDir != null)
            {
                Console.WriteLine("Parsing: " + inFile + " -> " + outDir);
                Builder.GenerateServices(null!, "test");
            }
        }
    }
}