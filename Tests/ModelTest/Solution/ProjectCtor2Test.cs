using CLI.Models;
using CLI.Services;
using CLITests.Assert;
using CLITests.Mocks;

namespace CLITests.Projects
{
    [TestClass]
    public class TrySecondProjectCtor
    {
        private const string PROJECT = "TempProject.csproj", PROJECT_DIR = "ProjectTest";
        private MyPath _tempDir;
        private MyPath _csprojPath;
        private readonly MockIQuery _mockIQuery = new();
        private readonly MockISolution _mockISolution = new();
        private readonly Name _name = new("TempProject");


        [TestInitialize]
        public MyPath InitializeTest()
        {
            _tempDir = new(DirectoryService.CreateTempDirectory([PROJECT_DIR]));
            _csprojPath = new(Path.Combine(_tempDir.ToString(), PROJECT));
            File.WriteAllText(Path.Combine(_tempDir.ToString(), PROJECT), "<Project Sdk=\"Microsoft.NET.Sdk\"> </Project>");
            return _tempDir;
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
            void act() => new Project(_mockIQuery.Object, _mockISolution.Object, invalidPath);

            // Assert
            Asserts.VerifyExceptionWithMessage<FileNotFoundException>(act, $"Error occurred while trying to find the project file: '{invalidPath}'");
        }

        [TestCleanup]
        public void TestCleanup()
        {
            string fullPath = Path.Combine(Directory.GetCurrentDirectory(), _tempDir.ToString());
            DirectoryService.DeleteDirectory(fullPath);
        }
    }
}