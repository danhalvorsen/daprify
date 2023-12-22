using System.CommandLine;
using System.CommandLine.Parsing;
using CLI.Settings;

namespace CLI.Validation
{
    public abstract class CommandValidator() : IValidator
    {
        private const string SOLUTION_OPT = "--sp";
        public void Validate(CommandResult commandResult, ISettings settings)
        {

            List<string>? slnPaths = GetOptionValue<List<string>>(commandResult, settings, SOLUTION_OPT);
            if (slnPaths != null && slnPaths.Count > 0)
            {
                foreach (string path in slnPaths)
                {
                    string currentDir = Directory.GetCurrentDirectory();
                    string slnDir = Path.Combine(currentDir, path);
                    if (!Directory.Exists(slnDir))
                    {
                        throw new DirectoryNotFoundException($"Solution path {slnDir} does not exist");
                    }
                }
            }
        }


        public static T? GetOptionValue<T>(CommandResult commandResult, ISettings settings, string optionName)
        {
            Option<List<string>>? option = settings.Options.Find(o => o.Aliases.Contains(optionName));

            if (option is Option<T> typedOption)
            {
                return commandResult.GetValueForOption(typedOption);
            }

            return default;
        }
    }
}