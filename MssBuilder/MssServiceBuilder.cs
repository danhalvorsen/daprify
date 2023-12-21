using CLI.Templates;
using Mss;
using Mss.Types;
using MssBuilder.Projects;
using System.Diagnostics;
using System.Text;

namespace MssBuilder
{
    public class MssServiceBuilder
    {
        private readonly PropertyTemplate _propertyTemplate = new();
        private readonly EntityTemplate _entityTemplate = new();
        private readonly UsingTemplate _usingTemplate = new();
        private readonly DbContextHeaderTemplate _dbContextHeaderTemplate = new();
        private readonly DbContextConstructorTemplate _dbContextConstructorTemplate = new();

        private string _serviceName = "";
        private const string ENTITY_FOLDER = "Entities";

        private bool _serviceUsesValueTypes = false;

        private string GetEntityName(MssEntity entity)
        {
            if (entity.Name == "root")
            {
                return _serviceName + "Root";
            }
            else
            {
                return entity.Name;
            }
        }

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

        private static bool IsFkPkPair(MssType a, MssType b)
        {
            if (a is MssKeyType keyA && b is MssKeyType keyB)
            {
                if ((keyA.ToString() == "PK" && keyB.ToString() == "FK") ||
                    (keyA.ToString() == "FK" && keyB.ToString() == "PK"))
                {
                    return true;
                }
            }
            return false;
        }

        private string BuildEntityRelationProperties(MssEntity entity, IEnumerable<MssField> fields,
                                                     IEnumerable<MssRelation> relations)
        {
            List<string> properties = [];

            foreach (var field in fields)
            {
                // primary key won't be added if part of a relation, so must be manually added here
                if (field.Type is MssKeyType && field.Type.ToString() == "PK")
                {
                    // Todo could add a to c# string
                    properties.Add(_propertyTemplate.Render(field.Name, "int"));
                }

                var rels = relations.Where(r => r.ContainsField(field)).ToList();
                Debug.Assert(rels.Count > 0);
                foreach (var rel in rels)
                {
                    var toField = rels[0].GetOppositeField(field)!;
                    var toEntity = rels[0].GetOppositeEntity(entity)!;
                    var fromRel = rels[0].GetRelation(field)!;
                    var toRel = rels[0].GetRelation(toField)!;

                    string toEntityName = GetEntityName(toEntity);
                    Debug.Assert(IsFkPkPair(field.Type, toField.Type));

                    if (toRel.IsMany)
                    {
                        properties.Add(_propertyTemplate.Render(toEntityName, $"List<{toEntityName}>"));
                    }
                    else
                    {
                        properties.Add(_propertyTemplate.Render(toEntityName, toEntityName));
                    }
                }
            }
            return "\n" + string.Join("\n", properties);
        }

        private MssCSharpFile BuildEntityFile(MssEntity entity, IEnumerable<MssRelation> relations)
        {
            string filename = ENTITY_FOLDER + "/" + GetEntityName(entity) + ".cs";
            string nameSpace = _serviceName + "." + ENTITY_FOLDER;

            List<MssField> fieldsWithRelation = [];
            List<MssField> fieldsWithoutRelation = [];
            foreach (var field in entity.Fields)
            {
                if (relations.Any(r => r.ContainsField(field)))
                {
                    fieldsWithRelation.Add(field);
                }
                else
                {
                    fieldsWithoutRelation.Add(field);
                }
            }

            string rel_properties = BuildEntityRelationProperties(entity, fieldsWithRelation, relations);

            (bool usesValueTypes, string properties) = BuildEntityProperties(fieldsWithoutRelation);
            if (usesValueTypes)
            {
                _serviceUsesValueTypes = true;
            }

            string content = _entityTemplate.Render(nameSpace, GetEntityName(entity), properties + rel_properties);
            if (usesValueTypes)
            {
                content = _usingTemplate.Render(MssValueTypeBuilder.ProjectName) + "\n\n" + content;
            }
            return new(filename, content);
        }

