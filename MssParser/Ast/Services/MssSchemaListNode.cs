using Irony.Ast;
using Irony.Parsing;
using Mss.Ast.Visitor;

namespace Mss.Ast
{
    public class MssSchemaListNode : MssNode
    {
        public readonly List<MssEntityNode> Entities = [];
        public readonly List<MssRelationNode> Relations = [];

        public override void Accept(IMssAstVisitor visitor)
        {
            visitor.Visit(this);
        }

        public override void Init(AstContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);

            foreach (var schema in Children)
            {
                const int EXPECTED_CHILD_COUNT = 1;
                if (schema.Children.Count == EXPECTED_CHILD_COUNT)
                {
                    if (schema.Children[0] is MssEntityNode entity)
                    {
                        Entities.Add(entity);
                    }
                    else if (schema.Children[0] is MssRelationNode relation)
                    {
                        Relations.Add(relation);
                    }
                    else
                    {
                        throw new InvalidChildTypeException();
                    }
                }
                else
                {
                    throw new InvalidChildCountException(EXPECTED_CHILD_COUNT, schema.Children.Count);
                }
            }
            AsString = "SchemaListNode";
        }
    }
}