using Daprify.Models;
using DaprifyTests.Assert;
using FluentAssertions;

namespace DaprifyTests.Paths
{
    [TestClass]
    public class TryMyPath
    {
        [TestMethod]
        public void Expect_Constructor_WithValidPath_ShouldSetPath()
        {
            // Arrange
            string validPath = "C:\\test";

            // Act
            MyPath sut = new(validPath);

            // Assert
            Asserts.VerifyString(sut.ToString(), validPath);
        }

        [TestMethod]
        public void Expect_Constructor_WithNullPath_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            static void act()
            {
                MyPath unused = new((string)null!);
            }

            // Assert
            Asserts.VerifyException<ArgumentNullException>(act);
        }

        [TestMethod]
        public void Expect_SetPath_WithValidPath_ShouldUpdatePath()
        {
            // Arrange
            MyPath sut = new("C:\\initial");
            string newPath = "C:\\new";

            // Act
            sut.SetPath(newPath);

            // Assert
            Asserts.VerifyString(sut.ToString(), newPath);
        }

        [TestMethod]
        public void Expect_SetPath_WithNullPath_ShouldThrowArgumentNullException()
        {
            // Arrange
            MyPath myPath = new("C:\\initial");

            // Act & Assert
            Action act = () => myPath.SetPath(null!);

            // Assert
            Asserts.VerifyException<ArgumentNullException>(act);
        }

        [TestMethod]
        public void Expect_FromStringList_WithValidList_ShouldReturnMyPathList()
        {
            // Arrange
            List<Value> paths = [new("C:\\path1"), new("C:\\path2")];

            // Act
            List<MyPath> sut = MyPath.FromStringList(paths).ToList();

            // Assert
            IEnumerable<string> expectedPaths = paths.Select(p => new MyPath(p).ToString());
            IEnumerable<string> actualPaths = sut.Select(mp => mp.ToString());

            Asserts.VerifyEnumerableString(actualPaths, expectedPaths);
        }

        [TestMethod]
        public void Expect_ToString_WhenCalled_ShouldReturnPath()
        {
            // Arrange
            string path = "C:\\path";
            MyPath myPath = new(path);

            // Act
            string sut = myPath.ToString();

            // Assert
            sut.Should().Be(path);
        }
    }
}