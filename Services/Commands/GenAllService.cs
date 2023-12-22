using CLI.Models;

namespace CLI.Services
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
        private const string CONFIG_OPT = "config";
        private const string SETTING_OPT = "settings";
        private const string MTLS = "mtls";
        private const string HTTPS = "https";


        public void Generate(OptionDictionary options)
        {
            options = LoadConfig(options);
            List<string> settingOpt = options.GetAllPairValues(SETTING_OPT);

            GenerateCerts(options, settingOpt);
            _componentService.Generate(options);
            _dockerfileService.Generate(options);
            _composeService.Generate(options);
            RemoveHttps(options, settingOpt);
            _configService.Generate(options);
        }

        private static OptionDictionary LoadConfig(OptionDictionary options)
        {
            string? confPath = options.GetAllPairValues(CONFIG_OPT).FirstOrDefault();
            if (!string.IsNullOrEmpty(confPath))
            {
                options = OptionDictionary.PopulateFromJson(confPath);
            }

            return options;
        }

        private void GenerateCerts(OptionDictionary options, List<string> settingOpt)
        {
            bool enableMtls = settingOpt.Contains(MTLS);

            if (enableMtls)
            {
                _certificateService.Generate(options);
            }
        }

        private static void RemoveHttps(OptionDictionary options, List<string> settingOpt)
        {
            bool useHttps = settingOpt.Contains(HTTPS);
            if (useHttps)
            {
                options.Remove(SETTING_OPT, HTTPS);
            }
        }
    }
}