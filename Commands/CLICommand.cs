using CLI.Models;
using CLI.Services;
using CLI.Settings;
using CLI.Validation;
using System.CommandLine;
using System.CommandLine.Invocation;

namespace CLI.Commands
{
    public class CLICommand<Service, Settings, Validator> : Command
        where Service : IService
        where Settings : ISettings
        where Validator : IValidator
    {
        private readonly Service _service;
        private readonly Settings _settings;
        private readonly Validator _validator;

        public CLICommand(Service service, Settings settings, Validator validator) : base(settings.CommandName, settings.CommandDescription + settings.CommandExample)
        {
            _service = service;
            _settings = settings;
            _validator = validator;

            foreach (var option in _settings.Options)
            {
                AddOption(option);
            }
            AddValidator(commandResult => _validator.Validate(commandResult, _settings));
            this.SetHandler(Execute);
        }

        private void Execute(InvocationContext context)
        {
            OptionDictionary optionArguments = [];
            foreach (var option in _settings.Options)
            {
                string optionName = option.Name;
                var values = context.ParseResult.GetValueForOption(option);
                if (values != null)
                {
                    optionArguments.Add(optionName, values);
                }
            }
            _service.Generate(optionArguments);
        }
    }
}