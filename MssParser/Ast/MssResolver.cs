using Irony.Parsing;
using Mss.Ast;
using Mss.Ast.Visitor;
using Mss.Types;

namespace Mss.Resolver
{
    public class MssResolver : IMssAstVisitor
    {
        public readonly List<MssResolverError> Errors = [];

        private readonly MssInvalidType _invalidType = new();
        private readonly List<MssBuiltInType> _builtInTypes =
        [
            new MssBuiltInType("int"),
            new MssBuiltInType("float"),
            new MssBuiltInType("bool"),
            new MssBuiltInType("string")
        ];

        public readonly MssKeyType _primaryKey = new("PK");
        public readonly MssKeyType _foreignKey = new("FK");
        public readonly MssKeyType _externKey = new("EK");

        public readonly List<MssKeyType> _keyTypes;

        private readonly List<MssClassType> _classTypes = [];
        public IEnumerable<MssClassType> Classes => _classTypes;

        private readonly List<MssExternType> _externTypes = [];
        public IEnumerable<MssExternType> Externs => _externTypes;

        private readonly List<MssListType> _listTypes = [];

        private const string ERROR_IDENTIFIER = "ErrorIdentifier";

        private readonly List<MssService> _services = [];
        public IEnumerable<MssService> Services => _services;

        public MssResolver()
        {
            _keyTypes = [_primaryKey, _foreignKey, _externKey];
        }

        public MssSpec GetSpec()
        {
            List<MssType> types = [];
            types.AddRange(_builtInTypes);
            types.AddRange(_keyTypes);
            types.AddRange(_classTypes);
            types.AddRange(_externTypes);
            types.AddRange(_listTypes);
            return new MssSpec(types, _services);
        }

        private string VerifyName(string name, SourceLocation location,
                                  string builtinMsg, string keyMsg, string classMsg,
                                  string externMsg, string serviceMsg)
        {
            if (name != ERROR_IDENTIFIER)
            {
                if (_builtInTypes.Select(t => t.ToString()).Contains(name))
                {
                    Errors.Add(new(builtinMsg, location));
                    return ERROR_IDENTIFIER;
                }

                if (_keyTypes.Select(k => k.ToString()).Contains(name))
                {
                    Errors.Add(new(keyMsg, location));
                    return ERROR_IDENTIFIER;
                }

                var classNames = _classTypes.Select(c => c.Name);
                if (classNames.Contains(name))
                {
                    Errors.Add(new(classMsg, location));
                    return ERROR_IDENTIFIER;
                }

                var externNames = _externTypes.Select(e => e.Name);
                if (externNames.Contains(name))
                {
                    Errors.Add(new(externMsg, location));
                    return ERROR_IDENTIFIER;
                }

                var serviceNames = Services.Select(s => s.Name);
                if (serviceNames.Contains(name))
                {
                    Errors.Add(new(serviceMsg, location));
                    return ERROR_IDENTIFIER;
                }
            }
            return name;
        }

        private string VerifyClassName(MssClassNode node)
        {
            var name = node.Identifier;
            return VerifyName(name, node.Location,
                    "Cannot give a class a built-in type name: " + name,
                    "Cannot give a class a key type name: " + name,
                    "Duplicate class name: " + name,
                    "Cannot give a class the same name as an extern: " + name,
                    "A class cannot have the same name as a service: " + name);
        }

        private string VerifyExternName(MssExternNode node)
        {
            var name = node.ClassName;
            return VerifyName(name, node.Location,
                    "Cannot give an extern a built-in type name: " + name,
                    "Cannot give an extern a key type name: " + name,
                    "Cannot give an extern the same name as a class: " + name,
                    "Duplicate extern name: " + name,
                    "An extern cannot have the same name as a service: " + name);
        }

        private string VerifyServiceName(MssServiceNode node)
        {
            var name = node.Identifier;
            return VerifyName(name, node.Location,
                    "Cannot give a class a built-in type name: " + name,
                    "Cannot give a class a key type name: " + name,
                    "Duplicate class name: " + name,
                    "Cannot give a class the same name as an extern: " + name,
                    "A class cannot have the same name as a service: " + name);
        }

