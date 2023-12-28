using CLI.Models;
using Moq;

namespace CLITests.Mocks
{
    public class MockISolution : Mock<ISolution>
    {
        public MockISolution()
        {
            Setup(x => x.GetPath()).Returns(new MyPath("/TestPath"));
        }
    }
}