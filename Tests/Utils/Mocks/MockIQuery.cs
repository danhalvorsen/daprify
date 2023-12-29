using CLI.Models;
using Moq;

namespace CLITests.Mocks
{
    public class MockIQuery : Mock<IQuery>
    {
        public MockIQuery()
        {
            Setup(m => m.CheckPackageReference(It.IsAny<Project>(), It.IsAny<string>())).Returns(true);
            Setup(m => m.GetFileInDirectory(It.IsAny<IPath>(), It.IsAny<string>())).Returns("TempSolution.sln");
        }
    }
}