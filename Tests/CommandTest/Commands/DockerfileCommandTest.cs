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
    public class TryDockerfileCommandTests
    {
        private readonly StringWriter _consoleOutput = new();
        private readonly DockerfileService _service;
        private readonly DockerfileSettings _settings = new();
        private readonly OptionDictionaryValidator _validator;
        private readonly DirectoryInfo _startingDir = new(Directory.GetCurrentDirectory());

        private const string FILE_EXT = ".Dockerfile", DOCKER_DIR = "Dapr/Docker";
        private readonly OptionValues arguments = new(new Key("components"), ["ServiceA", "ServiceB"]);

        public TryDockerfileCommandTests()
        {
            MyPathValidator myPathValidator = new();
            OptionValuesValidator optionValuesValidator = new();
            _validator = new(myPathValidator, optionValuesValidator);

            MockIQuery mockIQuery = new();
            MockIProjectProvider mockIProjectProvider = new();
            MockServiceProvider serviceProvider = new();

            TemplateFactory templateFactory = new(serviceProvider.Object);
            _service = new(mockIQuery.Object, mockIProjectProvider.Object, templateFactory);
            Console.SetOut(_consoleOutput);

            Directory.SetCurrentDirectory(DirectoryService.CreateTempDirectory().ToString());
            Environment.SetEnvironmentVariable("isTestProject", "true");
        }


        [TestMethod]
        public void Expected_Dockerfiles_Generated()
        {
            // Arrange
            string[] argument = [_settings.CommandName, DockerfileSettings.ServiceOptionName[0],
                                 arguments.GetValues().ElementAt(0).ToString(),
                                 arguments.GetValues().ElementAt(1).ToString()];
            DaprifyCommand<DockerfileService, DockerfileSettings> sut = new(_service, _settings, _validator);

            // Act
            sut.Parse(argument).Invoke();

            // Assert
            string consoleOutput = _consoleOutput.ToString();
            Directory.SetCurrentDirectory(MyPath.Combine(DirectoryService.GetCurrentDirectory().ToString(), DOCKER_DIR).ToString());

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