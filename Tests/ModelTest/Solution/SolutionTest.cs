using Daprify.Models;
using Daprify.Services;
using DaprifyTests.Assert;
using DaprifyTests.Mocks;
using FluentAssertions;
using Microsoft.Build.Construction;
using Moq;

namespace DaprifyTests.Solutions
{
    [TestClass]
    public class TrySolution
    {
        private const string SLN = "TempSolution.sln";
        private readonly int _numberOfProjects = 2;
        private readonly MockIQuery _mockIQuery = new();
        private MockIProjectProvider _mockIProjectProvider;
        private MyPath _tempDir;


        [TestInitialize]
        public void TestInitialize()
        {
            MyPath slnDir = new("SolutionTest");
            _mockIProjectProvider = new MockIProjectProvider(_mockIQuery, _numberOfProjects);
            _tempDir = DirectoryService.CreateTempDirectory([slnDir]);
            DirectoryService.WriteFile(_tempDir, SLN, "Microsoft Visual Studio Solution File, Format Version 12.00");
        }

        [TestMethod]
        public void Expect_EmptyConstructor_InitializesSolution()
        {
            // Act
            Solution solution = new();

            // Assert
            Asserts.VerifyNotNull(solution);
        }

        [TestMethod]
        public void Expect_Solution_Initialized_WithValid_Path()
        {
            // Arrange
            MyPath currentDir = DirectoryService.GetCurrentDirectory();
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
            MyPath currentDir = DirectoryService.GetCurrentDirectory();
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
            MyPath currentDir = DirectoryService.GetCurrentDirectory();
            RelativePath relativePath = new(currentDir, _tempDir);

            // Act
            Solution sut = new(_mockIQuery.Object, _mockIProjectProvider.Object, relativePath);

            // Assert
            _mockIQuery.Verify(x => x.CheckPackageReference(It.IsAny<Project>(), It.IsAny<string>()), Times.Never);
            _mockIQuery.Verify(x => x.GetFileInDirectory(It.IsAny<IPath>(), It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public void Expect_IProjectProvider_Never_Called()
        {
            // Arrange
            MyPath currentDir = DirectoryService.GetCurrentDirectory();
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

        [TestMethod]
        public void Expect_GetSolutionFile_Returns_SolutionFile()
        {
            // Arrange
            MyPath currentDir = DirectoryService.GetCurrentDirectory();
            RelativePath relativePath = new(currentDir, _tempDir);
            Solution sut = new(_mockIQuery.Object, _mockIProjectProvider.Object, relativePath);

            // Act
            SolutionFile solutionFile = sut.GetSolutionFile();

            // Assert
            Asserts.VerifyNotNull(solutionFile);
        }


        [TestCleanup]
        public void TestCleanup()
        {
            IPath fullPath = MyPath.Combine(DirectoryService.GetCurrentDirectory().ToString(), _tempDir.ToString());
            DirectoryService.DeleteDirectory(fullPath);
        }
    }
}
