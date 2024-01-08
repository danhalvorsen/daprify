using Daprify.Models;
using Serilog;

namespace Daprify.Services
{
    public class GenAllService(CertificateService certificateService, ComponentService componentService,
                             ComposeService composeService, ConfigService configService,
                             DockerfileService dockerfileService) : CommandService("GenAll")
    {
        private readonly CertificateService _certificateService = certificateService;
        private readonly ComponentService _componentService = componentService;
        private readonly ComposeService _composeService = composeService;
        private readonly ConfigService _configService = configService;
        private readonly DockerfileService _dockerfileService = dockerfileService;
        private readonly Key _settingKey = new("settings");


        public override void Generate(OptionDictionary options)
        {
            PrintMessage("start");
            OptionValues settingOpt = options.GetAllPairValues(_settingKey);

            GenerateCerts(options, settingOpt);
            _componentService.Generate(options);
            _dockerfileService.Generate(options);
            _composeService.Generate(options);
            RemoveHttps(options, settingOpt);
            _configService.Generate(options);

            PrintMessage("success");
        }

        private void GenerateCerts(OptionDictionary options, OptionValues settingOpt)
        {
            Value mtls = new("mtls");
            var enableMtls = settingOpt.GetValues();

            if (enableMtls != null && enableMtls.Contains(mtls))
            {
                _certificateService.Generate(options);
            }
        }

        private void RemoveHttps(OptionDictionary options, OptionValues settingOpt)
        {
            Value https = new("https");
            var useHttps = settingOpt.GetValues();
            if (useHttps != null && useHttps.Contains(https))
            {
                options.Remove(_settingKey, https);
            }
        }
    }
}