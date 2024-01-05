using Daprify.Models;
using Daprify.Templates;
using Serilog;

namespace Daprify.Services
{
    public class DockerfileService(IQuery query, IProjectProvider projectProvider, TemplateFactory templateFactory) : CommandService(DOCKER_NAME)
    {
        private readonly TemplateFactory _templateFactory = templateFactory;
        private readonly IProjectProvider _projectProvider = projectProvider;
        private const string DOCKER_NAME = "Docker";
        private const string DOCKERFILE_EXT = ".Dockerfile";
        private readonly IQuery _query = query;
        private readonly List<IProject> _projects = [];


        protected override List<string> CreateFiles(OptionDictionary options, IPath workingDir)
        {
            GetServices(options);
            DockerfileTemplate dockerfileTemp = GetDockerfile();
            GenDockerFile(dockerfileTemp, workingDir);

            return _projects.Select(project => project.GetName() + DOCKERFILE_EXT).ToList();
        }

        private void GetServices(OptionDictionary options)
        {
            MyPath projectRoot = CheckRootPath(options);
            _projects.AddRange(GetServicesFromSln(options, _query, _projectProvider, projectRoot));
            _projects.AddRange(GetServicesFromOptions(options));
        }

        private void GenDockerFile(DockerfileTemplate dockerfileTemp, IPath workingDir)
        {
            foreach (IProject project in _projects)
            {
                string dockerfile = dockerfileTemp.Render(project);
                DirectoryService.WriteFile(workingDir, project.GetName() + DOCKERFILE_EXT, dockerfile);
            }
        }

        private DockerfileTemplate GetDockerfile()
        {
            Log.Verbose("Getting Dockerfile template.");
            DockerfileTemplate dockerfileTemplate = _templateFactory.GetTemplateService<DockerfileTemplate>();
            Log.Verbose("Dockerfile template retrieved.");
            return dockerfileTemplate;
        }
    }
}