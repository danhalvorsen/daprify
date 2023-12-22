using CLI.Models;
using CLITests.Assert;
using FluentAssertions;

namespace CLITests.Paths
{
    [TestClass]
    public class TryAbsolutePath
    {
        private readonly AbsolutePath _basePath = new("Folder1/FolderX/Folder2");
        [TestMethod]
        public void Expect_GetRelativePath_WithValidTarget_ShouldReturnRelativePath()
        {
            // Arrange
            AbsolutePath targetPath = new("Folder1/FolderX/Folder3");
            string expectedResult = "../Folder3";

            // Act
            AbsolutePath sut = _basePath.GetRelativePath(targetPath);

            // Assert
            Asserts.VerifyString(sut.ToString(), expectedResult);
        }

        [TestMethod]
        public void Expect_GetRelativePath_WithNullTarget_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            Action act = () => _basePath.GetRelativePath(null!);

            // Assert
            Asserts.VerifyException<ArgumentNullException>(act);
        }
        [TestMethod]
        public void Expect_GetRelativePath_ToItself_ShouldReturnEmptyPath()
        {
            // Arrange
            string expectedResult = ".";

            // Act
            AbsolutePath sut = _basePath.GetRelativePath(_basePath);

            // Assert
            Asserts.VerifyString(sut.ToString(), expectedResult);
        }

        [TestMethod]
        public void Expect_GetRelativePath_ToSubfolder_ShouldReturnRelativeSubfolderPath()
        {
            // Arrange
            AbsolutePath basePath = new("Folder1/FolderX/");
            AbsolutePath targetPath = new("Folder1/FolderX/Folder2");
            string expectedResult = "Folder2";

            // Act
            AbsolutePath sut = basePath.GetRelativePath(targetPath);

            // Assert
            Asserts.VerifyString(sut.ToString(), expectedResult);
        }

        [TestMethod]
        public void Expect_GetRelativePath_ToParentFolder_ShouldReturnRelativeParentFolderPath()
        {
            // Arrange
            AbsolutePath targetPath = new("Folder1/FolderX/");
            string expectedResult = "..";

            // Act
            AbsolutePath sut = _basePath.GetRelativePath(targetPath);

            // Assert
            Asserts.VerifyString(sut.ToString(), expectedResult);
        }
    }
}
