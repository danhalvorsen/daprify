using System.Xml.Linq;

namespace MssBuilder.Projects
{
    public class MssWebApiProject : MssCSharpProject
    {
        public override Guid TypeGuid { get => _aspCoreProjectUUID; }

        public MssWebApiProject(string name, string path) : base(name, path)
        {
            _useWebSdk = true;
        }

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