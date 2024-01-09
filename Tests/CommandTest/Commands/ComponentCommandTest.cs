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
    public class TryComponentCommandTests
    {
        private readonly StringWriter _consoleOutput = new();
        private readonly ComponentService _service;
        private readonly ComponentSettings _settings = new();
        private readonly OptionDictionaryValidator _validator;
        private readonly MockServiceProvider _serviceProvider = new();
        private readonly TemplateFactory _templateFactory;
        private readonly DirectoryInfo _startingDir = new(Directory.GetCurrentDirectory());

        private const string FILE_EXT = ".yml", COMPONENTS_DIR = "Dapr/Components";
        private readonly OptionValues arguments = new(new Key("components"), ["bindings", "configstore", "crypto", "lock", "pubsub", "secretstore", "statestore"]);

        public TryComponentCommandTests()
        {
            MyPathValidator myPathValidator = new();
            OptionValuesValidator optionValuesValidator = new();
            _validator = new(myPathValidator, optionValuesValidator);

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
            string[] argument =
            [
                _settings.CommandName,
                ComponentSettings.OptionName[0],
                .. arguments.GetStringEnumerable(),
            ];

            DaprifyCommand<ComponentService, ComponentSettings> sut = new(_service, _settings, _validator);

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