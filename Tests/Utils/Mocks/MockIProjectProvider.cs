using CLI.Models;
using Moq;

namespace CLITests.Mocks
{
    public class MockIProjectProvider : Mock<IProjectProvider>
    {
        private readonly MockIQuery _queryMock;
        public MockIProjectProvider()
        {
            Setup(m => m.GetProjects(It.IsAny<Solution>())).Returns(new List<Project>());
        }

        public MockIProjectProvider(MockIQuery queryMock, int numberOfProjects)
        {
            _queryMock = queryMock;
            List<Project> projects = [];
            for (int i = 0; i < numberOfProjects; i++)
            {
                projects.Add(new Project(_queryMock.Object, new($"Project{i}")));
            }

            Setup(m => m.GetProjects(It.IsAny<Solution>())).Returns(projects);
        }
    }
}