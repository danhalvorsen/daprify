using CLI.Commands;
using CLI.Services;
using CLI.Settings;
using CLI.Templates;
using CLI.Validation;
using FluentAssertions;
using System.CommandLine;
using System.CommandLine.Parsing;

namespace CLITests.Commands
{
    [TestClass]
    public class TryComposeCommandTests
    {
        private readonly StringWriter _consoleOutput = new();
        private readonly ComposeService _service;
        private readonly ComposeSettings _settings = new();

        private readonly ComposeValidator _validator = new();
        private readonly TemplateFactory _templateFactory;
        private readonly DirectoryInfo _startingDir = new(Directory.GetCurrentDirectory());

        private const string EXPECTED_FILENAME = "docker-compose.yml", DOCKER_DIR = "Dapr/Docker";
        private readonly List<string> serviceArguments = ["ServiceA", "ServiceB",];
        private readonly List<string> componentArguments = ["rabbitmq", "redis",];

        public TryComposeCommandTests()
        {
            MockServiceProvider _serviceProvider = new();
            _templateFactory = new(_serviceProvider.Object);
            _service = new(_templateFactory);
            Console.SetOut(_consoleOutput);

            Directory.SetCurrentDirectory(DirectoryService.CreateTempDirectory());
            Environment.SetEnvironmentVariable("isTestProject", "true");
        }


        [TestMethod]
        public void Expected_Composes_Generated()
        {
            // Arrange
            string[] argument = [_settings.CommandName, ComposeSettings.ServiceOptionName[0], serviceArguments[0], serviceArguments[1],
                                                        ComposeSettings.ComponentOptionName[0], componentArguments[0], componentArguments[1]];
            CLICommand<ComposeService, ComposeSettings, ComposeValidator> sut = new(_service, _settings, _validator);

            // Act
            sut.Parse(argument).Invoke();

            // Assert
            string filepath = Path.Combine(Directory.GetCurrentDirectory(), DOCKER_DIR);
            Directory.Exists(filepath).Should().BeTrue($"Directory {filepath} should exist but was not found.");

            Directory.SetCurrentDirectory(filepath);
            File.Exists(EXPECTED_FILENAME).Should().BeTrue($"File {EXPECTED_FILENAME} should exist but was not found.");
        }


        [TestCleanup]
        public void Cleanup()
        {
            _consoleOutput.GetStringBuilder().Clear();
            DirectoryService.DeleteDirectory(Directory.GetCurrentDirectory());
            Directory.SetCurrentDirectory(_startingDir.FullName);
        }
    }
}