        private MssType ResolveType(string typeName)
        {
            return
                (MssType?)_builtInTypes.FirstOrDefault(t => t.ToString() == typeName) ??
                (MssType?)_keyTypes.FirstOrDefault(t => t.ToString() == typeName) ??
                (MssType?)_externTypes.FirstOrDefault(t => t.Name == typeName) ??
                (MssType?)_classTypes.FirstOrDefault(t => t.Name == typeName) ??
                _invalidType;
        }

        private MssType ResolveType(MssClassPropertyNode prop)
        {
            var type = ResolveType(prop.Type);
            if (type.IsValid && type is not MssBuiltInType)
            {
                Errors.Add(new("Invalid class property type: " + prop.Type, prop.Location));
            }
            return type;
        }

        private MssType ResolveType(MssEntityPropertyNode prop)
        {
            var type = ResolveType(prop.Type);
            if (type.IsValid && type is not MssBuiltInType && type is not MssClassType &&
                type is not MssKeyType && type is not MssExternType)
            {
                Errors.Add(new("Invalid entity property type: " + prop.Type, prop.Location));
            }
            return type;
        }

        private MssListType GetListType(MssType subType)
        {
            foreach (var listType in _listTypes)
            {
                if (listType.SubType.IsSameType(subType))
                {
                    return listType;
                }
            }

            var newListType = new MssListType(subType);
            _listTypes.Add(newListType);
            return newListType;
        }

        private MssType ResolveType(MssAggregatePropertyNode prop)
        {
            var type = ResolveType(prop.Type);
            if (type.IsValid && type is not MssBuiltInType && type is not MssClassType &&
                type is not MssExternType && type != _externKey)
            {
                Errors.Add(new("Invalid aggregate property type: " + prop.Type, prop.Location));
            }
            if (prop.IsList)
            {
                type = GetListType(type);
            }
            return type;
        }

        private bool IsExternToPrimaryKeyConversion(MssType externKey, MssType primaryKey)
        {
            return externKey == _externKey && primaryKey == _primaryKey;
        }

        private bool TypesAreCompatible(MssType typeA, MssType typeB)
        {
            if (typeA.IsSameType(typeB))
            {
                return true;
            }
            return IsExternToPrimaryKeyConversion(typeA, typeB) ||
                IsExternToPrimaryKeyConversion(typeB, typeA);
        }

        private List<MssField> ResolveEntityFields(List<MssEntityPropertyNode> properties, SourceLocation location)
        {
            bool hasPrimaryKey = false;
            List<MssField> fields = [];
            foreach (var prop in properties)
            {
                var type = ResolveType(prop);
                if (type == _primaryKey)
                {
                    if (hasPrimaryKey == true)
                    {
                        Errors.Add(new("entity can only have 1 primary key: " + prop.Identifier, prop.Location));
                    }
                    hasPrimaryKey = true;
                }
                if (!type.IsValid)
                {
                    Errors.Add(new("Invalid type in entity field: " + prop.Type, prop.Location));
                }
                if (fields.Select(f => f.Name).Contains(prop.Identifier))
                {
                    Errors.Add(new("Duplicate field name in entity: " + prop.Identifier, prop.Location));
                }
                fields.Add(new MssField(prop.Identifier, type));
            }

            if (!hasPrimaryKey)
            {
                Errors.Add(new("entity must have a primary key", location));
            }

            return fields;
        }

        private MssAggregateField? ResolveDirectAggregateField(MssField aggField, MssEntity root, string rootFieldName,
                                                               SourceLocation location)
        {
            var rootField = root.GetField(rootFieldName);
            if (rootField == null)
            {
                Errors.Add(new("Aggregate field not found in root: " + aggField.Name, location));
            }
            else if (!TypesAreCompatible(aggField.Type, rootField.Type))
            {
                Errors.Add(new("Aggregate field is not the same type as mapped root field: " + aggField.Name, location));
            }
            else
            {
                return new MssAggregateField(aggField, new MssAggregateFieldMappingDirect([(root, rootField)]));
            }
            return null;
        }

