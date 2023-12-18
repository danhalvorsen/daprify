using CLI.Models;
using CLI.Templates;

namespace CLI.Services
{
    public class ComposeService(TemplateFactory templateFactory) : CommandService(COMPOSE_NAME)
    {
        protected readonly TemplateFactory _templateFactory = templateFactory;
        private const string COMPOSE_NAME = "Compose";
        private const string FILE_NAME = "docker-compose.yml";
        private const string COMPONENT_OPT = "components";
        private const string SETTING_OPT = "settings";
        private const string SERVICE_OPT = "services";
        private const string SOLUTION_OPT = "solution_path";
        List<string> _services = [];


        protected override List<string> CreateFiles(OptionDictionary options, string workingDir)
        {
            GetServices(options);
            string compose = GetComposeStart(workingDir) + GetServicesComposeSection(_services);

            foreach (string argument in options.GetAllPairValues(COMPONENT_OPT))
            {
                string processedArg = PreProcessArgument(argument);
                compose = GetArgumentTemplate(processedArg, compose);
            }

            foreach (string argument in options.GetAllPairValues(SETTING_OPT))
            {
                compose = ReplaceSettings(argument, compose);
            }

            compose = PlaceholderRegex().Replace(compose, string.Empty);
            DirectoryService.AppendFile(workingDir, FILE_NAME, compose);

            return ["docker-compose"];
        }

        private void GetServices(OptionDictionary options)
        {
            List<string> solutionPaths = options.GetAllPairValues(SOLUTION_OPT);
            SolutionService.GetServicesFromSolution(ref _services, solutionPaths);
            _services.AddRange(options.GetAllPairValues(SERVICE_OPT));

            List<string> servOpt = options.GetAllPairValues(SERVICE_OPT);
            _services = _services.Union(servOpt).ToList();
        }

        private string GetComposeStart(string workingDir)
        {
            string filepath = Path.Combine(workingDir, FILE_NAME);
            return File.Exists(filepath) ? "\n\n" : _templateFactory.CreateTemplate<ComposeStartTemplate>();
        }

        private string GetServicesComposeSection(List<string> services)
        {
            return services.Aggregate(string.Empty, (current, service) => current + AddServiceToCompose(service));
        }

        private string AddServiceToCompose(string service)
        {
            string template = _templateFactory.CreateTemplate<ComposeTemplate>();
            return template.Replace("{service}", service.ToLower());
        }


        private static string PreProcessArgument(string argument)
        {
            return argument.ToLower() switch
            {
                "pubsub" => "rabbitmq",
                "statestore" => "redis",
                _ => argument
            };
        }

        protected override string GetArgumentTemplate(string argument, string template)
        {
            string argTemplate = argument.ToLower() switch
            {
                "dashboard" => _templateFactory.CreateTemplate<DaprDashboardTemplate>(),
                "placement" => _templateFactory.CreateTemplate<PlacementTemplate>(),
                "rabbitmq" => _templateFactory.CreateTemplate<RabbitMqTemplate>(),
                "redis" => _templateFactory.CreateTemplate<RedisTemplate>(),
                "sentry" => _templateFactory.CreateTemplate<SentryTemplate>(),
                "zipkin" => _templateFactory.CreateTemplate<ZipkinTemplate>(),
                _ => null!
            };

            return template + argTemplate;
        }
        private string ReplaceSettings(string argument, string template)
        {
            return argument.ToLower() switch
            {
                "https" => template.Replace("{https}", _templateFactory.CreateTemplate<HttpsTemplate>()),
                "mtls" => template.Replace("{mtls}", _templateFactory.CreateTemplate<MtlsCompTemplate>())
                                  .Replace("{env_file}", _templateFactory.CreateTemplate<EnvTemplate>()),
                _ => template
            };
        }
    }
}