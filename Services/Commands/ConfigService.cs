using Daprify.Models;
using Daprify.Templates;

namespace Daprify.Services
{
    public class ConfigService(TemplateFactory templateFactory) : CommandService(CONFIG_NAME)
    {
        protected readonly TemplateFactory _templateFactory = templateFactory;
        private const string CONFIG_NAME = "Config";
        private const string CONFIG_YAML = "config.yaml";


        protected override List<string> CreateFiles(OptionDictionary options, IPath workingDir)
        {
            Key settingKey = new("settings");
            ConfigTemplate configService = _templateFactory.GetTemplateService<ConfigTemplate>();
            OptionValues settingOpt = options.GetAllPairValues(settingKey);

            string config = configService.Render(settingOpt);
            config = PlaceholderRegex().Replace(config, string.Empty);

            DirectoryService.WriteFile(workingDir, CONFIG_YAML, config);
            return ["config"];
        }
    }
}