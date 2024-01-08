using Daprify.Commands;
using Daprify.Models;
using Daprify.Services;
using Daprify.Settings;
using Daprify.Validation;
using FluentAssertions;
using System.CommandLine;
using System.CommandLine.Parsing;

namespace DaprifyTests.Commands
{
    [TestClass]
    public class TryCertificateCommandTests
    {
        private readonly StringWriter _consoleOutput = new();
        private readonly CertificateService _service = new();
        private readonly CertificateSettings _settings = new();
        private readonly OptionDictionaryValidator _validator;
        private readonly DirectoryInfo _startingDir = new(Directory.GetCurrentDirectory());

        private const string DAPR_DIR = "Dapr", CERT_DIR = "Certs/", ENV_FILE = "Dapr.Env", ENV_DIR = "Env/";
        private readonly List<string> certFiles = ["ca.crt", "issuer.crt", "issuer.key",];

        public TryCertificateCommandTests()
        {
            MyPathValidator myPathValidator = new();
            OptionValuesValidator optionValuesValidator = new();
            _validator = new(myPathValidator, optionValuesValidator);

            Console.SetOut(_consoleOutput);
            Directory.SetCurrentDirectory(DirectoryService.CreateTempDirectory().ToString());
            Environment.SetEnvironmentVariable("isTestProject", "true");
        }


        [TestMethod]
        public void Expected_Certificates_Generated()
        {
            // Arrange
            string[] argument = [_settings.CommandName];
            DaprifyCommand<CertificateService, CertificateSettings> sut = new(_service, _settings, _validator);

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
            string[] argument = [_settings.CommandName];
            DaprifyCommand<CertificateService, CertificateSettings> sut = new(_service, _settings, _validator);

            // Act
            sut.Parse(argument).Invoke();

            // Assert
            Directory.SetCurrentDirectory(MyPath.Combine(DirectoryService.GetCurrentDirectory().ToString(), DAPR_DIR).ToString());
            File.Exists(ENV_DIR + ENV_FILE).Should().BeTrue($"File {ENV_FILE} should exist but was not found.");
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