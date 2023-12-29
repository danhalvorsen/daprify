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
    public class TryGenAllCommandTests
    {
        private readonly StringWriter _consoleOutput = new();
        private readonly GenAllService _service;
        private readonly GenAllSettings _settings = new();
        private readonly OptionValidatorFactory _optionValidatorFactory;
        private readonly MockServiceProvider _serviceProvider = new();
        private readonly TemplateFactory _templateFactory;
        private readonly DirectoryInfo _startingDir = new(Directory.GetCurrentDirectory());

        private const string FILE_EXT = ".yaml", DAPR_DIR = "Dapr", COMPOSE_FILE = "docker-compose.yml", DOCKER_DIR = "Docker/",
                            CONFIG_FILE = "config.yaml", CONFIG_DIR = "Config/", CERT_DIR = "Certs/",
                            ENV_FILE = "Dapr.Env", ENV_DIR = "Env/", COMPONENT_DIR = "Components/";
        private readonly MyPath _confPath;
        private readonly List<string> componentArgs = ["pubsub", "statestore"];
        private readonly List<string> settingArgs = ["mtls", "tracing"];
        private readonly List<string> certFiles = ["ca.crt", "issuer.crt", "issuer.key"];

        public TryGenAllCommandTests()
        {
            MyPathValidator myPathValidator = new();
            _optionValidatorFactory = new(myPathValidator);

            _templateFactory = new(_serviceProvider.Object);
            MockIQuery mockIQuery = new();
            MockIProjectProvider mockIProjectProvider = new();
            CertificateService certificateService = new();
            ComponentService componentService = new(_templateFactory);
            ComposeService composeService = new(mockIQuery.Object, mockIProjectProvider.Object, _templateFactory);
            ConfigService configService = new(_templateFactory);
            DockerfileService dockerfileService = new(mockIQuery.Object, mockIProjectProvider.Object, _templateFactory);
            _service = new(certificateService, componentService, composeService, configService, dockerfileService);
            Console.SetOut(_consoleOutput);

            string testDir = DirectoryService.FindDirectoryUpwards("CommandTest").FullName;
            _confPath = MyPath.Combine(testDir, "../Utils/Mocks/config-mock.json");
            Directory.SetCurrentDirectory(DirectoryService.CreateTempDirectory().ToString());
            Environment.SetEnvironmentVariable("isTestProject", "true");
        }


        [TestMethod]
        public void Expected_Certificates_Generated()
        {
            // Arrange
            string[] argument = [_settings.CommandName, GenAllSettings.SettingOptionName[0], settingArgs[0], settingArgs[1]];
            CLICommand<GenAllService, GenAllSettings> sut = new(_service, _settings, _optionValidatorFactory);

            // Act
            sut.Parse(argument).Invoke();

            // Assert
            Directory.SetCurrentDirectory(MyPath.Combine(DirectoryService.GetCurrentDirectory().ToString(), DAPR_DIR).ToString());
            string consoleOutput = _consoleOutput.ToString();

            foreach (string crt in certFiles)
            {
                consoleOutput.Should().Contain(crt);
                File.Exists(CERT_DIR + crt).Should().BeTrue($"File {crt} should exist but was not found.");
            }
        }


        [TestMethod]
        public void Expected_Env_File_Generated()
        {
            // Arrange
            string[] argument = [_settings.CommandName, GenAllSettings.SettingOptionName[0], settingArgs[0], settingArgs[1]];
            CLICommand<GenAllService, GenAllSettings> sut = new(_service, _settings, _optionValidatorFactory);

            // Act
            sut.Parse(argument).Invoke();

            // Assert
            Directory.SetCurrentDirectory(MyPath.Combine(DirectoryService.GetCurrentDirectory().ToString(), DAPR_DIR).ToString());
            File.Exists(ENV_DIR + ENV_FILE).Should().BeTrue($"File {ENV_FILE} should exist but was not found.");
        }


        [TestMethod]
        public void Expected_Component_Generated()
        {
            // Arrange
            string[] argument = [_settings.CommandName, GenAllSettings.ComponentOptionName[0], componentArgs[0], componentArgs[1]];
            CLICommand<GenAllService, GenAllSettings> sut = new(_service, _settings, _optionValidatorFactory);

            // Act
            sut.Parse(argument).Invoke();

            // Assert
            Directory.SetCurrentDirectory(MyPath.Combine(DirectoryService.GetCurrentDirectory().ToString(), DAPR_DIR).ToString());
            string consoleOutput = _consoleOutput.ToString();

            foreach (string comp in componentArgs)
            {
                consoleOutput.Should().Contain(comp);
                File.Exists(COMPONENT_DIR + comp + FILE_EXT).Should().BeTrue($"File {comp + FILE_EXT} should exist but was not found.");
            }
        }


        [TestMethod]
        public void Expected_Config_Generated()
        {
            // Arrange
            string[] argument = [_settings.CommandName];
            CLICommand<GenAllService, GenAllSettings> sut = new(_service, _settings, _optionValidatorFactory);

            // Act
            sut.Parse(argument).Invoke();

            // Assert
            Directory.SetCurrentDirectory(MyPath.Combine(DirectoryService.GetCurrentDirectory().ToString(), DAPR_DIR).ToString());
            string consoleOutput = _consoleOutput.ToString();

            consoleOutput.Should().Contain("config");
            File.Exists(CONFIG_DIR + CONFIG_FILE).Should().BeTrue($"File {CONFIG_FILE} should exist but was not found.");
        }

        [TestMethod]
        public void Expected_Compose_Generated()
        {
            // Arrange
            string[] argument = [_settings.CommandName, GenAllSettings.ComponentOptionName[0], componentArgs[0], componentArgs[1],
                                                        GenAllSettings.SettingOptionName[0], settingArgs[0], settingArgs[1]];
            CLICommand<GenAllService, GenAllSettings> sut = new(_service, _settings, _optionValidatorFactory);

            // Act
            sut.Parse(argument).Invoke();

            // Assert
            Directory.SetCurrentDirectory(MyPath.Combine(DirectoryService.GetCurrentDirectory().ToString(), DAPR_DIR).ToString());
            File.Exists(DOCKER_DIR + COMPOSE_FILE).Should().BeTrue($"File {COMPOSE_FILE} should exist but was not found.");
        }

        [TestMethod]
        public void Expected_Config_File_Generates_All()
        {
            // Arrange
            MyPath destDir = MyPath.Combine(DirectoryService.GetCurrentDirectory().ToString(), "Mocks");
            Directory.CreateDirectory(destDir.ToString());

            MyPath destPath = MyPath.Combine(destDir.ToString(), "config-mock.json");
            File.Copy(_confPath.ToString(), destPath.ToString(), true);
            string[] argument = [_settings.CommandName, GenAllSettings.ConfigOptionName[0], destPath.ToString()];
            CLICommand<GenAllService, GenAllSettings> sut = new(_service, _settings, _optionValidatorFactory);

            // Act
            sut.Parse(argument).Invoke();

            // Assert
            OptionDictionary options = OptionDictionary.PopulateFromJson(destPath.ToString());
            Directory.SetCurrentDirectory(MyPath.Combine(DirectoryService.GetCurrentDirectory().ToString(), DAPR_DIR).ToString());
            string consoleOutput = _consoleOutput.ToString();

            foreach (string crt in certFiles)
            {
                consoleOutput.Should().Contain(crt);
                File.Exists(CERT_DIR + crt).Should().BeTrue($"File {crt} should exist but was not found.");
            }

            foreach (string comp in options.GetAllPairValues(GenAllSettings.ComponentOptionName[0]))
            {
                consoleOutput.Should().Contain(comp);
                File.Exists(COMPONENT_DIR + comp + FILE_EXT).Should().BeTrue($"File {comp + FILE_EXT} should exist but was not found.");
            }

            File.Exists(ENV_DIR + ENV_FILE).Should().BeTrue($"File {ENV_FILE} should exist but was not found.");
            File.Exists(CONFIG_DIR + CONFIG_FILE).Should().BeTrue($"File {CONFIG_FILE} should exist but was not found.");
            File.Exists(DOCKER_DIR + COMPOSE_FILE).Should().BeTrue($"File {COMPOSE_FILE} should exist but was not found.");
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