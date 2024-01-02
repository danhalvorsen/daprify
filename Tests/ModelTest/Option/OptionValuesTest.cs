using CLI.Models;
using CLITests.Assert;

namespace CLITests.Options
{
    [TestClass]
    public class TryOptionValues
    {
        [TestMethod]
        public void Expect_Constructor_WithStringList_InitializesCorrectly()
        {
            // Arrange
            List<string> expectedValues = ["value1", "value2", "value3"];

            // Act
            OptionValues sut = new(expectedValues);

            // Assert
            Asserts.VerifyEnumerableString(sut.GetStringEnumerable(), expectedValues);
        }

        [TestMethod]
        public void Expect_Constructor_WithValueList_InitializesCorrectly()
        {
            // Arrange
            List<Value> valueList = [new("value1"), new("value2"), new("value3")];

            // Act
            OptionValues sut = new(valueList);

            // Assert
            Asserts.VerifyEnumerableValue(sut.GetValues(), valueList);
        }

        [TestMethod]
        public void Expect_CorrectCount()
        {
            // Arrange
            List<string> expectedValues = ["value1", "value2", "value3"];
            OptionValues sut = new(expectedValues);

            // Act
            int count = sut.Count();

            // Assert
            Asserts.VerifyCount(sut.GetValues(), count);
        }

        [TestMethod]
        public void RemoveValue_RemovesSpecifiedValue()
        {
            // Arrange
            List<Value> initialValues = [new("value1"), new("value2"), new("value3")];
            Value valueToRemove = new("value2");
            OptionValues sut = new(initialValues);

            // Act
            sut.RemoveValue(valueToRemove);

            // Assert
            Asserts.VerifyFalse(sut.GetValues().Contains(valueToRemove));
        }

        [TestMethod]
        public void Contain_ReturnsTrueForContainedValue()
        {
            // Arrange
            Value valueToCheck = new("value1");
            List<Value> initialValues = [valueToCheck, new("value2"), new("value3")];
            OptionValues sut = new(initialValues);

            // Act
            bool contains = sut.Contain(valueToCheck);

            // Assert
            Asserts.VerifyTrue(contains);
        }

        [TestMethod]
        public void Contain_ReturnsFalseForNonExistentValue()
        {
            // Arrange
            Value valueToCheck = new("nonexistent");
            List<Value> initialValues = [new("value1"), new("value2"), new("value3")];
            OptionValues sut = new(initialValues);

            // Act
            bool contains = sut.Contain(valueToCheck);

            // Assert
            Asserts.VerifyFalse(contains);
        }
    }
}
