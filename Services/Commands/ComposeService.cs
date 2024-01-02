using CLI.Models;
using CLI.Templates;
using System.Text;

namespace CLI.Services
{
    public class ComposeService(IQuery query, IProjectProvider projectProvider, TemplateFactory templateFactory) : CommandService(DOCKER_NAME)
    {
        protected readonly TemplateFactory _templateFactory = templateFactory;
        private readonly IQuery _query = query;
        private readonly IProjectProvider _projectProvider = projectProvider;
        private const string DOCKER_NAME = "Docker";
        private const string FILE_NAME = "docker-compose.yml";
        private readonly Key _componentKey = new("components");
        private readonly Key _settingKey = new("settings");
        private readonly Key _serviceKey = new("services");
        private readonly Key _solutionPathKey = new("solution_paths");
        private bool _addMtls = false;
        private int _startPort = 1000;
        private readonly int _nextPort = 500;
        private static readonly List<string> _components = [];
        private List<IProject> _projects = [];


        protected override List<string> CreateFiles(OptionDictionary options, IPath workingDir)
        {
            GetServices(options);

            string compose = GetComposeStart(workingDir) + GetServicesComposeSection(options);
            compose = AddComponents(options, compose);
            compose = AddDependsOn(compose);

            compose = PlaceholderRegex().Replace(compose, string.Empty);
            DirectoryService.AppendFile(workingDir, FILE_NAME, compose);

            return ["docker-compose"];
        }


        private void GetServices(OptionDictionary options)
        {
            IEnumerable<Value> solutionPathValues = options.GetAllPairValues(_solutionPathKey).GetValues();
            if (solutionPathValues != null)
            {
                IEnumerable<MyPath>? solutionPaths = MyPath.FromStringList(solutionPathValues);
                IEnumerable<Solution> solutions = solutionPaths.Select(path => new Solution(_query, _projectProvider, path));
                _projects = SolutionService.GetDaprServicesFromSln(new(string.Empty), solutions).ToList();
            }

            IEnumerable<Project> services = Project.FromStringList(options.GetAllPairValues(_serviceKey).GetValues());
            _projects.AddRange(services);
        }


        private string GetComposeStart(IPath workingDir)
        {
            MyPath filepath = MyPath.Combine(workingDir.ToString(), FILE_NAME);
            return File.Exists(filepath.ToString()) ? "\n\n" : _templateFactory.CreateTemplate<ComposeStartTemplate>();
        }


        private string GetServicesComposeSection(OptionDictionary options)
        {
            StringBuilder composeBuilder = new();
            foreach (IProject project in _projects)
            {
                composeBuilder.Append(AddServiceToCompose(project.GetName()));
                composeBuilder.Append(AddDaprServiceToCompose(project.GetName()));

                foreach (string argument in options.GetAllPairValues(_settingKey).GetStringEnumerable())
                {
                    ReplaceSettings(argument, composeBuilder);
                }
                composeBuilder.Replace("{{port}}", SetServicePort());
            }
            CheckMtls(composeBuilder);
            return composeBuilder.ToString();
        }

        private string AddServiceToCompose(Name service)
        {
            string serviceTemp = _templateFactory.CreateTemplate<ComposeServiceTemplate>();
            return serviceTemp.Replace("{{service}}", service.ToString().ToLower())
                              .Replace("{{Service}}", service.ToString());
        }


        private string AddDaprServiceToCompose(Name service)
        {
            string daprTemp = _templateFactory.CreateTemplate<ComposeDaprTemplate>();
            return daprTemp.Replace("{{service}}", service.ToString().ToLower());
        }


        private void ReplaceSettings(string argument, StringBuilder template)
        {
            _ = argument.ToLower() switch
            {
                "https" => ReplaceHttps(template),
                "mtls" => ReplaceMtls(template),
                _ => null!
            };
        }


        private string AddComponents(OptionDictionary options, string compose)
        {
            OptionValues componentOpt = options.GetAllPairValues(_componentKey);
            foreach (Value argument in componentOpt.GetValues())
            {
                Value processedArg = PreProcessArgument(argument);
                compose = GetArgumentTemplate(processedArg, compose);
            }
            return compose;
        }


        private static string AddDependsOn(string compose)
        {
            const string INDENT = "    ";
            StringBuilder dependsOnBuilder = new();
            dependsOnBuilder.AppendLine("depends_on:");

            _components.ForEach(component => dependsOnBuilder.AppendLine($"{INDENT}  - {component}"));

            return compose.Replace("{{depends_on}}", dependsOnBuilder.ToString());
        }


        private static Value PreProcessArgument(Value argument)
        {
            return argument.ToString().ToLower() switch
            {
                "pubsub" => new("rabbitmq"),
                "statestore" => new("redis"),
                _ => argument
            };
        }


        protected override string GetArgumentTemplate(Value argument, string template)
        {
            string argTemplate = argument.ToString().ToLower() switch
            {
                "dashboard" => _templateFactory.CreateTemplate<DaprDashboardTemplate>(),
                "placement" => _templateFactory.CreateTemplate<PlacementTemplate>(),
                "rabbitmq" => _templateFactory.CreateTemplate<RabbitMqTemplate>(),
                "redis" => _templateFactory.CreateTemplate<RedisTemplate>(),
                "sentry" => _templateFactory.CreateTemplate<SentryTemplate>(),
                "zipkin" => _templateFactory.CreateTemplate<ZipkinTemplate>(),
                _ => null!
            };

            if (argTemplate != null)
            {
                _components.Add(argument.ToString());
            }

            return template + argTemplate;
        }

        private void CheckMtls(StringBuilder template)
        {
            if (_addMtls)
            {
                template.Append(_templateFactory.CreateTemplate<SentryTemplate>());
                _addMtls = false;
            }
        }

        private string SetServicePort()
        {
            int port = _startPort;
            _startPort += _nextPort; // Increment the service port by 500
            return port.ToString();
        }

        private StringBuilder ReplaceHttps(StringBuilder template)
        {
            return template.Replace("{{dapr-https}}", _templateFactory.CreateTemplate<HttpsDaprTemplate>())
                           .Replace("{{https}}", _templateFactory.CreateTemplate<HttpsServiceTemplate>());
        }

        private StringBuilder ReplaceMtls(StringBuilder template)
        {
            _addMtls = true;
            return template.Replace("{{mtls}}", _templateFactory.CreateTemplate<MtlsCompTemplate>())
                           .Replace("{{env_file}}", _templateFactory.CreateTemplate<EnvTemplate>());
        }
    }
}