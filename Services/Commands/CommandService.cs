using System.Text.RegularExpressions;
using CLI.Models;

namespace CLI.Services
{
    public abstract partial class CommandService(string directoryName) : IService
    {
        protected readonly string _directoryName = directoryName;

        public virtual void Generate(OptionDictionary options)
        {
            try
            {
                string workingDir = DirectoryService.SetDirectory(_directoryName);
                List<string> generatedFiles = CreateFiles(options, workingDir);

                Console.WriteLine(FormatOutput(generatedFiles));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        protected virtual List<string> CreateFiles(OptionDictionary options, string workingDir) => [];

        protected virtual string GetArgumentTemplate(string argument, string template) => "";

        protected string FormatOutput(List<string> generatedFiles)
        {
            if (generatedFiles.Count == 0)
            {
                return $"No {_directoryName.ToLower()} files were generated.";
            }

            string fileList = string.Join(", ", generatedFiles);
            return $"The {_directoryName.ToLower()} files: {fileList} were generated successfully.";
        }


        [GeneratedRegex(@"^.*\{\{.*?\}\}.*(\r?\n|\r)?", RegexOptions.Multiline | RegexOptions.Compiled)]
        public static partial Regex PlaceholderRegex();
    }
}