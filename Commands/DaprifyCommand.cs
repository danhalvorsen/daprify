using Daprify.Models;
using Daprify.Services;
using Daprify.Settings;
using Daprify.Validation;
using FluentValidation.Results;
using Serilog;
using System.CommandLine;
using System.CommandLine.Invocation;

namespace Daprify.Commands
{
    public class DaprifyCommand<Service, Settings> : Command
        where Service : IService
        where Settings : ISettings
    {
        private readonly Service _service;
        private readonly Settings _settings;
        private readonly OptionDictionaryValidator _validator;
        private OptionDictionary _options = [];
        private readonly Option<bool> _verboseOption = new(["--v", "--verbose"], "Enable verbose logging");

        public DaprifyCommand(Service service, Settings settings, OptionDictionaryValidator validator)
            : base(settings.CommandName, settings.CommandDescription + settings.CommandExample)
        {
            _service = service;
            _settings = settings;
            _validator = validator;

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
            ConfigureLogging(context);
            GetOptionArguments(context);
            ValidateOptions();
            _service.Generate(_options);
        }


        private void ConfigureLogging(InvocationContext context)
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


        private void GetOptionArguments(InvocationContext context)
        {
            foreach (var option in _settings.Options)
            {
                List<string>? values = context.ParseResult.GetValueForOption(option);
                if (values == null || values.Count == 0)
                {
                    Log.Warning("No values found for option {Option}", option.Name);
                    continue;
                }

                bool shouldExit = ProcessOption(option.Name, values);
                if (shouldExit)
                {
                    break;
                }
            }
        }

        private bool ProcessOption(string optionName, List<string> values)
        {
            if (optionName == "config")
            {
                _options = LoadConfig(values[0]);
                return true;
            }
            else
            {
                Log.Verbose("Registering option {Option} with values {Values}", optionName, values);
                Key key = new(optionName);
                OptionValues optionValues = new(key, values);
                _options.Add(key, optionValues);
                return false;
            }
        }


        private static OptionDictionary LoadConfig(string configPath)
        {
            Log.Verbose("Loading config from {config}", configPath);
            return OptionDictionary.PopulateFromJson(configPath);
        }


        private void ValidateOptions()
        {
            ValidationResult result = _validator.Validate(_options);
            if (!result.IsValid)
            {
                List<string> errorMessages = result.Errors.Select(error => error.ErrorMessage).ToList();
                string exceptionMessage = string.Join(Environment.NewLine, errorMessages);

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{exceptionMessage}");
                Console.ResetColor();
                Console.WriteLine("Shutting down...");

                Environment.Exit(1);
            }
        }
    }
}