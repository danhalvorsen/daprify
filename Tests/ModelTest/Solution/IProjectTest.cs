using Daprify.Models;
using DaprifyTests.Assert;
using DaprifyTests.Mocks;
using Moq;

namespace DaprifyTests.Projects
{
    [TestClass]
    public class TryIProject
    {
        private readonly MockIQuery _mockIQuery = new();
        private readonly Name _name = new("TempProject");


        [TestMethod]
        public void FromStringList_CreatesProjectsFromNames()
        {
            // Arrange
            List<Value> names = [new("TempProject"), new("TempProject2")];

            // Act
            IEnumerable<Project> sut = Project.FromStringList(names);

            // Assert
            Asserts.VerifyItemCount(sut, names.Count);
            for (int i = 0; i < names.Count; i++)
            {
                Asserts.VerifyShouldBe(sut.Select(p => p.GetName().ToString()).ElementAt(i), names.ElementAt(i).ToString());
            }
        }

        [TestMethod]
        public void SetPath_ValidPath_SetsPath()
        {
            // Arrange
            Project sut = new(_mockIQuery.Object, _name);
            MockIPath path = new();

            // Act
            sut.SetPath(path.Object);

            // Assert
            Asserts.VerifyShouldBe(sut.GetPath(), path.Object);
        }

        [TestMethod]
        public void SetPath_NullPath_ThrowsArgumentNullException()
        {
            // Arrange
            Project project = new(_mockIQuery.Object, _name);

            // Act & Assert
            void act() => project.SetPath(null!);

            // Assert
            Asserts.VerifyException<ArgumentNullException>(act);
        }

        [TestMethod]
        public void CheckPackageReference_DelegatesToQuery()
        {
            // Arrange
            string dependency = "Dapr";
            Project project = new(_mockIQuery.Object, _name);

            // Act
            project.CheckPackageReference(dependency);

            // Assert
            _mockIQuery.Verify(q => q.CheckPackageReference(project, dependency), Times.Once);
        }

        [TestMethod]
        public void Contains_WithValidTarget_ReturnsTrue()
        {
            // Arrange
            Project project = new(_mockIQuery.Object, _name);

            // Act & Assert
            Asserts.VerifyTrue(project.Contains("Temp"));
        }

        [TestMethod]
        public void Contains_WithValidTarget_ReturnsFalse()
        {
            // Arrange
            Project project = new(_mockIQuery.Object, _name);

            // Act & Assert
            Asserts.VerifyFalse(project.Contains("NotValid"));
        }

        [TestMethod]
        public void Contains_WithNullTarget_ThrowsArgumentNullException()
        {
            // Arrange
            Project project = new(_mockIQuery.Object, _name);

            // Act & Assert
            void act() => project.Contains(null!);

            // Assert
            Asserts.VerifyException<ArgumentNullException>(act);
        }

        [TestMethod]
        public void FromStringList_EmptyList_ReturnsNoProjects()
        {
            // Arrange
            List<Value> names = [];

            // Act
            IEnumerable<Project> sut = Project.FromStringList(names);

            // Assert
            Asserts.VerifyItemCount(sut, 0);
        }
    }
}