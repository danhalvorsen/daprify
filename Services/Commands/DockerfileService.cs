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
            MyPath projectRoot = GetProjectRoot(options);
            GetSolutions(options, projectRoot);
        }

        private static MyPath GetProjectRoot(OptionDictionary options)
        {
            Log.Verbose("Getting project root for Dockerfile generation.");

            Key projectPathKey = new("project_path");
            var projectPathOpt = options.GetAllPairValues(projectPathKey).GetValues();

            MyPath projectRoot = new(string.Empty);
            if (projectPathOpt != null && projectPathOpt.Count() > 1)
            {
                projectRoot = new(projectPathOpt.First());
            }

            Log.Verbose("Project root determined: {ProjectRoot}", projectRoot.ToString() == string.Empty ? "Could not find any project root!" : projectRoot);
            return projectRoot;
        }

        private void GetSolutions(OptionDictionary options, MyPath projectRoot)
        {
            Log.Verbose("Retrieving solutions for Dockerfile generation.");
            Key solutionKey = new("solution_paths");
            var solutionOpt = options.GetAllPairValues(solutionKey).GetValues();

            if (solutionOpt != null)
            {
                Log.Verbose("Searching for solutions...");

                IEnumerable<MyPath> solutionPaths = MyPath.FromStringList(solutionOpt);
                IEnumerable<Solution> solutions = solutionPaths.Select(path => new Solution(_query, _projectProvider, path));

                IEnumerable<IProject> projects = SolutionService.GetDaprServicesFromSln(projectRoot, solutions);
                _projects.AddRange(projects);
            }
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
            Log.Verbose("Getting Dockerfile template.");
            DockerfileTemplate dockerfileTemplate = _templateFactory.GetTemplateService<DockerfileTemplate>();
            Log.Verbose("Dockerfile template retrieved.");
            return dockerfileTemplate;
        }
    }
}