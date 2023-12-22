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
        private readonly GenAllValidator _validator = new();
        private readonly MockServiceProvider _serviceProvider = new();
        private readonly TemplateFactory _templateFactory;
        private readonly DirectoryInfo _startingDir = new(Directory.GetCurrentDirectory());

        private const string FILE_EXT = ".yaml", DAPR_DIR = "Dapr", COMPOSE_FILE = "docker-compose.yml", DOCKER_DIR = "Docker/",
                            CONFIG_FILE = "config.yaml", CONFIG_DIR = "Config/", CERT_DIR = "Certs/",
                            ENV_FILE = "Dapr.Env", ENV_DIR = "Env/", COMPONENT_DIR = "Components/";
        private readonly string _confPath;
        private readonly List<string> componentArgs = ["pubsub", "statestore"];
        private readonly List<string> settingArgs = ["mtls", "tracing"];
        private readonly List<string> certFiles = ["ca.crt", "issuer.crt", "issuer.key"];

        public TryGenAllCommandTests()
        {
            _templateFactory = new(_serviceProvider.Object);
            MockIQuery mockIQuery = new();
            MockIProjectProvider mockIProjectProvider = new();
            CertificateService certificateService = new();
            ComponentService componentService = new(_templateFactory);
            ComposeService composeService = new(mockIQuery.Object, mockIProjectProvider.Object, _templateFactory);
            ConfigService configService = new(_templateFactory);
            _service = new(certificateService, componentService, composeService, configService);
            Console.SetOut(_consoleOutput);

            string testDir = DirectoryService.FindDirectoryUpwards("CommandTest").FullName;
            _confPath = Path.Combine(testDir, "../Mocks/config-mock.json");
            Directory.SetCurrentDirectory(DirectoryService.CreateTempDirectory());
            Environment.SetEnvironmentVariable("isTestProject", "true");
        }


        [TestMethod]
        public void Expected_Certificates_Generated()
        {
            // Arrange
            string[] argument = [_settings.CommandName, GenAllSettings.SettingOptionName[0], settingArgs[0], settingArgs[1]];
            CLICommand<GenAllService, GenAllSettings, GenAllValidator> sut = new(_service, _settings, _validator);

            // Act
            sut.Parse(argument).Invoke();

            // Assert
            Directory.SetCurrentDirectory(Path.Combine(Directory.GetCurrentDirectory(), DAPR_DIR));
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
            CLICommand<GenAllService, GenAllSettings, GenAllValidator> sut = new(_service, _settings, _validator);

            // Act
            sut.Parse(argument).Invoke();

            // Assert
            Directory.SetCurrentDirectory(Path.Combine(Directory.GetCurrentDirectory(), DAPR_DIR));
            File.Exists(ENV_DIR + ENV_FILE).Should().BeTrue($"File {ENV_FILE} should exist but was not found.");
        }


        [TestMethod]
        public void Expected_Component_Generated()
        {
            // Arrange
            string[] argument = [_settings.CommandName, GenAllSettings.ComponentOptionName[0], componentArgs[0], componentArgs[1]];
            CLICommand<GenAllService, GenAllSettings, GenAllValidator> sut = new(_service, _settings, _validator);

            // Act
            sut.Parse(argument).Invoke();

            // Assert
            Directory.SetCurrentDirectory(Path.Combine(Directory.GetCurrentDirectory(), DAPR_DIR));
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
            CLICommand<GenAllService, GenAllSettings, GenAllValidator> sut = new(_service, _settings, _validator);

            // Act
            sut.Parse(argument).Invoke();

            // Assert
            Directory.SetCurrentDirectory(Path.Combine(Directory.GetCurrentDirectory(), DAPR_DIR));
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
            CLICommand<GenAllService, GenAllSettings, GenAllValidator> sut = new(_service, _settings, _validator);

            // Act
            sut.Parse(argument).Invoke();

            // Assert
            Directory.SetCurrentDirectory(Path.Combine(Directory.GetCurrentDirectory(), DAPR_DIR));
            File.Exists(DOCKER_DIR + COMPOSE_FILE).Should().BeTrue($"File {COMPOSE_FILE} should exist but was not found.");
        }

        [TestMethod]
        public void Expected_Config_File_Generates_All()
        {
            // Arrange
            string destDir = Path.Combine(Directory.GetCurrentDirectory(), "Mocks");
            Directory.CreateDirectory(destDir);

            string destPath = Path.Combine(destDir, "config-mock.json");
            File.Copy(_confPath, destPath, true);
            string[] argument = [_settings.CommandName, GenAllSettings.ConfigOptionName[0], destPath];
            CLICommand<GenAllService, GenAllSettings, GenAllValidator> sut = new(_service, _settings, _validator);

            // Act
            sut.Parse(argument).Invoke();

            // Assert
            OptionDictionary options = OptionDictionary.PopulateFromJson(destPath);
            Directory.SetCurrentDirectory(Path.Combine(Directory.GetCurrentDirectory(), DAPR_DIR));
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
            DirectoryService.DeleteDirectory(Directory.GetCurrentDirectory());
            Directory.SetCurrentDirectory(_startingDir.FullName);
        }
    }
}