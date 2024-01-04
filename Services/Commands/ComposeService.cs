using Daprify.Models;
using Daprify.Templates;
using Serilog;
using System.Text;

namespace Daprify.Services
{
    public class ComposeService(IQuery query, IProjectProvider projectProvider, TemplateFactory templateFactory) : CommandService(DOCKER_NAME)
    {
        protected readonly TemplateFactory _templateFactory = templateFactory;
        private readonly IQuery _query = query;
        private readonly IProjectProvider _projectProvider = projectProvider;
        private const string DOCKER_NAME = "Docker";
        private const string FILE_NAME = "docker-compose.yml";
        private int _startPort = 1000;
        private readonly int _nextPort = 500;
        private readonly List<IProject> _projects = [];

        protected override List<string> CreateFiles(OptionDictionary options, IPath workingDir)
        {
            StringBuilder composeBuilder = new();

            GetServicesFromSln(options);
            GetServicesFromOptions(options);

            composeBuilder.Append(GetComposeStart(workingDir));
            AppendServices(options, composeBuilder);
            AppendComponents(options, composeBuilder);
            AppendSentry(options, composeBuilder);

            string compose = RemovePlaceholders(composeBuilder);
            DirectoryService.AppendFile(workingDir, FILE_NAME, compose);

            return [FILE_NAME];
        }

        private static string RemovePlaceholders(StringBuilder composeBuilder)
        {
            Log.Verbose("Removing placeholders from the template...");
            return PlaceholderRegex().Replace(composeBuilder.ToString(), string.Empty);
        }

        private void GetServicesFromSln(OptionDictionary options)
        {
            Key solutionPathKey = new("solution_paths");
            IEnumerable<Value> solutionPathValues = options.GetAllPairValues(solutionPathKey).GetValues();

            if (solutionPathValues != null)
            {
                Log.Verbose("Searching for solutions...");

                IEnumerable<MyPath> solutionPaths = MyPath.FromStringList(solutionPathValues);
                IEnumerable<Solution> solutions = solutionPaths.Select(path => new Solution(_query, _projectProvider, path));

                IEnumerable<IProject> projects = SolutionService.GetDaprServicesFromSln(new(string.Empty), solutions);
                _projects.AddRange(projects);
            }
        }


        private void GetServicesFromOptions(OptionDictionary options)
        {
            Key serviceKey = new("services");
            IEnumerable<Value>? serviceValues = options.GetAllPairValues(serviceKey).GetValues();
            if (serviceValues != null)
            {
                IEnumerable<IProject> services = Project.FromStringList(serviceValues);
                Log.Verbose("Found services from service option: {services}", services.Select(service => service.GetName()));
                _projects.AddRange(services);
            }
        }


        private string GetComposeStart(IPath workingDir)
        {
            MyPath filepath = MyPath.Combine(workingDir.ToString(), FILE_NAME);
            if (File.Exists(filepath.ToString()))
            {
                Log.Verbose("Found existing docker-compose.yml file, appending to it...");
                return "\n\n";
            }
            else
            {
                Log.Verbose("No existing docker-compose.yml file found, creating new one...");
                return _templateFactory.CreateTemplate<ComposeStartTemplate>();
            }
        }


        private void AppendServices(OptionDictionary options, StringBuilder composeBuilder)
        {
            ComposeServiceTemplate composeService = _templateFactory.GetTemplateService<ComposeServiceTemplate>();
            ComposeDaprTemplate composeDapr = _templateFactory.GetTemplateService<ComposeDaprTemplate>();

            foreach (IProject project in _projects)
            {
                string port = SetServicePort();
                composeBuilder.Append(composeService.Render(options, project, port));
                composeBuilder.Append(composeDapr.Render(options, project, port));
                Log.Verbose("Successfully added templates for service: {service}", project.GetName());
            }
        }

        private void AppendComponents(OptionDictionary options, StringBuilder composeBuilder)
        {
            ComposeComponentTemplate composeComponent = _templateFactory.GetTemplateService<ComposeComponentTemplate>();
            composeBuilder.Append(composeComponent.Render(options));
            Log.Verbose("Successfully added templates for components");
        }

        private void AppendSentry(OptionDictionary options, StringBuilder template)
        {
            Log.Verbose("Checking if mtls is given as input...");
            Key sentryKey = new("settings");
            OptionValues settings = options.GetAllPairValues(sentryKey);
            if (settings.GetValues() != null && settings.GetStringEnumerable().Contains("mtls"))
            {
                Log.Verbose("mtls is given as input, adding sentry template...");
                template.Append(_templateFactory.CreateTemplate<SentryTemplate>());
            }
        }

        private string SetServicePort()
        {
            int port = _startPort;
            _startPort += _nextPort; // Increment the service port by 500
            return port.ToString();
        }
    }
}