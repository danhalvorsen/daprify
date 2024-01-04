using Daprify.Models;
using Serilog;
using System.Text.RegularExpressions;

namespace Daprify.Services
{
    public abstract partial class CommandService(string directoryName) : IService
    {
        protected readonly string _directoryName = directoryName;

        public virtual void Generate(OptionDictionary options)
        {
            try
            {
                IPath workingDir = DirectoryService.SetDirectory(_directoryName);
                Log.Verbose("Successfully created working directory: {working}", workingDir);
                IEnumerable<string> generatedFiles = CreateFiles(options, workingDir);

                WriteOutput(generatedFiles);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Erorr: {ex.Message}");
                Console.ResetColor();
            }
        }

        public virtual void WriteOutput(IEnumerable<string> generatedFiles)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Command executed successfully!");
            Console.ResetColor();
            Console.WriteLine("Generated files:");
            foreach (string file in generatedFiles)
            {
                Console.WriteLine($"    - {file}");
            }
        }


        protected virtual IEnumerable<string> CreateFiles(OptionDictionary options, IPath workingDir) => [];

        [GeneratedRegex(@"^\s*.*\{\{\s*\}\}.*(\r?\n|\r)?", RegexOptions.Multiline | RegexOptions.Compiled)]
        public static partial Regex PlaceholderRegex();
    }
}