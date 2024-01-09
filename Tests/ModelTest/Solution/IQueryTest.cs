using Daprify.Models;
using DaprifyTests.Assert;
using DaprifyTests.Mocks;

namespace DaprifyTests.IQuery
{
    [TestClass]
    public class TryIQuery
    {
        private readonly MockIProject _mockProject = new();

        [TestMethod]
        public void CheckPackageReference_ReturnsTrue_WhenDependencyExists()
        {
            // Arrange
            string dependency = "ExampleDependency";
            _mockProject.SetupDependencyExists(dependency);
            Query sut = new();

            // Act
            bool result = sut.CheckPackageReference(_mockProject.Object, dependency);

            // Assert
            Asserts.VerifyTrue(result);
        }

        [TestMethod]
        public void CheckPackageReference_ReturnsFalse_WhenDependencyDoesNotExist()
        {
            // Arrange
            string dependency = "NonExistentDependency";
            _mockProject.SetupDependencyDoesNotExist(dependency);
            Query sut = new();

            // Act
            bool result = sut.CheckPackageReference(_mockProject.Object, dependency);

            // Assert
            Asserts.VerifyFalse(result);
        }
    }
}
