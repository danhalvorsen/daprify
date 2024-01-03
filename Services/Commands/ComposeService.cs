using Daprify.Models;
using Daprify.Templates;
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
        private bool _addMtls = false;
        private int _startPort = 1000;
        private readonly int _nextPort = 500;
        private List<IProject> _projects = [];


        protected override List<string> CreateFiles(OptionDictionary options, IPath workingDir)
        {
            StringBuilder composeBuilder = new();

            GetServicesFromSln(options);
            GetServicesFromOptions(options);

            composeBuilder.Append(GetComposeStart(workingDir));
            AppendServices(options, composeBuilder);
            AppendComponents(options, composeBuilder);
            AppendSentry(composeBuilder);

            string compose = PlaceholderRegex().Replace(composeBuilder.ToString(), string.Empty);
            DirectoryService.WriteFile(workingDir, FILE_NAME, compose);

            return ["docker-compose"];
        }


        private void GetServicesFromSln(OptionDictionary options)
        {
            Key solutionPathKey = new("solution_paths");
            IEnumerable<Value> solutionPathValues = options.GetAllPairValues(solutionPathKey).GetValues();

            if (solutionPathValues != null)
            {
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
                _projects.AddRange(services);
            }
        }


        private string GetComposeStart(IPath workingDir)
        {
            MyPath filepath = MyPath.Combine(workingDir.ToString(), FILE_NAME);
            return File.Exists(filepath.ToString()) ? "\n\n" : _templateFactory.CreateTemplate<ComposeStartTemplate>();
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
            }
        }

        private void AppendComponents(OptionDictionary options, StringBuilder composeBuilder)
        {
            ComposeComponentTemplate composeComponent = _templateFactory.GetTemplateService<ComposeComponentTemplate>();
            composeBuilder.Append(composeComponent.Render(options));
        }

        private void AppendSentry(StringBuilder template)
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
    }
}