        private string BuildEntityModel(MssEntity entity, IEnumerable<MssRelation> relations)
        {
            StringBuilder modelBuilder = new();
            modelBuilder.AppendLine($"            modelBuilder.Entity<{GetEntityName(entity)}>(entity =>");
            modelBuilder.AppendLine("            {");

            string entityName = GetEntityName(entity);
            string indent = "                ";

            foreach (var field in entity.Fields)
            {
                if (field.Type is MssKeyType keyType && keyType.ToString() == "PK")
                {
                    modelBuilder.AppendLine($"{indent}entity.HasKey(e => e.{field.Name});");
                }
                else
                {
                    var rels = relations.Where(r => r.ContainsField(field));
                    if (rels.Any())
                    {
                        foreach (var rel in rels)
                        {
                            var toField = rel.GetOppositeField(field)!;
                            var toEntity = rel.GetOppositeEntity(entity)!;
                            var fromRel = rel.GetRelation(field)!;
                            var toRel = rel.GetRelation(toField)!;

                            string toEntityName = GetEntityName(toEntity);
                            Debug.Assert(IsFkPkPair(field.Type, toField.Type));

                            if (toRel.IsMany)
                            {
                                modelBuilder.AppendLine($"{indent}entity.HasMany(e => e.{toEntityName})");
                                if (fromRel.IsMany)
                                {
                                    modelBuilder.AppendLine($"{indent}    .WithMany(o => o.{entityName})");
                                }
                                else
                                {
                                    modelBuilder.AppendLine($"{indent}    .WithOne(o => o.{entityName})");
                                }
                            }
                            else
                            {
                                modelBuilder.AppendLine($"{indent}entity.HasOne(e => e.{toEntityName})");
                                if (fromRel.IsMany)
                                {
                                    modelBuilder.AppendLine($"{indent}    .WithMany(o => o.{entityName})");
                                }
                                else
                                {
                                    modelBuilder.AppendLine($"{indent}    .WithOne(o => o.{entityName})");
                                }
                            }
                            modelBuilder.AppendLine($"{indent}    .OnDelete(DeleteBehavior.Restrict);");
                        }
                    }
                    // Cannot own a built in type
                    else if (field.Type is not MssBuiltInType && field.Type is not MssKeyType)
                    {
                        modelBuilder.AppendLine($"{indent}entity.OwnsOne(e => e.{field.Name});");
                    }
                }
            }

            modelBuilder.AppendLine("            });");
            return modelBuilder.ToString();
        }

        private MssCSharpFile BuildDbContext(MssDatabase database, string projectName)
        {
            string className = projectName + "Context";
            StringBuilder contentBuilder = new();
            if (_serviceUsesValueTypes)
            {
                contentBuilder.Append(_usingTemplate.Render(MssValueTypeBuilder.ProjectName));
            }
            contentBuilder.Append(_dbContextHeaderTemplate.Render(projectName, className));

            string rootName = GetEntityName(database.Root);
            contentBuilder.Append($"        public DbSet<{rootName}> {rootName} {{ get; set; }}\n");
            foreach (var entity in database.Entities)
            {
                contentBuilder.Append($"        public DbSet<{entity.Name}> {entity.Name} {{ get; set; }}\n");
            }

            contentBuilder.Append(_dbContextConstructorTemplate.Render(projectName, className));

            contentBuilder.Append(
    @"
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

");

            contentBuilder.Append(BuildEntityModel(database.Root, database.Relations.Where(r => r.ContainsEntity(database.Root))));
            foreach (var entity in database.Entities)
            {
                contentBuilder.Append(BuildEntityModel(entity, database.Relations.Where(r => r.ContainsEntity(entity))));
            }

            contentBuilder.Append("        }\n    }\n}\n");

            return new(className + ".cs", contentBuilder.ToString());
        }

        public MssWebApiProject Build(MssService service)
        {
            _serviceUsesValueTypes = false;
            _serviceName = service.Name;

            MssWebApiProject project = new(_serviceName, _serviceName);

            foreach (var entity in service.Database.Entities)
            {
                project.AddFile(
                    BuildEntityFile(entity, service.Database.Relations.Where(r => r.ContainsEntity(entity))));
            }

            project.AddFile(
                BuildEntityFile(service.Database.Root,
                                service.Database.Relations.Where(r => r.ContainsEntity(service.Database.Root))));

            project.AddFile(new MssApiProgramFile(_serviceName));

            if (_serviceUsesValueTypes)
            {
                project.AddProjectReference(MssValueTypeBuilder.ProjectName);
            }

            project.AddFile(BuildDbContext(service.Database, _serviceName));

            project.AddPackageReference("Microsoft.EntityFrameworkCore", "8.0.0");
            project.AddPackageReference("Microsoft.EntityFrameworkCore.Design", "8.0.0");
            // Todo: delete, this is for early testing, should use a server!!!
            project.AddPackageReference("Microsoft.EntityFrameworkCore.InMemory", "8.0.0");

            return project;
        }
    }
}