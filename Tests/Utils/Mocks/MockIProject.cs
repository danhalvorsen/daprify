using System.Xml.Linq;
using Daprify.Models;
using Moq;

namespace DaprifyTests.Mocks
{
    public class MockIProject : Mock<IProject>
    {
        public MockIProject()
        {
            Setup(p => p.GetProject()).Returns(new XDocument());
        }

        public void SetupDependencyExists(string dependency)
        {
            XDocument mockProjectXml = new(
                new XElement("Project",
                    new XElement("ItemGroup",
                        new XElement("PackageReference", new XAttribute("Include", dependency))
                    )
                )
            );
            Setup(p => p.GetProject()).Returns(mockProjectXml);
        }

        public void SetupDependencyDoesNotExist(string dependency)
        {
            XDocument mockProjectXml = new(
                new XElement("Project",
                    new XElement("ItemGroup",
                        new XElement("PackageReference", new XAttribute("Include", "SomeOtherDependency"))
                    )
                )
            );
            Setup(p => p.GetProject()).Returns(mockProjectXml);
        }
    }
}