        private MssAggregateField? ResolveLinkedAggregateField(MssField aggField, MssDatabase db, string rootFieldName,
                                                               string entityFieldName, SourceLocation location)
        {
            var rootField = db.Root.GetField(rootFieldName);
            if (rootField == null)
            {
                Errors.Add(new("Aggregate field not found in root: " + aggField.Name, location));
            }
            else
            {
                if (rootField.Type != _foreignKey)
                {
                    Errors.Add(new("Root field type must be foreign key to map aggregate to another entity: " + rootField.Name, location));
                }

                var rel = db.GetFieldRelations(rootField);
                if (rel != null)
                {
                    if (rel.Kind is not MssRelationKindOne)
                    {
                        Errors.Add(new("Not a to-one mapping: " + aggField.Name, location));
                    }
                    else
                    {
                        var mappedField = rel.Entity.GetField(entityFieldName);
                        if (mappedField == null)
                        {
                            Errors.Add(new("Aggregate field mapping points to non-existent field '" + entityFieldName + "' in entity: " + rel.Entity.Name, location));
                        }
                        else
                        {
                            return new MssAggregateField(aggField, new MssAggregateFieldMappingDirect([(db.Root, rootField), (rel.Entity, mappedField)]));
                        }
                    }
                }
            }
            return null;
        }

        MssAggregateField? ResolveListAggregateField(MssField aggField, MssDatabase db, string entityName, string fieldName,
                                                     SourceLocation location)
        {
            if (aggField.Type is MssListType list)
            {
                var entity = db.GetEntity(entityName);
                if (entity == null)
                {
                    Errors.Add(new("Aggregate mapping points to non-existent entity: " + entityName, location));
                    return null;
                }
                var field = entity.GetField(fieldName);
                if (field == null)
                {
                    Errors.Add(new("Aggregate mapping points to non-existent field '" + fieldName + "' in entity: " + entityName, location));
                    return null;
                }
                if (!TypesAreCompatible(list.SubType, field.Type))
                {
                    Errors.Add(new("Aggregate field list subtype is not the same type as mapped entity field: " + aggField.Name, location));
                    return null;
                }

                var relations = db.GetEntityRelations(entity);
                MssField? rootPK = null;
                MssField? relField = null;
                foreach (var rel in relations)
                {
                    if (rel.FromEntity == db.Root)
                    {
                        if ((rel.ToRelation is MssRelationKindMany ||
                             rel.ToRelation is MssRelationKindZeroOrMany)
                            && rel.FromField.Type == _primaryKey)
                        {
                            rootPK = rel.FromField;
                            relField = rel.ToField;
                            break;
                        }
                    }
                    if (rel.ToEntity == db.Root)
                    {
                        if ((rel.FromRelation is MssRelationKindMany ||
                             rel.FromRelation is MssRelationKindZeroOrMany)
                            && rel.ToField.Type == _primaryKey)
                        {
                            rootPK = rel.ToField;
                            relField = rel.FromField;
                            break;
                        }
                    }
                }

                if (rootPK == null || relField == null)
                {
                    Errors.Add(new("Aggregate mapping entity does not have a many-to-one relation to the root's primary key: " + entityName, location));
                    return null;
                }
                var mapper = new MssAggregateFieldMappingWhere([(entity, field)], db.Root, rootPK, entity, relField);
                return new MssAggregateField(aggField, mapper);
            }
            else
            {
                Errors.Add(new("Aggregate fields that do not directly map to root must be list types: " + aggField.Name, location));
                return null;
            }
        }

