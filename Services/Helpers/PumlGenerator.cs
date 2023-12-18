using System.Text;
using Mss;
using Mss.Types;

namespace CLI.Services
{
    public static class PumlGenerator
    {
        private static string GetServiceRef(string serviceName) => serviceName.ToLowerInvariant() + "_service";
        private static string GetDbRef(string serviceRef) => serviceRef + "_db";
        private static string GetEntityRef(string dbRef, string entityName) => dbRef + "_" + entityName.ToLowerInvariant();
        private static string GetAggregateRef(string serviceRef) => serviceRef + "_agg";

        private static void GenerateField(StringBuilder builder, MssField field, string indent)
        {
            builder.AppendLine(indent + field.Name + " : " + field.Type);
        }

        private static void GenerateExternClass(StringBuilder builder, MssExternType type, string indent)
        {
            var nameSpace = string.Join(".", type.NamespaceAccess);
            builder.AppendLine(indent + @$"class ""{nameSpace}.{type.Name}"" as {type.Name} {{");
            builder.AppendLine(indent + "}\n");
        }

        private static void GenerateExternProject(StringBuilder builder, string projectName, List<MssExternType> types, string indent)
        {
            builder.AppendLine(indent + $"package {projectName} {{");
            foreach (var type in types)
            {
                GenerateExternClass(builder, type, indent + "    ");
            }
            builder.AppendLine(indent + "}\n");
        }

        private static void GenerateExterns(StringBuilder builder, IEnumerable<MssType> types)
        {
            builder.AppendLine(@"package ""Extern Types"" as extern_types << Rectangle >> {");
            Dictionary<string, List<MssExternType>> externDict = [];
            foreach (var type in types)
            {
                if (type is MssExternType externType)
                {
                    if (externDict.TryGetValue(externType.Project, out List<MssExternType>? typeList) && typeList != null)
                    {
                        typeList.Add(externType);
                    }
                    else
                    {
                        externDict.Add(externType.Project, [externType]);
                    }
                }
            }

            foreach (var (project, externTypes) in externDict)
            {
                GenerateExternProject(builder, project, externTypes, "    ");
            }
            builder.AppendLine("}\n");
        }

        private static void GenerateClass(StringBuilder builder, MssClassType type, string indent)
        {
            //builder.AppendLine(indent + @$"class ""{type.Name}"" as {type.Name} {{");
            builder.AppendLine(indent + $"class {type.Name} {{");
            GenerateField(builder, type.Field, indent + "    ");
            builder.AppendLine(indent + "}\n");
        }

        private static void GenerateClasses(StringBuilder builder, IEnumerable<MssType> types)
        {
            builder.AppendLine(@"package ""HelperTypes"" as helper_types {");
            foreach (var type in types)
            {
                if (type is MssClassType classType)
                {
                    GenerateClass(builder, classType, "    ");
                }
            }
            builder.AppendLine("}\n");
        }

        private static void GenerateRoot(StringBuilder builder, MssEntity root, string service, string dbRef, string indent)
        {
            var rootRef = GetEntityRef(dbRef, root.Name);
            builder.AppendLine(indent + @$"entity ""{service}"" as {rootRef} << (R, SkyBlue) >> {{");
            foreach (var field in root.Fields)
            {
                GenerateField(builder, field, indent + "    ");
            }
            builder.AppendLine(indent + "}\n");
        }

        private static void GenerateEntity(StringBuilder builder, MssEntity entity, string dbRef, string indent)
        {
            var entityRef = GetEntityRef(dbRef, entity.Name);
            builder.AppendLine(indent + @$"entity ""{entity.Name}"" as {entityRef} {{");
            foreach (var field in entity.Fields)
            {
                GenerateField(builder, field, indent + "    ");
            }
            builder.AppendLine(indent + "}\n");
        }

        private static void GenerateRelation(StringBuilder builder, MssRelation rel, string dbRef, string indent)
        {
            var fromRef = GetEntityRef(dbRef, rel.FromEntity.Name);
            var toRef = GetEntityRef(dbRef, rel.ToEntity.Name);
            builder.AppendLine(indent + fromRef + "::" + rel.FromField.Name + " " + rel.FromRelation.FromString() +
                               rel.ToRelation.ToString() + " " + toRef + "::" + rel.ToField.Name);
        }

        private static void GenerateDatabase(StringBuilder builder, MssDatabase database,
                                             string service, string serviceRef, string indent)
        {
            var dbRef = GetDbRef(serviceRef);
            builder.AppendLine(indent + @$"database ""Database"" as {dbRef} {{");
            GenerateRoot(builder, database.Root, service, dbRef, indent + "    ");
            foreach (var entity in database.Entities)
            {
                GenerateEntity(builder, entity, dbRef, indent + "    ");
            }

            foreach (var rel in database.Relations)
            {
                GenerateRelation(builder, rel, dbRef, indent + "    ");
            }

            builder.AppendLine(indent + "}\n");
        }

        private static void GenerateAggregate(StringBuilder builder, List<MssAggregateField> fields,
                                              string service, string serviceRef, string indent, bool withAggRel)
        {
            var aggRef = GetAggregateRef(serviceRef);
            builder.AppendLine(indent + @$"entity ""{service}"" as {aggRef} << (A, Orange) >> {{");
            foreach (var field in fields)
            {
                GenerateField(builder, field.Field, indent + "    ");
            }
            builder.AppendLine(indent + "}\n");

            if (withAggRel)
            {
                var dbRef = GetDbRef(serviceRef);
                foreach (var field in fields)
                {
                    var fromEntityRef = aggRef;
                    var fromFieldRef = field.Field.Name;

                    var (toEntity, toField) = field.Mapping.Mapping.LastOrDefault()!;
                    var toEntityRef = GetEntityRef(dbRef, toEntity.Name);
                    var toFieldRef = toField.Name;

                    builder.AppendLine(indent + fromEntityRef + "::" + fromFieldRef + " --> " + toEntityRef + "::" + toFieldRef);
                }
            }
        }

        private static void GenerateService(StringBuilder builder, MssService service, bool withAggRel, string indent)
        {
            var serviceRef = GetServiceRef(service.Name);
            builder.AppendLine(@$"package ""{service.Name} Service"" as {serviceRef} {{");

            GenerateDatabase(builder, service.Database, service.Name, serviceRef, indent + "    ");
            GenerateAggregate(builder, service.AggregateFields, service.Name, serviceRef, indent + "    ", withAggRel);

            builder.AppendLine("}\n");
        }

        private static void GenerateServices(StringBuilder builder, IEnumerable<MssService> services, bool withAggRel)
        {
            builder.AppendLine(@"package ""Services"" as services <<Rectangle>> {");
            foreach (var service in services)
            {
                GenerateService(builder, service, withAggRel, "    ");
            }
            builder.AppendLine("}\n");
        }

        public static string Generate(string name, MssSpec spec, bool withAggRel)
        {
            var builder = new StringBuilder();

            builder.AppendLine("@startuml " + name);
            builder.AppendLine("left to right direction");
            GenerateExterns(builder, spec.Types);
            GenerateClasses(builder, spec.Types);
            GenerateServices(builder, spec.Services, withAggRel);
            builder.AppendLine("@enduml");

            return builder.ToString();
        }
    }
}