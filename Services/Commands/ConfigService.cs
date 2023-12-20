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


        protected override List<string> CreateFiles(OptionDictionary options, string workingDir)
        {
            string config = _templateFactory.CreateTemplate<ConfigTemplate>();
            List<string> settingOpt = options.GetAllPairValues(SETTING_OPT);

            foreach (string argument in settingOpt)
            {
                config = GetArgumentTemplate(argument, config);
            }

            config = PlaceholderRegex().Replace(config, string.Empty);

            string filePath = Path.Combine(workingDir, CONFIG_YAML);
            File.WriteAllText(filePath, config);
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