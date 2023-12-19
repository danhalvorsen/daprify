using CLI.Commands;
using CLI.Services;
using CLI.Settings;
using CLI.Validation;
using FluentAssertions;
using System.CommandLine;
using System.CommandLine.Parsing;

namespace CLITests.Commands
{
    [TestClass]
    public class TryCertificateCommandTests
    {
        private readonly StringWriter _consoleOutput = new();
        private readonly CertificateService _service = new();
        private readonly CertificateSettings _settings = new();
        private readonly CertificateValidator _validator = new();
        private readonly DirectoryInfo _startingDir = new(Directory.GetCurrentDirectory());

        private const string DAPR_DIR = "Dapr", CERT_DIR = "Certs/", ENV_FILE = "Dapr.Env", ENV_DIR = "Env/";
        private readonly List<string> certFiles = ["ca.crt", "issuer.crt", "issuer.key",];

        public TryCertificateCommandTests()
        {
            Console.SetOut(_consoleOutput);
            Directory.SetCurrentDirectory(DirectoryService.CreateTempDirectory());
            Environment.SetEnvironmentVariable("isTestProject", "true");
        }


        [TestMethod]
        public void Expected_Certificates_Generated()
        {
            // Arrange
            string[] argument = [_settings.CommandName];
            CLICommand<CertificateService, CertificateSettings, CertificateValidator> sut = new(_service, _settings, _validator);

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
            string[] argument = [_settings.CommandName];
            CLICommand<CertificateService, CertificateSettings, CertificateValidator> sut = new(_service, _settings, _validator);

            // Act
            sut.Parse(argument).Invoke();

            // Assert
            Directory.SetCurrentDirectory(Path.Combine(Directory.GetCurrentDirectory(), DAPR_DIR));
            File.Exists(ENV_DIR + ENV_FILE).Should().BeTrue($"File {ENV_FILE} should exist but was not found.");
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