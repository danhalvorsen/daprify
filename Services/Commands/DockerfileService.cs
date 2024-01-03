using Daprify.Models;
using Daprify.Templates;

namespace Daprify.Services
{
    public class DockerfileService(IQuery query, IProjectProvider projectProvider, TemplateFactory templateFactory) : CommandService(DOCKER_NAME)
    {
        private readonly TemplateFactory _templateFactory = templateFactory;
        private readonly IProjectProvider _projectProvider = projectProvider;
        private const string DOCKER_NAME = "Docker";
        private const string DOCKERFILE_EXT = ".Dockerfile";
        private readonly IQuery _query = query;
        IEnumerable<IProject> _projects = [];


        protected override List<string> CreateFiles(OptionDictionary options, IPath workingDir)
        {
            GetServices(options);
            DockerfileTemplate dockerfileTemp = GetDockerfile();
            GenDockerFile(dockerfileTemp, workingDir);

            return ["Dockerfile"];
        }

        private void GetServices(OptionDictionary options)
        {
            MyPath projectRoot = GetProjectRoot(options);
            IEnumerable<Solution> solutions = GetSolutions(options);

            _projects = SolutionService.GetDaprServicesFromSln(projectRoot, solutions);
        }

        private static MyPath GetProjectRoot(OptionDictionary options)
        {
            Key projectPathKey = new("project_path");
            OptionValues projectPathOpt = options.GetAllPairValues(projectPathKey);
            MyPath projectRoot = projectPathOpt.Count() > 0 ? new(projectPathOpt.GetStringEnumerable().First()) : new(string.Empty);

            return projectRoot;
        }

        private IEnumerable<Solution> GetSolutions(OptionDictionary options)
        {
            Key solutionKey = new("solution_paths");
            IEnumerable<MyPath> solutionPaths = MyPath.FromStringList(options.GetAllPairValues(solutionKey).GetValues());
            IEnumerable<Solution> solutions = solutionPaths.Select(path => new Solution(_query, _projectProvider, path));

            return solutions;
        }

        private void GenDockerFile(DockerfileTemplate dockerfileTemp, IPath workingDir)
        {
            foreach (IProject project in _projects)
            {
                string dockerfile = dockerfileTemp.Render(project.GetRelativeProjPath(), project.GetName());
                DirectoryService.WriteFile(workingDir, project.GetName() + DOCKERFILE_EXT, dockerfile);
            }
        }

        private DockerfileTemplate GetDockerfile()
        {
            return _templateFactory.GetTemplateService<DockerfileTemplate>();
        }
    }
}