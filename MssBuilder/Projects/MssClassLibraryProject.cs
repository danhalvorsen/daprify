using System.Xml.Linq;

namespace MssBuilder.Projects
{
    public class MssClassLibraryProject(string name, string path) : MssCSharpProject(name, path)
    {
        protected override string CreateProjectFile()
        {
            XElement header = CreateProjectHeader();
            header.Add(CreatePropertyGroup());

            if (_packageReferences.Count > 0)
            {
                header.Add(CreatePackageReferences());
            }

            if (_projectReferences.Count > 0)
            {
                header.Add(CreateProjectReferences());
            }

            return header.ToString();
        }
    }
}