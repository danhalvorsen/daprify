using CLI.Commands;
using CLI.Models;
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
        private readonly OptionValidatorFactory _optionValidatorFactory;
        private readonly MockServiceProvider _serviceProvider = new();
        private readonly TemplateFactory _templateFactory;
        private readonly DirectoryInfo _startingDir = new(Directory.GetCurrentDirectory());

        private const string FILE_EXT = ".yaml", COMPONENTS_DIR = "Dapr/Components";
        private readonly OptionValues arguments = new(["pubsub", "statestore"]);

        public TryComponentCommandTests()
        {
            MyPathValidator myPathValidator = new();
            _optionValidatorFactory = new(myPathValidator);

            _templateFactory = new(_serviceProvider.Object);
            _service = new(_templateFactory);
            Console.SetOut(_consoleOutput);

            Directory.SetCurrentDirectory(DirectoryService.CreateTempDirectory().ToString());
            Environment.SetEnvironmentVariable("isTestProject", "true");
        }


        [TestMethod]
        public void Expected_Components_Generated()
        {
            // Arrange
            string[] argument = [_settings.CommandName, ComponentSettings.OptionName[0],
                                 arguments.GetValues().ElementAt(0).ToString(),
                                 arguments.GetValues().ElementAt(1).ToString()];
            CLICommand<ComponentService, ComponentSettings> sut = new(_service, _settings, _optionValidatorFactory);

            // Act
            sut.Parse(argument).Invoke();

            // Assert
            string consoleOutput = _consoleOutput.ToString();
            Directory.SetCurrentDirectory(MyPath.Combine(DirectoryService.GetCurrentDirectory().ToString(), COMPONENTS_DIR).ToString());

            foreach (string arg in arguments.GetStringEnumerable())
            {
                consoleOutput.Should().Contain(arg);
                File.Exists(arg + FILE_EXT).Should().BeTrue($"File {arg} should exist but was not found.");
            }
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