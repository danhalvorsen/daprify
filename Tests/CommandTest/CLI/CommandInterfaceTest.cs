
using FluentAssertions;
using Moq;

namespace CLITests.Commands
{
    [TestClass]
    public class TryCommandInterface
    {
        private readonly string[] optionArgs = ["Service1 Service2"];

        [TestMethod]
        public void Expect_Command_Line_Argument_Sent_Read_Successfully()
        {
            // Arrange
            Mock<ICommandLineInterface> mockedCLI = new();
            mockedCLI.Setup(m => m.GetCommandLineArgs()).Returns(optionArgs);
            ICommandLineInterface target = mockedCLI.Object;

            // Act
            string[] args = target.GetCommandLineArgs();

            // Assert
            args.Should().NotBeNull();
            args.Should().ContainInOrder(optionArgs);
        }
    }
}