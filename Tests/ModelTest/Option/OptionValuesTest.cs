using CLI.Models;
using CLITests.Assert;

namespace CLITests.Options
{
    [TestClass]
    public class TryOptionValues
    {
        [TestMethod]
        public void Expect_ValuesMatchInitialization()
        {
            // Arrange
            IEnumerable<string> expectedValues = ["value1", "value2", "value3"];

            // Act
            OptionValues sut = new(expectedValues);

            // Assert
            Asserts.VerifyEnumerableString(sut.GetValues(), expectedValues);
        }

        [TestMethod]
        public void Expect_CorrectCount()
        {
            // Arrange
            IEnumerable<string> expectedValues = ["value1", "value2", "value3"];
            OptionValues sut = new(expectedValues);

            // Act
            int count = sut.Count();

            // Assert
            Asserts.VerifyCount(sut.GetValues(), count);
        }

        [TestMethod]
        public void Expect_EmptyOnNoInitialization()
        {
            // Arrange
            OptionValues optionValues = new(new List<string>());

            // Act
            IEnumerable<string> sut = optionValues.GetValues();

            // Assert
            Asserts.VerifyEmpty(sut);
        }
    }
}
