using Daprify.Models;
using Daprify.Services;
using DaprifyTests.Assert;
using DaprifyTests.Mocks;

namespace DaprifyTests.Projects
{
    [TestClass]
    public class TrySecondProjectCtor
    {
        private const string PROJECT = "TempProject.csproj";
        private MyPath _tempDir;
        private MyPath _csprojPath;
        private readonly MockIQuery _mockIQuery = new();
        private readonly MockISolution _mockISolution = new();
        private readonly Name _name = new("TempProject");


        [TestInitialize]
        public void InitializeTest()
        {
            MyPath projectDir = new("ProjectTest");
            _tempDir = DirectoryService.CreateTempDirectory([projectDir]);
            _csprojPath = MyPath.Combine(_tempDir.ToString(), PROJECT);
            DirectoryService.WriteFile(_tempDir, PROJECT, "<Project Sdk=\"Microsoft.NET.Sdk\"> </Project>");
        }

        [TestMethod]
        public void Expect_Project_Constructor_Set_SolutionPath()
        {
            // Act
            Project sut = new(_mockIQuery.Object, _mockISolution.Object, _csprojPath.ToString());

            // Assert
            Asserts.VerifyString(sut.GetSolution().GetPath().ToString(), _mockISolution.Object.GetPath().ToString());

        }

        [TestMethod]
        public void Expect_Project_Constructor_Loads_Project()
        {
            // Act
            Project sut = new(_mockIQuery.Object, _mockISolution.Object, _csprojPath.ToString());

            // Assert
            Asserts.VerifyNotNull(sut.GetProject());

        }

        [TestMethod]
        public void Expect_Project_Constructor_Sets_Path()
        {
            // Act
            Project sut = new(_mockIQuery.Object, _mockISolution.Object, _csprojPath.ToString());

            // Assert
            Asserts.VerifyString(sut.GetPath().ToString(), _csprojPath.ToString());

        }

        [TestMethod]
        public void Expect_Project_Constructor_Sets_RelativeSlnPath()
        {
            // Arrange
            RelativePath relativeSlnPath = new(_mockISolution.Object.GetPath(), _csprojPath);

            // Act
            Project sut = new(_mockIQuery.Object, _mockISolution.Object, _csprojPath.ToString());

            // Assert
            Asserts.VerifyString(sut.GetRelativeSlnPath().ToString(), relativeSlnPath.ToString());

        }

        [TestMethod]
        public void Expect_Project_Constructor_Sets_ProjectName()
        {
            // Arrange

            // Act
            Project sut = new(_mockIQuery.Object, _mockISolution.Object, _csprojPath.ToString());

            // Assert
            Asserts.VerifyString(sut.GetName().ToString(), _name.ToString());

        }

        [TestMethod]
        public void Expect_Project_Constructor_Throws_FileNotFoundException_For_InvalidPath()
        {
            // Arrange
            string invalidPath = "/invalid/path/project.csproj";

            // Act
            void act()
            {
                Project unused = new(_mockIQuery.Object, _mockISolution.Object, invalidPath);
            }

            // Assert
            Asserts.VerifyExceptionWithMessage<FileNotFoundException>(act, $"Error occurred while trying to find the project file: '{invalidPath}'");
        }

        [TestCleanup]
        public void TestCleanup()
        {
            MyPath fullPath = MyPath.Combine(DirectoryService.GetCurrentDirectory().ToString(), _tempDir.ToString());
            DirectoryService.DeleteDirectory(fullPath);
        }
    }
}