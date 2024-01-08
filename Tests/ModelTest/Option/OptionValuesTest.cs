using Daprify.Models;
using DaprifyTests.Assert;

namespace DaprifyTests.Options
{
    [TestClass]
    public class TryOptionValues
    {
        [TestMethod]
        public void Expect_Constructor_WithStringList_InitializesCorrectly()
        {
            // Arrange
            Key key = new("key");
            List<string> expectedValues = ["value1", "value2", "value3"];

            // Act
            OptionValues sut = new(key, expectedValues);

            // Assert
            Asserts.VerifyEnumerableString(sut.GetStringEnumerable(), expectedValues);
        }

        [TestMethod]
        public void Expect_CorrectCount()
        {
            // Arrange
            Key key = new("key");
            List<string> expectedValues = ["value1", "value2", "value3"];
            OptionValues sut = new(key, expectedValues);

            // Act
            int count = sut.Count();

            // Assert
            Asserts.VerifyCount(sut.GetValues(), count);
        }

        [TestMethod]
        public void RemoveValue_RemovesSpecifiedValue()
        {
            // Arrange
            Key key = new("key");
            List<string> initialValues = ["value1", "value2", "value3"];
            Value valueToRemove = new("value2");
            OptionValues sut = new(key, initialValues);

            // Act
            sut.RemoveValue(valueToRemove);

            // Assert
            Asserts.VerifyFalse(sut.GetValues().Contains(valueToRemove));
        }

        [TestMethod]
        public void Contain_ReturnsTrueForContainedValue()
        {
            // Arrange
            Key key = new("key");
            Value valueToCheck = new("value1");
            List<string> initialValues = [valueToCheck.ToString(), new("value2"), new("value3")];
            OptionValues sut = new(key, initialValues);

            // Act
            bool contains = sut.GetValues().Contains(valueToCheck);

            // Assert
            Asserts.VerifyTrue(contains);
        }

        [TestMethod]
        public void Contain_ReturnsFalseForNonExistentValue()
        {
            // Arrange
            Key key = new("key");
            Value valueToCheck = new("nonexistent");
            List<string> initialValues = ["value1", "value2", "value3"];
            OptionValues sut = new(key, initialValues);

            // Act
            bool contains = sut.GetValues().Contains(valueToCheck);

            // Assert
            Asserts.VerifyFalse(contains);
        }
    }
}
