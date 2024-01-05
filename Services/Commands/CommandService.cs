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
                PrintExecuting();
                MyPath projectPath = CheckRootPath(options);
                IPath workingDir = DirectoryService.SetDirectory(projectPath, _directoryName);
                IEnumerable<string> generatedFiles = CreateFiles(options, workingDir);

                PrintFinish(generatedFiles);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error: {ex.Message}");
                Console.ResetColor();
            }
        }

        public void PrintExecuting()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Executing command: {GetType().Name[..^7]}");
            Console.ResetColor();
        }

        public static MyPath CheckRootPath(OptionDictionary options)
        {
            MyPath path = GetFirstPathOption(options, "project_path");
            if (path != null)
            {
                return path;
            }

            path = GetFirstPathOption(options, "solution_paths");
            if (path != null)
            {
                path.SetDirectoryPath();
                return path;
            }

            Log.Verbose("No project or solution path found");
            return new MyPath(string.Empty);
        }

        private static MyPath GetFirstPathOption(OptionDictionary options, string keyName)
        {
            Key pathKey = new(keyName);
            Log.Verbose($"Checking for {keyName}...");
            var pathOpt = options.GetAllPairValues(pathKey).GetValues();

            if (pathOpt != null && pathOpt.Any())
            {
                Value path = pathOpt.First();
                Log.Verbose($"Found {keyName}: {path}");
                return new MyPath(path);
            }

            return null!;
        }

        public virtual void PrintFinish(IEnumerable<string> generatedFiles)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Command executed successfully!");
            Console.ResetColor();

            if (generatedFiles == null || !generatedFiles.Any())
            {

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("No files generated!");
                Console.ResetColor();
                return;
            }

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