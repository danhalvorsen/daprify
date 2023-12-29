using CLI.Models;
using CLI.Templates;

namespace CLI.Services
{
    public class ConfigService(TemplateFactory templateFactory) : CommandService(CONFIG_NAME)
    {
        protected readonly TemplateFactory _templateFactory = templateFactory;
        private const string CONFIG_NAME = "Config";
        private const string SETTING_OPT = "settings";
        private const string CONFIG_YAML = "config.yaml";


        protected override List<string> CreateFiles(OptionDictionary options, IPath workingDir)
        {
            string config = _templateFactory.CreateTemplate<ConfigTemplate>();
            OptionValues settingOpt = options.GetAllPairValues(SETTING_OPT);

            foreach (string argument in settingOpt.GetValues())
            {
                config = GetArgumentTemplate(argument, config);
            }

            config = PlaceholderRegex().Replace(config, string.Empty);
            DirectoryService.WriteFile(workingDir, CONFIG_YAML, config);
            return ["config"];
        }

        protected override string GetArgumentTemplate(string argument, string template)
        {
            string argTemplate = argument.ToLower() switch
            {
                "logging" => _templateFactory.CreateTemplate<LoggingTemplate>(),
                "metric" => _templateFactory.CreateTemplate<MetricTemplate>(),
                "middleware" => _templateFactory.CreateTemplate<MiddlewareTemplate>(),
                "mtls" => _templateFactory.CreateTemplate<MTlsTemplate>(),
                "tracing" => _templateFactory.CreateTemplate<TracingTemplate>(),
                _ => throw new ArgumentException("Invalid setting name:" + argument, nameof(argument))
            };

            return template.Replace($"{{{{{argument}}}}}", argTemplate);
        }
    }
}