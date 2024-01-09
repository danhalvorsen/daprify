using Daprify.Models;
using DaprifyTests.Assert;
using FluentAssertions;

namespace DaprifyTests.Options
{
    [TestClass]
    public class TryValue
    {
        [TestMethod]
        public void Expect_ConstructorToInitializeValueCorrectly()
        {
            // Arrange
            string expectedValue = "testValue";

            // Act
            Value value = new(expectedValue);

            // Assert
            Asserts.VerifyShouldBe(value.ToString(), expectedValue);
        }

        [TestMethod]
        public void Expect_ConstructorToThrowExceptionForNull()
        {
            // Arrange
            static void act()
            {
                _ = new Value(null!);
            }

            // Assert
            Asserts.VerifyExceptionWithMessage<ArgumentNullException>(act, "The option value cannot be null! (Parameter 'value')");
        }


        [TestMethod]
        public void Expect_ToStringReturnsCorrectValue()
        {
            // Arrange
            string expectedValue = "testValue";
            Value value = new(expectedValue);

            // Act & Assert
            Asserts.VerifyShouldBe(value.ToString(), expectedValue);
        }

        [TestMethod]
        public void Expect_GetHashCodeReturnsConsistentValue()
        {
            // Arrange
            string testValue = "testValue";
            Value value1 = new(testValue);
            Value value2 = new(testValue);

            // Act & Assert
            Asserts.VerifyTrue(value1.GetHashCode() == value2.GetHashCode());
        }

        [TestMethod]
        public void Expect_EqualsIdentifiesEqualValues()
        {
            // Arrange
            string testValue = "testValue";
            Value value1 = new(testValue);
            Value value2 = new(testValue);

            // Act & Assert
            Asserts.VerifyTrue(value1.Equals(value2));
        }

        [TestMethod]
        public void Expect_EqualsIdentifiesNonEqualValues()
        {
            // Arrange
            Value value1 = new("value1");
            Value value2 = new("value2");

            // Act & Assert
            Asserts.VerifyFalse(value1.Equals(value2));
        }

        [TestMethod]
        public void Expect_EqualsReturnsFalseForDifferentObjectType()
        {
            // Arrange
            Value value1 = new("testValue");
            object value2 = new();

            // Act & Assert
            Asserts.VerifyFalse(value1.Equals(value2));
        }

        [TestMethod]
        public void Expect_EqualsReturnsFalseForNull()
        {
            // Arrange
            Value key = new("testValue");

            // Act & Assert
            Asserts.VerifyFalse(key.Equals(null));
        }
    }
}
