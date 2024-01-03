using Daprify.Models;
using DaprifyTests.Assert;

namespace DaprifyTests.Options
{
    [TestClass]
    public class TryKey
    {
        [TestMethod]
        public void Expect_ToStringReturnsCorrectKey()
        {
            // Arrange
            string expectedKey = "testKey";
            Key key = new(expectedKey);

            // Act & Assert
            Asserts.VerifyString(key.ToString(), expectedKey);
        }

        [TestMethod]
        public void Expect_GetHashCodeReturnsConsistentValue()
        {
            // Arrange
            string testKey = "testKey";
            Key key1 = new(testKey);
            Key key2 = new(testKey);

            // Act & Assert
            Asserts.VerifyTrue(key1.GetHashCode() == key2.GetHashCode());
        }

        [TestMethod]
        public void Expect_EqualsIdentifiesEqualKeys()
        {
            // Arrange
            string testKey = "testKey";
            Key key1 = new(testKey);
            Key key2 = new(testKey);

            // Act & Assert
            Asserts.VerifyTrue(key1.Equals(key2));
        }

        [TestMethod]
        public void Expect_EqualsIdentifiesNonEqualKeys()
        {
            // Arrange
            Key key1 = new("key1");
            Key key2 = new("key2");

            // Act & Assert
            Asserts.VerifyFalse(key1.Equals(key2));
        }
    }
}
