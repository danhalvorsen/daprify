using Daprify.Models;
using FluentValidation;

namespace Daprify.Validation
{
    public class ConfigValidator : AbstractValidator<OptionDictionary>
    {
        private readonly MyPathValidator _myPathValidator;
        private readonly OptionValuesValidator _optionValuesValidator;

        public ConfigValidator(MyPathValidator myPathValidator, OptionValuesValidator optionValuesValidator)
        {
            _myPathValidator = myPathValidator;
            _optionValuesValidator = optionValuesValidator;
            DefineRules();
        }

        public void DefineRules()
        {
            Key projectPathKey = new("project_path");
            Key solutionPathKey = new("solution_paths"); ;
            Key componentKey = new("components");
            Key settingsKey = new("settings");

            RuleFor(options => options.GetAllPairValues(componentKey))
                .SetValidator(_optionValuesValidator);

            RuleFor(options => options.GetAllPairValues(settingsKey))
                .SetValidator(_optionValuesValidator);

            RuleFor(options => GetPath(options, projectPathKey))
                .SetValidator(_myPathValidator);

            RuleFor(options => GetPath(options, solutionPathKey))
                .SetValidator(_myPathValidator);
        }

        private static IEnumerable<MyPath> GetPath(OptionDictionary options, Key key)
        {
            var values = options.GetAllPairValues(key).GetValues();
            if (values != null && values.Any())
            {
                return values.Select(v => new MyPath(v));
            }

            return Enumerable.Empty<MyPath>();
        }
    }
}