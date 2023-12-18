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

        private const string CERT_PATH = "Dapr/Certs";

        private readonly List<string> expectedOutputs =
        [
            "ca.crt",
            "issuer.crt",
            "issuer.key",
        ];

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
            CLICommand<CertificateService, CertificateSettings, CertificateValidator> sut = new(_service, _settings, _validator);

            // Act
            sut.Parse().Invoke();

            // Assert
            string dirPath = Path.Combine(Directory.GetCurrentDirectory(), CERT_PATH);
            Directory.Exists(dirPath).Should().BeTrue($"Directory {dirPath} should exist but was not found.");

            string consoleOutput = _consoleOutput.ToString();
            foreach (string expectedFile in expectedOutputs)
            {
                consoleOutput.Should().Contain(expectedFile);
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), CERT_PATH, expectedFile);
                File.Exists(filePath).Should().BeTrue($"File {filePath} should exist but was not found.");
            }
        }


        [TestCleanup]
        public void Cleanup()
        {
            string DirToDelete = Path.Combine(Directory.GetCurrentDirectory(), CERT_PATH);
            DirectoryService.DeleteDirectory(DirToDelete);

            _consoleOutput.GetStringBuilder().Clear();
            Directory.SetCurrentDirectory(_startingDir.FullName);
        }
    }
}