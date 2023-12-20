using CLI.Templates;
using Mss;
using Mss.Types;
using MssBuilder.Projects;
using System.Diagnostics;

namespace MssBuilder
{
    public class MssServiceBuilder
    {
        private readonly PropertyTemplate _propertyTemplate = new();
        private readonly EntityTemplate _entityTemplate = new();
        private readonly UsingTemplate _usingTemplate = new();
        private const string ENTITY_FOLDER = "Entities";

        private bool _serviceUsesValueTypes = false;

        private (bool, string) BuildEntityProperties(IEnumerable<MssField> fields)
        {
            List<string> properties = [];
            bool hasValueType = false;

            foreach (var field in fields)
            {
                var typeStr = field.Type.ToString();
                if (field.Type is MssClassType classType)
                {
                    hasValueType = true;
                }
                else if (field.Type is MssKeyType keyType)
                {
                    // Todo: use relations to figure out types
                    typeStr = "int";
                }
                properties.Add(_propertyTemplate.Render(field.Name, typeStr));
            }

            return (hasValueType, string.Join("\n", properties));
        }

        private MssCSharpFile BuildEntityFile(string name, IEnumerable<MssField> fields, string projectName)
        {
            string filename = ENTITY_FOLDER + "/" + name + ".cs";
            string nameSpace = projectName + "." + ENTITY_FOLDER;

            (bool usesValueTypes, string properties) = BuildEntityProperties(fields);
            if (usesValueTypes)
            {
                _serviceUsesValueTypes = true;
            }

            string content = _entityTemplate.Render(nameSpace, name, properties);
            if (usesValueTypes)
            {
                content = _usingTemplate.Render(MssValueTypeBuilder.ProjectName) + "\n\n" + content;
            }
            return new(filename, content);
        }

        private MssCSharpFile BuildEntityFile(MssEntity entity, string projectName)
        {
            return BuildEntityFile(entity.Name, entity.Fields, projectName);
        }

        private MssCSharpFile BuildRootFile(MssEntity root, string projectName)
        {
            return BuildEntityFile(projectName + "Root", root.Fields, projectName);
        }

        public MssWebApiProject Build(MssService service)
        {
            _serviceUsesValueTypes = false;

            MssWebApiProject project = new(service.Name, service.Name);

            foreach (var entity in service.Database.Entities)
            {
                project.AddFile(BuildEntityFile(entity, service.Name));
            }

            project.AddFile(BuildRootFile(service.Database.Root, service.Name));

            if (_serviceUsesValueTypes)
            {
                project.AddProjectReference(MssValueTypeBuilder.ProjectName);
            }

            // generate entities ignoring relations
            // add relations to entities as necessary

            return project;
        }
    }
}