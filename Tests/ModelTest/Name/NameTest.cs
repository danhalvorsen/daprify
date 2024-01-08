using Daprify.Models;
using DaprifyTests.Assert;

namespace DaprifyTests.Names
{
    [TestClass]
    public class TryName
    {
        [TestMethod]
        public void Expect_Constructor_WithValidName_ShouldSetName()
        {
            // Arrange
            string name = "John";

            // Act
            Name sut = new(name);

            // Assert
            Asserts.VerifyString(sut.ToString(), name);
        }

        [TestMethod]
        public void Expect_Constructor_WithNullName_ShouldThrowArgumentNullException()
        {
            // Arrange
            Name sut;

            // Act
            void act() => sut = new(null!);

            // Assert
            Asserts.VerifyException<ArgumentNullException>(act);
        }

        [TestMethod]
        public void Constructor_WithEmptyName_ShouldThrowArgumentNullException()
        {
            //Arrange 
            Name sut;

            // Act
            void act() => sut = new(string.Empty);

            // Assert
            Asserts.VerifyException<ArgumentNullException>(act);
        }

        [TestMethod]
        public void SetName_WithValidName_ShouldUpdateName()
        {
            // Arrange
            Name sut = new();
            string name = "Alice";

            // Act
            sut.SetName(name);

            // Assert
            Asserts.VerifyString(sut.ToString(), name);
        }

        [TestMethod]
        public void Expect_SetName_WithNullName_ShouldThrowArgumentNullException()
        {
            // Arrange
            Name sut = new();

            // Act
            void act() => sut.SetName(null!);

            // Assert
            Asserts.VerifyException<ArgumentNullException>(act);
        }

        [TestMethod]
        public void Expect_SetName_WithEmptyName_ShouldThrowArgumentNullException()
        {
            // Arrange
            Name sut = new();

            // Act
            void act() => sut.SetName(string.Empty);

            // Assert
            Asserts.VerifyException<ArgumentNullException>(act);
        }

        [TestMethod]
        public void Expect_GetName_ShouldReturnSelf()
        {
            // Arrange
            Name sut = new("Bob");

            // Act
            Name retrievedName = sut.GetName();

            // Assert
            Asserts.VerifyString(sut.ToString(), retrievedName.ToString());
        }
    }
}