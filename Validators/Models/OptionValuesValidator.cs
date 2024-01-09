using Daprify.Models;
using FluentValidation;
using Daprify.Settings;
using Serilog;

namespace Daprify.Validation
{
    public class OptionValuesValidator : AbstractValidator<OptionValues>
    {
        private readonly List<Value> _invalidOptions = [];

        public OptionValuesValidator()
        {
            RuleFor(val => val)
                .Custom((optionValues, context) =>
                {
                    if (optionValues.GetValues() == null || !optionValues.GetValues().Any())
                    {
                        Log.Verbose("Skipping validation of option {Option} since it has no values", optionValues.GetKey().ToString());
                        return;
                    }

                    if (!ValidArgument(optionValues))
                    {
                        string error = ErrorMessage.InvalidErrorFunc(optionValues.GetKey().ToString(), _invalidOptions);
                        context.AddFailure(error);
                    }
                    else
                    { Log.Verbose("Validation success: The options {Option} is valid", optionValues.GetValues()); }
                });
        }

        private bool ValidArgument(OptionValues optionValues)
        {
            string key = optionValues.GetKey().ToString();
            Log.Verbose("Validating if the values for option {Option} is valid", key);

            return key switch
            {
                "components" => ValidateOptionValues(optionValues, GenAllSettings.AvailableComponents, "component"),
                "settings" => ValidateOptionValues(optionValues, GenAllSettings.AvailableSettings, "setting"),
                _ => false
            };
        }

        private bool ValidateOptionValues(OptionValues optionValues, IEnumerable<string> validOptions, string optionType)
        {
            var values = optionValues.GetValues();
            return values.All(value =>
            {
                bool isValid = validOptions.Contains(value.ToString());
                Log.Verbose("Validating {OptionType} '{OptionValue}': {IsValid}", optionType, value, isValid);

                if (!isValid)
                {
                    _invalidOptions.Add(value);
                }

                return isValid;
            });
        }
    }
}
