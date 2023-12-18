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

        private const string EXPECTED_FILENAME = "config.yaml", CONFIG_DIR = "Dapr/Config";
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
        public void Expected_Configs_Generated()
        {
            // Arrange
            string[] argument = [_settings.CommandName, ConfigSettings.OptionName[0], arguments[0], arguments[1]];
            CLICommand<ConfigService, ConfigSettings, ConfigValidator> sut = new(_service, _settings, _validator);

            // Act
            sut.Parse(argument).Invoke();

            // Assert
            string filepath = Path.Combine(Directory.GetCurrentDirectory(), CONFIG_DIR);
            Directory.Exists(filepath).Should().BeTrue($"Directory {filepath} should exist but was not found.");
            Directory.SetCurrentDirectory(filepath);

            File.Exists(EXPECTED_FILENAME).Should().BeTrue($"File {EXPECTED_FILENAME} should exist but was not found.");
        }


        [TestCleanup]
        public void Cleanup()
        {
            if (File.Exists(EXPECTED_FILENAME))
            {
                File.Delete(EXPECTED_FILENAME);
            }

            DirectoryService.DeleteDirectory(Directory.GetCurrentDirectory());
            Directory.SetCurrentDirectory(_startingDir.FullName);
        }
    }
}