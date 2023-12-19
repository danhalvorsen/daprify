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
    public class TryConfigCommandTests
    {
        private readonly StringWriter _consoleOutput = new();
        private readonly ConfigService _service;
        private readonly ConfigSettings _settings = new();

        private readonly ConfigValidator _validator = new();
        private readonly TemplateFactory _templateFactory;
        private readonly DirectoryInfo _startingDir = new(Directory.GetCurrentDirectory());

        private const string EXPECTED_FILENAME = "config.yaml", DAPR_DIR = "Dapr", CONFIG_FILE = "config.yaml", CONFIG_DIR = "Config/";
        private readonly List<string> arguments = ["mtls", "tracing",];

        public TryConfigCommandTests()
        {
            MockServiceProvider _serviceProvider = new();
            _templateFactory = new(_serviceProvider.Object);
            _service = new(_templateFactory);
            Console.SetOut(_consoleOutput);

            Directory.SetCurrentDirectory(DirectoryService.CreateTempDirectory());
            Environment.SetEnvironmentVariable("isTestProject", "true");
        }


        [TestMethod]
        public void Expected_Config_Generated()
        {
            // Arrange
            string[] argument = [_settings.CommandName];
            CLICommand<ConfigService, ConfigSettings, ConfigValidator> sut = new(_service, _settings, _validator);

            // Act
            sut.Parse(argument).Invoke();

            // Assert
            Directory.SetCurrentDirectory(Path.Combine(Directory.GetCurrentDirectory(), DAPR_DIR));
            string consoleOutput = _consoleOutput.ToString();

            consoleOutput.Should().Contain("config");
            File.Exists(CONFIG_DIR + CONFIG_FILE).Should().BeTrue($"File {CONFIG_FILE} should exist but was not found.");
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