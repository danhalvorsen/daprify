namespace Mss.Ast.Visitor
{
    public interface IMssAstVisitor
    {
        void Visit(MssSpecNode node);
        void Visit(MssSpecListNode node);
        void Visit(MssSpecItemNode node);

        void Visit(MssTypeSpecNode node);
        void Visit(MssTypeListNode node);
        void Visit(MssTypeNode node);
        void Visit(MssClassNode node);

        void Visit(MssClassPropertyNode node);
        void Visit(MssBuiltInTypeNode node);

        void Visit(MssExternNode node);
        void Visit(MssAccessListNode node);

        void Visit(MssServiceNode node);
        void Visit(MssDatabaseNode node);
        void Visit(MssRootNode node);
        void Visit(MssSchemaNode node);
        void Visit(MssSchemaListNode node);

        void Visit(MssEntityNode node);
        void Visit(MssEntityPropertyNode node);
        void Visit(MssEntityPropertyListNode node);
        void Visit(MssEntityPropertyTypeNode node);

        void Visit(MssRelationNode node);
        void Visit(MssRelationStartNode node);
        void Visit(MssRelationEndNode node);

        void Visit(MssAggregateNode node);
        void Visit(MssAggregatePropertyListNode node);
        void Visit(MssAggregatePropertyNode node);
        void Visit(MssAggregatePropertyAutoNode node);
        void Visit(MssAggregatePropertyMappedNode node);
        void Visit(MssAggregatePropertyTypeNode node);
        void Visit(MssListTypeNode node);
        void Visit(MssListContainedTypeNode node);

        void Visit(MssIdentifierNode node);
    }
}