using Daprify.Commands;
using Daprify.Models;
using Daprify.Services;
using Daprify.Settings;
using Daprify.Templates;
using Daprify.Validation;
using DaprifyTests.Mocks;
using FluentAssertions;
using System.CommandLine;
using System.CommandLine.Parsing;

namespace DaprifyTests.Commands
{
    [TestClass]
    public class TryConfigCommandTests
    {
        private readonly StringWriter _consoleOutput = new();
        private readonly ConfigService _service;
        private readonly ConfigSettings _settings = new();

        private readonly OptionValidatorFactory _optionValidatorFactory;
        private readonly TemplateFactory _templateFactory;
        private readonly DirectoryInfo _startingDir = new(Directory.GetCurrentDirectory());

        private const string EXPECTED_FILENAME = "config.yaml", DAPR_DIR = "Dapr", CONFIG_FILE = "config.yml", CONFIG_DIR = "Config/";
        private readonly List<string> arguments = ["mtls", "tracing",];

        public TryConfigCommandTests()
        {
            MyPathValidator myPathValidator = new();
            _optionValidatorFactory = new(myPathValidator);

            MockServiceProvider _serviceProvider = new();
            _templateFactory = new(_serviceProvider.Object);
            _service = new(_templateFactory);
            Console.SetOut(_consoleOutput);

            Directory.SetCurrentDirectory(DirectoryService.CreateTempDirectory().ToString());
            Environment.SetEnvironmentVariable("isTestProject", "true");
        }


        [TestMethod]
        public void Expected_Config_Generated()
        {
            // Arrange
            string[] argument = [_settings.CommandName];
            DaprifyCommand<ConfigService, ConfigSettings> sut = new(_service, _settings, _optionValidatorFactory);

            // Act
            sut.Parse(argument).Invoke();

            // Assert
            Directory.SetCurrentDirectory(MyPath.Combine(DirectoryService.GetCurrentDirectory().ToString(), DAPR_DIR).ToString());
            string consoleOutput = _consoleOutput.ToString();

            consoleOutput.Should().Contain("config");
            File.Exists(CONFIG_DIR + CONFIG_FILE).Should().BeTrue($"File {CONFIG_FILE} should exist but was not found.");
        }


        [TestCleanup]
        public void Cleanup()
        {
            _consoleOutput.GetStringBuilder().Clear();
            DirectoryService.DeleteDirectory(DirectoryService.GetCurrentDirectory());
            Directory.SetCurrentDirectory(_startingDir.FullName);
        }
    }
}