        private List<MssAggregateField> ResolveAggregateFields(MssAggregateNode node, MssDatabase db)
        {
            var errorCount = Errors.Count;
            List<MssAggregateField> aggFields = [];
            foreach (var prop in node.Properties)
            {
                var identifier = prop.Identifier;
                var type = ResolveType(prop);
                var field = new MssField(identifier, type);

                // check if there is a direct mapping to a root field
                if (prop.Access.Count == 2)
                {
                    if (prop.Access[0] == "root")
                    {
                        var aggField = ResolveDirectAggregateField(field, db.Root, prop.Access[1], prop.Location);
                        if (aggField != null)
                        {
                            aggFields.Add(aggField);
                        }
                        else
                        {
                            // errors have already been added
                            continue;
                        }
                    }
                    else
                    {
                        var aggField = ResolveListAggregateField(field, db, prop.Access[0], prop.Access[1], prop.Location);
                        if (aggField != null)
                        {
                            aggFields.Add(aggField);
                        }
                        else
                        {
                            // errors have already been added
                            continue;
                        }
                    }
                }
                else if (prop.Access.Count == 3)
                {
                    if (prop.Access[0] == "root")
                    {
                        var aggField = ResolveLinkedAggregateField(field, db, prop.Access[1], prop.Access[2], prop.Location);
                        if (aggField != null)
                        {
                            aggFields.Add(aggField);
                        }
                        else
                        {
                            // errors have already been added
                            continue;
                        }
                    }
                }
                else
                {
                    Errors.Add(new("Not an allowed mapping from aggregate field to root field: " + identifier, prop.Location));
                }

                if (Errors.Count > errorCount) { return []; }
            }

            return aggFields;
        }

        List<(MssEntity, MssField)> GetAllForeignKeys(IEnumerable<MssEntity> entities)
        {
            List<(MssEntity, MssField)> foreignKeys = [];
            foreach (var entity in entities)
            {
                foreach (var field in entity.Fields)
                {
                    if (field.Type == _foreignKey)
                    {
                        foreignKeys.Add((entity, field));
                    }
                }
            }
            return foreignKeys;
        }

        private void VisitChildren(MssNode node)
        {
            foreach (var child in node.Children)
            {
                child.Accept(this);
            }
        }

        // these nodes are only visited to delve deeper
        public void Visit(MssSpecNode node) => VisitChildren(node);
        public void Visit(MssSpecListNode node) => VisitChildren(node);
        public void Visit(MssSpecItemNode node) => VisitChildren(node);
        public void Visit(MssTypeSpecNode node) => VisitChildren(node);
        public void Visit(MssTypeListNode node) => VisitChildren(node);
        public void Visit(MssTypeNode node) => VisitChildren(node);

        public void Visit(MssClassNode node)
        {
            MssType type = ResolveType(node.Property.Type);
            if (!type.IsValid)
            {
                Errors.Add(new("Invalid type in class field: " + node.Property.Type, node.Property.Location));
            }

            _classTypes.Add(new MssClassType(VerifyClassName(node), new MssField(node.Property.Identifier, type)));
        }

        public void Visit(MssExternNode node)
        {
            _externTypes.Add(new MssExternType(VerifyExternName(node), node.ProjectName, [.. node.Namespace]));
        }

