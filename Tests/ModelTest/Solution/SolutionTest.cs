using CLI.Models;
using CLI.Services;
using CLITests.Assert;
using CLITests.Mocks;
using FluentAssertions;
using Moq;

namespace CLITests.Solutions
{
    [TestClass]
    public class TrySolution
    {
        private const string SLN = "TempSolution.sln", SLN_DIR = "SolutionTest";
        private readonly int _numberOfProjects = 2;
        private readonly MockIQuery _mockIQuery = new();
        private MockIProjectProvider _mockIProjectProvider;
        private MyPath _tempDir;


        [TestInitialize]
        public void TestInitialize()
        {
            _mockIProjectProvider = new MockIProjectProvider(_mockIQuery, _numberOfProjects);
            _tempDir = new(DirectoryService.CreateTempDirectory([SLN_DIR]));
            File.WriteAllText(Path.Combine(_tempDir.ToString(), SLN), "Microsoft Visual Studio Solution File, Format Version 12.00");
        }

        [TestMethod]
        public void Expect_Solution_Initialized_WithValid_Path()
        {
            // Arrange
            MyPath currentDir = new(Directory.GetCurrentDirectory());
            RelativePath relativePath = new(currentDir, _tempDir);

            // Act
            Solution sut = new(_mockIQuery.Object, _mockIProjectProvider.Object, relativePath);

            // Assert
            Asserts.VerifyNotNull(sut.GetPath());
        }

        [TestMethod]
        public void Expect_Solution_Initialized_With_Projects()
        {
            // Arrange
            MyPath currentDir = new(Directory.GetCurrentDirectory());
            RelativePath relativePath = new(currentDir, _tempDir);

            // Act
            Solution sut = new(_mockIQuery.Object, _mockIProjectProvider.Object, relativePath);

            // Assert
            Asserts.VerifyNotNull(sut.GetProjects());
            Asserts.VerifyItemCount(sut.GetProjects(), _numberOfProjects);
        }

        [TestMethod]
        public void Expect_IQuery_Called_Once()
        {
            // Arrange
            MyPath currentDir = new(Directory.GetCurrentDirectory());
            RelativePath relativePath = new(currentDir, _tempDir);

            // Act
            Solution sut = new(_mockIQuery.Object, _mockIProjectProvider.Object, relativePath);

            // Assert
            _mockIQuery.Verify(x => x.CheckPackageReference(It.IsAny<Project>(), It.IsAny<string>()), Times.Never);
            _mockIQuery.Verify(x => x.GetFileInDirectory(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public void Expect_IProjectProvider_Never_Called()
        {
            // Arrange
            MyPath currentDir = new(Directory.GetCurrentDirectory());
            RelativePath relativePath = new(currentDir, _tempDir);

            // Act
            Solution sut = new(_mockIQuery.Object, _mockIProjectProvider.Object, relativePath);

            // Assert
            _mockIProjectProvider.Verify(x => x.GetProjects(sut), Times.Never);
        }


        [TestMethod]
        public void Expect_Constructor_WhenSolutionFileNotFound_ShouldThrowFileNotFoundException()
        {
            // Arrange
            MyPath invalidPath = new("InvalidPath");

            // Act
            Action act = () =>
            {
                Solution unused = new(_mockIQuery.Object, _mockIProjectProvider.Object, invalidPath);
            };

            // Assert
            act.Should().Throw<FileNotFoundException>()
               .WithMessage("*Could not find the solution file*");
        }


        [TestCleanup]
        public void TestCleanup()
        {
            string fullPath = Path.Combine(Directory.GetCurrentDirectory(), _tempDir.ToString());
            DirectoryService.DeleteDirectory(fullPath);
        }
    }
}
