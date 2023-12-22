using CLI.Models;
using CLITests.Assert;
using CLITests.Mocks;

namespace CLITests.Projects
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
            Asserts.VerifyString(sut.GetName().ToString(), _name.ToString());
        }

        [TestMethod]
        public void Expect_Constructor_SetsPathToEmpty()
        {
            // Act
            Project sut = new(_mockIQuery.Object, _name);

            // Assert
            Asserts.VerifyString(sut.GetPath().ToString(), string.Empty);
        }

        [TestMethod]
        public void Expect_Query_Never_Called()
        {
            // Assert
            _mockIQuery.VerifyNoOtherCalls();
        }
    }
}