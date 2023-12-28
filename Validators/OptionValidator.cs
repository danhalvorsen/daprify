using CLI.Models;
using CLI.Settings;
using FluentValidation;
using System.CommandLine.Parsing;

namespace CLI.Validation
{
    public class OptionValidator : AbstractValidator<CommandResult>
    {
        private const string PROJECT_OPT = "--pp";
        private const string SOLUTION_OPT = "--sp";
        private readonly MyPathValidator _myPathValidator;
        private readonly ISettings _settings;

        public OptionValidator(MyPathValidator myPathValidator, ISettings settings)
        {
            _settings = settings;
            _myPathValidator = myPathValidator;
            DefineRules();
        }

        private void DefineRules()
        {
            RuleFor(commandResult => GetPaths(commandResult, SOLUTION_OPT))
                .SetValidator(_myPathValidator);

            RuleFor(commandResult => GetPaths(commandResult, PROJECT_OPT))
                .SetValidator(_myPathValidator);
        }

        public IEnumerable<MyPath> GetPaths(CommandResult commandResult, string optionName)
        {
            List<string>? options = GetOptionValue(commandResult, optionName);
            if (options == null)
            {
                return Enumerable.Empty<MyPath>();
            }
            IEnumerable<MyPath> paths = MyPath.FromStringList(options);
            return paths;
        }

        public List<string>? GetOptionValue(CommandResult commandResult, string optionName)
        {
            var matchedOpt = _settings.Options.Find(o => o.Aliases.Contains(optionName));
            return matchedOpt != null ? commandResult.GetValueForOption(matchedOpt) : default;
        }
    }
}