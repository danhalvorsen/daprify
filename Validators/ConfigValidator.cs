
using Daprify.Models;
using FluentValidation;

namespace Daprify.Validation
{
    public class ConfigValidator : AbstractValidator<OptionDictionary>
    {
        private readonly MyPathValidator _myPathValidator;

        public ConfigValidator(MyPathValidator myPathValidator)
        {
            _myPathValidator = myPathValidator;
            DefineRules();
        }

        public void DefineRules()
        {
            Key projectPathKey = new("project_path");
            Key solutionPathKey = new("solution_paths"); ;

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