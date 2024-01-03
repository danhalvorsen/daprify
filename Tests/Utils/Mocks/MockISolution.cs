using Daprify.Models;
using Moq;

namespace DaprifyTests.Mocks
{
    public class MockISolution : Mock<ISolution>
    {
        public MockISolution()
        {
            Setup(x => x.GetPath()).Returns(new MyPath("/TestPath"));
        }
    }
}