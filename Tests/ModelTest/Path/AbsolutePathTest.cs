using CLI.Models;
using CLITests.Assert;
using FluentAssertions;

namespace CLITests.Paths
{
    [TestClass]
    public class TryAbsolutePath
    {
        [TestMethod]
        public void Expect_GetRelativePath_WithValidTarget_ShouldReturnRelativePath()
        {
            // Arrange
            AbsolutePath basePath = new("Folder1/FolderX/Folder2");
            AbsolutePath targetPath = new("Folder1/FolderX/Folder3");
            MyPath expectedResult = new("../Folder3");

            // Act
            AbsolutePath sut = basePath.GetRelativePath(targetPath);

            // Assert
            Asserts.VerifyString(sut, expectedResult);
        }

        [TestMethod]
        public void Expect_GetRelativePath_WithNullTarget_ShouldThrowArgumentNullException()
        {
            // Arrange
            AbsolutePath basePath = new("Folder1/FolderX/Folder2");

            // Act & Assert
            Action act = () => basePath.GetRelativePath(null!);

            // Assert
            Asserts.VerifyException<ArgumentNullException>(act);
        }
        [TestMethod]
        public void Expect_GetRelativePath_ToItself_ShouldReturnEmptyPath()
        {
            // Arrange
            AbsolutePath basePath = new("Folder1/FolderX/Folder2");
            MyPath expectedResult = new(".");

            // Act
            AbsolutePath sut = basePath.GetRelativePath(basePath);

            // Assert
            Asserts.VerifyString(sut, expectedResult);
        }

        [TestMethod]
        public void Expect_GetRelativePath_ToSubfolder_ShouldReturnRelativeSubfolderPath()
        {
            // Arrange
            AbsolutePath basePath = new("Folder1/FolderX/");
            AbsolutePath targetPath = new("Folder1/FolderX/Folder2");
            MyPath expectedResult = new("Folder2");

            // Act
            AbsolutePath sut = basePath.GetRelativePath(targetPath);

            // Assert
            Asserts.VerifyString(sut, expectedResult);
        }

        [TestMethod]
        public void Expect_GetRelativePath_ToParentFolder_ShouldReturnRelativeParentFolderPath()
        {
            // Arrange
            AbsolutePath basePath = new("Folder1/FolderX/Folder2");
            AbsolutePath targetPath = new("Folder1/FolderX/");
            MyPath expectedResult = new("..");

            // Act
            AbsolutePath sut = basePath.GetRelativePath(targetPath);

            // Assert
            Asserts.VerifyString(sut, expectedResult);
        }
    }
}
