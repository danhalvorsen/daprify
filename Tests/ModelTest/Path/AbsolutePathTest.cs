using Daprify.Models;
using DaprifyTests.Assert;

namespace DaprifyTests.Paths
{
    [TestClass]
    public class TryAbsolutePath
    {
        private readonly AbsolutePath _basePath = new("Folder1/FolderX/Folder2");

        [TestMethod]
        public void Expect_Constructor_ConvertsToAbsolutePath()
        {
            // Arrange
            string relativePath = "relative/path";
            string expectedAbsolutePath = Path.GetFullPath(relativePath);

            // Act
            AbsolutePath sut = new(relativePath);

            // Assert
            Asserts.VerifyString(sut.ToString(), expectedAbsolutePath);
        }


        [TestMethod]
        public void Expect_GetRelativePath_WithValidTarget_ShouldReturnRelativePath()
        {
            // Arrange
            AbsolutePath targetPath = new("Folder1/FolderX/Folder3");
            string expectedResult = "../Folder3";

            // Act
            RelativePath sut = MyPath.GetRelativePath(_basePath, targetPath);

            // Assert
            Asserts.VerifyString(sut.ToString(), expectedResult);
        }

        [TestMethod]
        public void Expect_GetRelativePath_WithNullTarget_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            Action act = () => MyPath.GetRelativePath(_basePath, null!);

            // Assert
            Asserts.VerifyException<ArgumentNullException>(act);
        }
        [TestMethod]
        public void Expect_GetRelativePath_ToItself_ShouldReturnEmptyPath()
        {
            // Arrange
            string expectedResult = ".";

            // Act
            RelativePath sut = MyPath.GetRelativePath(_basePath, _basePath);

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
            RelativePath sut = MyPath.GetRelativePath(basePath, targetPath);

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
            RelativePath sut = MyPath.GetRelativePath(_basePath, targetPath);

            // Assert
            Asserts.VerifyString(sut.ToString(), expectedResult);
        }
    }
}
