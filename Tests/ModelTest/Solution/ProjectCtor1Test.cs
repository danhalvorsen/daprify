using Daprify.Models;
using DaprifyTests.Assert;
using DaprifyTests.Mocks;

namespace DaprifyTests.Projects
{
    [TestClass]
    public class TryFirstProjectCtor
    {
        private readonly MockIQuery _mockIQuery = new();
        private readonly Name _name = new("TempProject");

        [TestMethod]
        public void Expect_Constructor_WithValidName_ShouldSetProjectName()
        {
            // Act
            Project sut = new(_mockIQuery.Object, _name);

            // Assert
            Asserts.VerifyShouldBe(sut.GetName(), _name);
        }

        [TestMethod]
        public void Expect_Constructor_SetsPathToEmpty()
        {
            // Act
            Project sut = new(_mockIQuery.Object, _name);

            // Assert
            Asserts.VerifyShouldBe(sut.GetPath().ToString(), string.Empty);
        }

        [TestMethod]
        public void Expect_Query_Never_Called()
        {
            // Assert
            _mockIQuery.VerifyNoOtherCalls();
        }
    }
}