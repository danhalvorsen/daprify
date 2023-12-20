using System.Xml.Linq;

namespace MssBuilder.Projects
{
    public class MssClassLibraryProject(string name, string path) : MssCSharpProject(name, path)
    {
        protected override string CreateProjectFile()
        {
            XElement header = CreateProjectHeader();
            header.Add(CreatePropertyGroup());
            header.Add(CreateProjectReferences());
            return header.ToString();
        }
    }
}