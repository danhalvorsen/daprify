using Daprify.Models;
using Daprify.Services;
using Daprify.Settings;
using Daprify.Validation;
using FluentValidation;
using FluentValidation.Results;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;

namespace Daprify.Commands
{
    public class DaprifyCommand<Service, Settings> : Command
        where Service : IService
        where Settings : ISettings
    {
        private readonly Service _service;
        private readonly Settings _settings;
        private readonly OptionValidator _validator;
        private readonly OptionDictionary _optionArguments = [];

        public DaprifyCommand(Service service, Settings settings, IOptionValidatorFactory validatorFactory)
            : base(settings.CommandName, settings.CommandDescription + settings.CommandExample)
        {
            _service = service;
            _settings = settings;
            _validator = validatorFactory.CreateValidator(settings);

            AddOptions();
            this.SetHandler(Execute);
        }

        private void AddOptions()
        {
            foreach (var option in _settings.Options)
            {
                AddOption(option);
            }
        }

        private void Execute(InvocationContext context)
        {
            ValidateCommand(context.ParseResult.CommandResult);
            GetOptionArguments(context);
            _service.Generate(_optionArguments);
        }

        private void ValidateCommand(CommandResult commandResult)
        {
            ValidationResult result = _validator.Validate(commandResult);

            if (!result.IsValid)
            {
                List<string> errorMessages = result.Errors.Select(error => error.ErrorMessage).ToList();
                string exceptionMessage = string.Join(Environment.NewLine, errorMessages);

                throw new ValidationException(exceptionMessage);
            }
        }

        private void GetOptionArguments(InvocationContext context)
        {
            foreach (var option in _settings.Options)
            {
                var values = context.ParseResult.GetValueForOption(option);
                if (values != null)
                {
                    _optionArguments.Add(new(option.Name), new(values));
                }
            }
        }
    }
}