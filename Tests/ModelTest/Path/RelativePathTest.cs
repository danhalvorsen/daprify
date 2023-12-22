using CLI.Models;
using CLITests.Assert;
namespace CLITests.Paths
{
    [TestClass]
    public class TryRelativePath
    {
        [TestMethod]
        public void Expect_Constructor_WithUnixPaths_ShouldCreateCorrectRelativePath()
        {
            // Arrange
            MyPath basePath = new("/home/user/Folder1");
            MyPath targetPath = new("/home/user/Folder1/Folder2");
            string expected = "Folder2";

            // Act
            RelativePath sut = new(basePath, targetPath);

            // Assert
            Asserts.VerifyString(sut.ToString(), expected);
            Asserts.VerifyString(sut.GetTargetPath().ToString(), targetPath.ToString());
        }

        [TestMethod]
        public void Expect_Constructor_WithNullBasePath_ShouldThrowArgumentNullException()
        {
            // Arrange
            MyPath basePath = null!;
            MyPath targetPath = new("/home/user/Folder1/Folder2");

            // Act & Assert
            void act()
            {
                RelativePath unused = new(basePath, targetPath);
            }

            // Assert
            Asserts.VerifyException<ArgumentNullException>(act);
        }

        [TestMethod]
        public void Expect_Constructor_WithNullTargetPath_ShouldThrowArgumentNullException()
        {
            // Arrange
            MyPath basePath = new("/home/user/Folder1");
            MyPath targetPath = null!;

            // Act & Assert
            void act()
            {
                RelativePath unused = new(basePath, targetPath);
            }

            // Assert
            Asserts.VerifyException<ArgumentNullException>(act);
        }

        [TestMethod]
        public void Expect_Constructor_WithUnixPathsToItself_ShouldReturnEmptyPath()
        {
            // Arrange
            MyPath basePath = new("/home/user/Folder1");
            string expected = ".";

            // Act
            RelativePath sut = new(basePath, basePath);

            // Assert
            Asserts.VerifyString(sut.ToString(), expected);
        }
    }
}
