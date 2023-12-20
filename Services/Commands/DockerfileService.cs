using CLI.Models;
using CLI.Templates;

namespace CLI.Services
{
    public class DockerfileService(TemplateFactory templateFactory) : CommandService(DOCKER_NAME)
    {
        private readonly TemplateFactory _templateFactory = templateFactory;
        private const string DOCKER_NAME = "Docker";
        private const string PROJECT_OPT = "project_path";
        private const string SOLUTION_OPT = "solution_path";
        private const string DOCKERFILE_EXT = ".Dockerfile";
        List<string> _services = [];
        List<string> _projectPaths = [];


        protected override List<string> CreateFiles(OptionDictionary options, string workingDir)
        {
            GetServices(options);
            DockerfileTemplate dockerfileTemp = GetDockerfile();
            GenDockerFile(dockerfileTemp, workingDir);

            return ["Dockerfile"];
        }

        private void GenDockerFile(DockerfileTemplate dockerfileTemp, string workingDir)
        {
            for (int i = 0; i < _services.Count; i++)
            {
                string dockerfile = dockerfileTemp.Render(_projectPaths[i], _services[i]);
                DirectoryService.WriteFile(workingDir, _services[i] + DOCKERFILE_EXT, dockerfile);
            }
        }


        private DockerfileTemplate GetDockerfile()
        {
            return _templateFactory.GetTemplateService<DockerfileTemplate>();
        }


        private void GetServices(OptionDictionary options)
        {
            List<string> projectPathOpt = options.GetAllPairValues(PROJECT_OPT);
            string projectRoot = projectPathOpt.Count > 0 ? projectPathOpt[0] : string.Empty;
            List<string> solutionPaths = options.GetAllPairValues(SOLUTION_OPT);
            _projectPaths = SolutionService.GetDaprServicesFromSln(ref _services, projectRoot, solutionPaths);
        }
    }
}