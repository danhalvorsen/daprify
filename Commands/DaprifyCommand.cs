using Daprify.Models;
using Daprify.Services;
using Daprify.Settings;
using Daprify.Validation;
using FluentValidation.Results;
using Serilog;
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
        private readonly Option<bool> _verboseOption = new(["--v", "--verbose"], "Enable verbose logging");

        public DaprifyCommand(Service service, Settings settings, IOptionValidatorFactory validatorFactory)
            : base(settings.CommandName, settings.CommandDescription + settings.CommandExample)
        {
            _service = service;
            _settings = settings;
            _validator = validatorFactory.CreateValidator(settings);

            AddOptions();
            AddGlobalOption(_verboseOption);
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
            Console.WriteLine("Starting to execute command...");
            AddLogging(context);
            ValidateCommand(context.ParseResult.CommandResult);
            GetOptionArguments(context);
            _service.Generate(_optionArguments);
        }

        private void AddLogging(InvocationContext context)
        {
            var optionResult = context.ParseResult.FindResultFor(_verboseOption);
            if (optionResult != null)
            {
                bool enableVerbose = optionResult.GetValueOrDefault<bool>();
                if (enableVerbose)
                {
                    Log.Logger = new LoggerConfiguration()
                                    .MinimumLevel.Verbose()
                                    .WriteTo.Console()
                                    .CreateLogger();
                    Log.Verbose("Verbose logging enabled");
                }
            }
        }

        private void ValidateCommand(CommandResult commandResult)
        {
            ValidationResult result = _validator.Validate(commandResult);

            if (!result.IsValid)
            {
                List<string> errorMessages = result.Errors.Select(error => error.ErrorMessage).ToList();
                string exceptionMessage = string.Join(Environment.NewLine, errorMessages);
                Log.Error("Validation failed: {ValidationErrors}", exceptionMessage);

                Log.Information("Shutting down...");
                Environment.Exit(1);
            }
        }

        private void GetOptionArguments(InvocationContext context)
        {
            foreach (var option in _settings.Options)
            {
                var values = context.ParseResult.GetValueForOption(option);
                if (values != null && values.Count > 0)
                {
                    Log.Verbose("Registering option {Option} with values {Values}", option.Name, values);
                    _optionArguments.Add(new(option.Name), new(values));
                }
                else
                {
                    Log.Warning("No values found for option {Option}", option.Name);
                }
            }
        }
    }
}