using System.Xml.Linq;

namespace MssBuilder.Projects
{
    public class MssWebApiProject(string name, string path) : MssCSharpProject(name, path)
    {
        public override Guid TypeGuid { get => _aspCoreProjectUUID; }

        protected override string CreateProjectFile()
        {
            XElement header = CreateProjectHeader();
            var propertyGrp = CreatePropertyGroup();
            propertyGrp.Add(new XElement("OutputType", "Exe"));
            header.Add(propertyGrp);

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