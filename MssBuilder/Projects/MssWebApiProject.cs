using System.Xml.Linq;

namespace MssBuilder.Projects
{
    public class MssWebApiProject(string name, string path) : MssCSharpProject(name, path)
    {
        protected override string CreateProjectFile()
        {
            XElement header = CreateProjectHeader();
            header.Add(CreatePropertyGroup());

            if (_projectReferences.Count > 0)
            {
                header.Add(CreateProjectReferences());
            }
            return header.ToString();
        }
    }
}