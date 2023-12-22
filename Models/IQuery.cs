using System.Xml.Linq;

namespace CLI.Models
{
    public interface IQuery
    {
        public bool CheckPackageReference(Project project, string dependency);
        public string? GetFileInDirectory(string dirPath, string fileType);
    }

    public class Query : IQuery
    {
        public bool CheckPackageReference(Project project, string dependency)
        {
            XDocument proj = project.GetProject();
            return proj
                    .Descendants("PackageReference")
                    .Any(pr => pr.Attribute("Include")?.Value
                    .Contains(dependency, StringComparison.OrdinalIgnoreCase) ?? false);
        }

        public string? GetFileInDirectory(string dirPath, string fileType)
        {
            return Directory.GetFiles(dirPath, fileType).FirstOrDefault();
        }
    }
}