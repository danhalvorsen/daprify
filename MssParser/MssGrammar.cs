using Irony.Parsing;
using Mss.Ast;

namespace Mss.Parsing
{
    /*
       See Outline.md for grammar definition
    */
    public class MssGrammar : Grammar
    {
        public MssGrammar() : base(caseSensitive: true)
        {
            // Non-Terminals
            var spec = new NonTerminal("Spec", typeof(MssSpecNode));
            var specList = new NonTerminal("SpecList", typeof(MssSpecListNode));
            var specItem = new NonTerminal("SpecItem", typeof(MssSpecItemNode));

            var typeSpec = new NonTerminal("TypeSpec", typeof(MssTypeSpecNode));
            var typeDefList = new NonTerminal("TypeDefList", typeof(MssTypeListNode));
            var typeDef = new NonTerminal("TypeDef", typeof(MssTypeNode));

            var classDef = new NonTerminal("Class", typeof(MssClassNode));
            var classProperty = new NonTerminal("ClassProperty", typeof(MssClassPropertyNode));
            var builtInType = new NonTerminal("BuiltInType", typeof(MssBuiltInTypeNode));

            var externDef = new NonTerminal("Extern", typeof(MssExternNode));
            var accessList = new NonTerminal("AccessList", typeof(MssAccessListNode));

            var service = new NonTerminal("Service", typeof(MssServiceNode));
            var database = new NonTerminal("Database", typeof(MssDatabaseNode));
            var root = new NonTerminal("Root", typeof(MssRootNode));
            var schema = new NonTerminal("Schema", typeof(MssSchemaNode));
            var schemaList = new NonTerminal("SchemaList", typeof(MssSchemaListNode));

            var entity = new NonTerminal("Entity", typeof(MssEntityNode));
            var entityProperty = new NonTerminal("EntityProperty", typeof(MssEntityPropertyNode));
            var entityPropertyList = new NonTerminal("EntityPropertyList", typeof(MssEntityPropertyListNode));
            var entityPropertyType = new NonTerminal("EntityPropertyType", typeof(MssEntityPropertyTypeNode));

            var relation = new NonTerminal("Relation", typeof(MssRelationNode));
            var relationStart = new NonTerminal("RelationStart", typeof(MssRelationStartNode));
            var relationEnd = new NonTerminal("RelationEnd", typeof(MssRelationEndNode));

            var aggregate = new NonTerminal("Aggregate", typeof(MssAggregateNode));
            var aggregatePropertyList = new NonTerminal("AggregatePropertyList", typeof(MssAggregatePropertyListNode));
            var aggregateProperty = new NonTerminal("AggregateProperty", typeof(MssAggregatePropertyNode));
            var aggregatePropertyAuto = new NonTerminal("AggregatePropertyAuto", typeof(MssAggregatePropertyAutoNode));
            var aggregatePropertyMapped = new NonTerminal("AggregatePropertyMapped", typeof(MssAggregatePropertyMappedNode));
            var aggregatePropertyType = new NonTerminal("AggregatePropertyType", typeof(MssAggregatePropertyTypeNode));

            var listType = new NonTerminal("ListType", typeof(MssListTypeNode));
            var listContainedType = new NonTerminal("ListContainedType", typeof(MssListContainedTypeNode));

            // Terminals
            var identifier = new IdentifierTerminal("Identifier");
            identifier.AstConfig.NodeType = typeof(MssIdentifierNode);

            var intType = ToTerm("int");
            var floatType = ToTerm("float");
            var stringType = ToTerm("string");
            var boolType = ToTerm("bool");

            var primaryKey = ToTerm("PK");
            var foreignKey = ToTerm("FK");
            var externalKey = ToTerm("EK");

            var fromOneOrZero = ToTerm("|o-");
            var fromOne = ToTerm("||-");
            var fromManyOrZero = ToTerm("}o-");
            var fromMany = ToTerm("}-");
            var toOneOrZero = ToTerm("-o|");
            var toOne = ToTerm("-||");
            var toManyOrZero = ToTerm("-o{");
            var toMany = ToTerm("-{");

            // Comment Terminals
            var lineComment = new CommentTerminal("LineComment", "//", "\n", "\r\n");
            var multilineComment = new CommentTerminal("MultilineComment", "/*", "*/");

            NonGrammarTerminals.Add(lineComment);
            NonGrammarTerminals.Add(multilineComment);

            spec.Rule = specList;
            specList.Rule = MakePlusRule(specList, specItem);
            specItem.Rule = typeSpec | service;

            typeSpec.Rule = ToTerm("types") + "{" + typeDefList + "}";
            typeDefList.Rule = MakeStarRule(typeDefList, typeDef);
            typeDef.Rule = classDef | externDef;

            classDef.Rule = ToTerm("class") + identifier + "{" + classProperty + "}";
            classProperty.Rule = identifier + ":" + builtInType + ";";
            builtInType.Rule = intType | floatType | stringType | boolType;

            externDef.Rule = ToTerm("extern") + identifier + ":" + identifier + "::" + accessList + ";";
            accessList.Rule = MakeStarRule(accessList, ToTerm("."), identifier);

            service.Rule = ToTerm("service") + identifier + "{" + database + aggregate + "}";
            database.Rule = ToTerm("database") + "{" + root + schemaList + "}";
            root.Rule = ToTerm("root") + "{" + entityPropertyList + "}";
            schemaList.Rule = MakeStarRule(schemaList, schema);
            schema.Rule = entity | relation;

            entity.Rule = ToTerm("entity") + identifier + "{" + entityPropertyList + "}";
            entityPropertyList.Rule = MakePlusRule(entityPropertyList, entityProperty);
            entityProperty.Rule = identifier + ":" + entityPropertyType + ";";
            entityPropertyType.Rule = identifier | primaryKey | foreignKey | externalKey;

            relation.Rule = accessList + relationStart + relationEnd + accessList + ";";
            relationStart.Rule = fromOneOrZero | fromOne | fromManyOrZero | fromMany;
            relationEnd.Rule = toOneOrZero | toOne | toManyOrZero | toMany;

            aggregate.Rule = ToTerm("aggregate") + "{" + aggregatePropertyList + "}";
            aggregatePropertyList.Rule = MakePlusRule(aggregatePropertyList, aggregateProperty);
            aggregateProperty.Rule = aggregatePropertyAuto | aggregatePropertyMapped;
            aggregatePropertyAuto.Rule = identifier + ":" + aggregatePropertyType + ";";
            aggregatePropertyMapped.Rule = identifier + ":" + aggregatePropertyType + "=" + accessList + ";";
            aggregatePropertyType.Rule = externalKey | identifier | listType;

            listType.Rule = ToTerm("List") + "<" + listContainedType + ">";
            listContainedType.Rule = externalKey | identifier;

            this.Root = spec;
            this.LanguageFlags = LanguageFlags.CreateAst;
        }
    }
}