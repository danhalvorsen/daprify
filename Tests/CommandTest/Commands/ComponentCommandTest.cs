using CLI.Commands;
using CLI.Services;
using CLI.Settings;
using CLI.Templates;
using CLI.Validation;
using CLITests.Mocks;
using FluentAssertions;
using System.CommandLine;
using System.CommandLine.Parsing;

namespace CLITests.Commands
{
    [TestClass]
    public class TryComponentCommandTests
    {
        private readonly StringWriter _consoleOutput = new();
        private readonly ComponentService _service;
        private readonly ComponentSettings _settings = new();
        private readonly ComponentValidator _validator = new();
        private readonly MockServiceProvider _serviceProvider = new();
        private readonly TemplateFactory _templateFactory;
        private readonly DirectoryInfo _startingDir = new(Directory.GetCurrentDirectory());

        private const string FILE_EXT = ".yaml", COMPONENTS_DIR = "Dapr/Components";
        private readonly List<string> arguments = ["pubsub", "statestore",];

        public TryComponentCommandTests()
        {
            _templateFactory = new(_serviceProvider.Object);
            _service = new(_templateFactory);
            Console.SetOut(_consoleOutput);

            Directory.SetCurrentDirectory(DirectoryService.CreateTempDirectory());
            Environment.SetEnvironmentVariable("isTestProject", "true");
        }


        [TestMethod]
        public void Expected_Components_Generated()
        {
            // Arrange
            string[] argument = [_settings.CommandName, ComponentSettings.OptionName[0], arguments[0], arguments[1]];
            CLICommand<ComponentService, ComponentSettings, ComponentValidator> sut = new(_service, _settings, _validator);

            // Act
            sut.Parse(argument).Invoke();

            // Assert
            string consoleOutput = _consoleOutput.ToString();
            Directory.SetCurrentDirectory(Path.Combine(Directory.GetCurrentDirectory(), COMPONENTS_DIR));

            foreach (string arg in arguments)
            {
                consoleOutput.Should().Contain(arg);
                File.Exists(arg + FILE_EXT).Should().BeTrue($"File {arg} should exist but was not found.");
            }
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