using FluentAssertions;
using Daprify.Models;
using Daprify.Validation;
using FluentValidation.Results;
using DaprifyTests.Assert;

namespace DaprifyTests.Validation
{
    [TestClass]
    public class TryOptionValuesValidatorTest
    {
        private readonly OptionValuesValidator _validator = new();

        [TestMethod]
        public void Expect_Validation_WithValidOptionValues_ShouldPass()
        {
            // Arrange
            OptionValues optionValues = new(new Key("components"), ["pubsub", "redis"]);

            // Act
            ValidationResult result = _validator.Validate(optionValues);

            // Assert
            Asserts.VerifyTrue(result.IsValid);
        }

        [TestMethod]
        public void Expect_Validation_WithInvalidOptionValues_ShouldFail()
        {
            // Arrange
            OptionValues optionValues = new(new Key("components"), ["invalidComponent"]);

            // Act
            ValidationResult result = _validator.Validate(optionValues);

            // Assert
            Asserts.VerifyFalse(result.IsValid);
        }

        [TestMethod]
        public void Expect_Validation_WithEmptyOptionValues_ShouldFail()
        {
            // Arrange
            OptionValues optionValues = new(new Key("components"), []);

            // Act
            ValidationResult result = _validator.Validate(optionValues);

            // Assert
            Asserts.VerifyTrue(result.IsValid);
        }

        [TestMethod]
        public void Expect_Validation_WithValidSettingsOptionValues_ShouldPass()
        {
            // Arrange
            var optionValues = new OptionValues(new("settings"), ["mtls", "https"]);

            // Act
            var result = _validator.Validate(optionValues);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [TestMethod]
        public void Expect_Validation_WithInvalidSettingsOptionValues_ShouldFail()
        {
            // Arrange
            var optionValues = new OptionValues(new("settings"), ["invalidSetting"]);

            // Act
            var result = _validator.Validate(optionValues);

            // Assert
            result.IsValid.Should().BeFalse();
        }

        [TestMethod]
        public void Expect_Validation_WithEmptySettingsOptionValues_ShouldFail()
        {
            // Arrange
            var optionValues = new OptionValues(new("settings"), []);

            // Act
            var result = _validator.Validate(optionValues);

            // Assert
            Asserts.VerifyTrue(result.IsValid);
        }
    }
}
