using CLI.Models;
using CLI.Templates;

namespace CLI.Services
{
    public class DockerfileService(IQuery query, IProjectProvider projectProvider, TemplateFactory templateFactory) : CommandService(DOCKER_NAME)
    {
        private readonly TemplateFactory _templateFactory = templateFactory;
        private readonly IProjectProvider _projectProvider = projectProvider;
        private const string DOCKER_NAME = "Docker";
        private readonly Key _projectPathKey = new("project_path");
        private readonly Key _solutionKey = new("solution_paths");
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

        private void GetServices(OptionDictionary options)
        {
            OptionValues projectPathOpt = options.GetAllPairValues(_projectPathKey);
            MyPath projectRoot = projectPathOpt.Count() > 0 ? new(projectPathOpt.GetStringEnumerable().First()) : new(string.Empty);

            IEnumerable<MyPath> solutionPaths = MyPath.FromStringList(options.GetAllPairValues(_solutionKey).GetValues());
            IEnumerable<Solution> solutions = solutionPaths.Select(path => new Solution(_query, _projectProvider, path));

            _projects = SolutionService.GetDaprServicesFromSln(projectRoot, solutions);
        }
    }
}