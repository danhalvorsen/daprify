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
    public class TryComposeCommandTests
    {
        private readonly StringWriter _consoleOutput = new();
        private readonly ComposeService _service;
        private readonly ComposeSettings _settings = new();

        private readonly OptionDictionaryValidator _validator;
        private readonly TemplateFactory _templateFactory;
        private readonly DirectoryInfo _startingDir = new(Directory.GetCurrentDirectory());

        private const string EXPECTED_FILENAME = "docker-compose.yml", DOCKER_DIR = "Dapr/Docker";
        private readonly List<string> serviceArguments = ["ServiceA", "ServiceB",];
        private readonly List<string> componentArguments = ["dashboard", "placement", "rabbitmq", "redis", "sentry", "zipkin"];

        public TryComposeCommandTests()
        {
            MyPathValidator myPathValidator = new();
            OptionValuesValidator optionValuesValidator = new();
            _validator = new(myPathValidator, optionValuesValidator);

            MockServiceProvider _serviceProvider = new();
            MockIQuery mockIQuery = new();
            MockIProjectProvider mockIProjectProvider = new();
            _templateFactory = new(_serviceProvider.Object);
            _service = new(mockIQuery.Object, mockIProjectProvider.Object, _templateFactory);
            Console.SetOut(_consoleOutput);

            Directory.SetCurrentDirectory(DirectoryService.CreateTempDirectory().ToString());
            Environment.SetEnvironmentVariable("isTestProject", "true");
        }


        [TestMethod]
        public void Expected_Composes_Generated()
        {
            // Arrange
            string[] argument = [_settings.CommandName, ComposeSettings.ServiceOptionName[0], serviceArguments[0], serviceArguments[1],
                                                        ComposeSettings.ComponentOptionName[0], componentArguments[0], componentArguments[1]];
            DaprifyCommand<ComposeService, ComposeSettings> sut = new(_service, _settings, _validator);

            // Act
            sut.Parse(argument).Invoke();

            // Assert
            string filepath = MyPath.Combine(DirectoryService.GetCurrentDirectory().ToString(), DOCKER_DIR).ToString();
            Directory.Exists(filepath).Should().BeTrue($"Directory {filepath} should exist but was not found.");

            Directory.SetCurrentDirectory(filepath);
            File.Exists(EXPECTED_FILENAME).Should().BeTrue($"File {EXPECTED_FILENAME} should exist but was not found.");
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