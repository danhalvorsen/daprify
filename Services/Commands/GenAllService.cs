using Daprify.Models;

namespace Daprify.Services
{
    public class GenAllService(CertificateService certificateService, ComponentService componentService,
                             ComposeService composeService, ConfigService configService,
                             DockerfileService dockerfileService) : IService
    {
        private readonly CertificateService _certificateService = certificateService;
        private readonly ComponentService _componentService = componentService;
        private readonly ComposeService _composeService = composeService;
        private readonly ConfigService _configService = configService;
        private readonly DockerfileService _dockerfileService = dockerfileService;
        private readonly Key _settingKey = new("settings");


        public void Generate(OptionDictionary options)
        {
            options = LoadConfig(options);
            OptionValues settingOpt = options.GetAllPairValues(_settingKey);

            GenerateCerts(options, settingOpt);
            _componentService.Generate(options);
            _dockerfileService.Generate(options);
            _composeService.Generate(options);
            RemoveHttps(options, settingOpt);
            _configService.Generate(options);
        }

        private static OptionDictionary LoadConfig(OptionDictionary options)
        {
            Key configKey = new("config");
            Value? config = options.GetAllPairValues(configKey).GetValues().FirstOrDefault();
            if (config != null)
            {
                options = OptionDictionary.PopulateFromJson(config.ToString());
            }

            return options;
        }

        private void GenerateCerts(OptionDictionary options, OptionValues settingOpt)
        {
            Value mtls = new("mtls");
            bool enableMtls = settingOpt.Contain(mtls);

            if (enableMtls)
            {
                _certificateService.Generate(options);
            }
        }

        private void RemoveHttps(OptionDictionary options, OptionValues settingOpt)
        {
            Value https = new("https");
            bool useHttps = settingOpt.GetValues().Contains(https);
            if (useHttps)
            {
                options.Remove(_settingKey, https);
            }
        }
    }
}