        public void Visit(MssServiceNode node)
        {
            var errorCount = Errors.Count;

            // first verify and add the entities
            List<MssEntity> entities = [];
            foreach (var entity in node.Database.Entities)
            {
                // verify the name
                if (entities.Select(e => e.Name).Contains(entity.Identifier))
                {
                    Errors.Add(new("Duplicate entity name in service: " + entity.Identifier, entity.Location));
                }
                if (entity.Identifier == "root")
                {
                    Errors.Add(new("Cannot name an entity root: " + entity.Identifier, entity.Location));
                }

                entities.Add(new MssEntity(entity.Identifier, ResolveEntityFields(entity.Properties, entity.Location)));
            }

            // then verify and add the root
            List<MssField> rootFields = ResolveEntityFields(node.Database.Root.Properties, node.Database.Root.Location);
            var root = new MssEntity("root", rootFields);

            if (Errors.Count > errorCount) { return; }

            // then verify and add the relations
            List<MssRelation> relations = [];
            foreach (var relation in node.Database.Relations)
            {
                if (relation.FromAccess.Access.Count != 2)
                {
                    Errors.Add(new("Relation from must have exactly 2 accessors", relation.FromAccess.Location));
                    continue;
                }
                else if (relation.ToAccess.Access.Count != 2)
                {
                    Errors.Add(new("Relation to must have exactly 2 accessors", relation.FromAccess.Location));
                    continue;
                }
                else
                {
                    var fromEntity = entities.FirstOrDefault(e => e.Name == relation.FromAccess.Access[0]);
                    if (fromEntity == null)
                    {
                        if (relation.FromAccess.Access[0] == "root")
                        {
                            fromEntity = root;
                        }
                        else
                        {
                            Errors.Add(new("Relation from entity not found: " + relation.FromAccess.Access[0], relation.FromAccess.Location));
                            continue;
                        }
                    }
                    var fromField = fromEntity.Fields.FirstOrDefault(f => f.Name == relation.FromAccess.Access[1]);
                    if (fromField == null)
                    {
                        Errors.Add(new("Relation from field not found in " + fromEntity.Name + ": " + relation.FromAccess.Access[1],
                                       relation.FromAccess.Location));
                        continue;
                    }

                    var toEntity = entities.FirstOrDefault(e => e.Name == relation.ToAccess.Access[0]);
                    if (toEntity == null)
                    {
                        if (relation.ToAccess.Access[0] == "root")
                        {
                            toEntity = root;
                        }
                        else
                        {
                            Errors.Add(new("Relation from entity not found: " + relation.ToAccess.Access[0], relation.ToAccess.Location));
                            continue;
                        }
                    }
                    var toField = toEntity.Fields.FirstOrDefault(f => f.Name == relation.ToAccess.Access[1]);
                    if (toField == null)
                    {
                        Errors.Add(new("Relation to field not found in " + toEntity.Name + ": " + relation.ToAccess.Access[1],
                                       relation.ToAccess.Location));
                        continue;
                    }

                    if ((fromField.Type != _primaryKey && toField.Type != _primaryKey) ||
                        (fromField.Type != _foreignKey && toField.Type != _foreignKey))
                    {
                        Errors.Add(new("A relation must be between an entity's primary key and another's foreign key", relation.Location));
                        continue;
                    }


                    relations.Add(new(fromEntity, fromField,
                                      relation.RelationStart, relation.RelationEnd,
                                      toEntity, toField));
                }
            }

            if (Errors.Count > errorCount) { return; }

            // check that all foreign keys have a relation
            var foreignKeys = GetAllForeignKeys(new List<MssEntity> { root }.Concat(entities));
            foreach (var (fkEntity, fkField) in foreignKeys)
            {
                if (!relations.Any(r => r.FromEntity == fkEntity && r.FromField == fkField) &&
                    !relations.Any(r => r.ToEntity == fkEntity && r.ToField == fkField))
                {
                    Errors.Add(new("Foreign key " + fkField.Name + " in " + fkEntity.Name + " must have a relation", node.Location));
                }
            }

            if (Errors.Count > errorCount) { return; }

            var db = new MssDatabase(root, entities, relations);

            // then verify and add the aggregate

            var aggFields = ResolveAggregateFields(node.Aggregate, db);

            _services.Add(new MssService(VerifyServiceName(node), db, aggFields));
        }

        // the resolver does not need to visit these nodes
        public void Visit(MssAccessListNode node) { }
        public void Visit(MssDatabaseNode node) { }
        public void Visit(MssRootNode node) { }
        public void Visit(MssSchemaNode node) { }
        public void Visit(MssSchemaListNode node) { }
        public void Visit(MssEntityNode node) { }
        public void Visit(MssEntityPropertyNode node) { }
        public void Visit(MssEntityPropertyListNode node) { }
        public void Visit(MssEntityPropertyTypeNode node) { }
        public void Visit(MssRelationNode node) { }
        public void Visit(MssRelationStartNode node) { }
        public void Visit(MssRelationEndNode node) { }
        public void Visit(MssAggregateNode node) { }
        public void Visit(MssAggregatePropertyListNode node) { }
        public void Visit(MssAggregatePropertyNode node) { }
        public void Visit(MssAggregatePropertyAutoNode node) { }
        public void Visit(MssAggregatePropertyMappedNode node) { }
        public void Visit(MssAggregatePropertyTypeNode node) { }
        public void Visit(MssListTypeNode node) { }
        public void Visit(MssListContainedTypeNode node) { }
        public void Visit(MssIdentifierNode node) { }
        public void Visit(MssClassPropertyNode node) { }
        public void Visit(MssBuiltInTypeNode node) { }
    }
}