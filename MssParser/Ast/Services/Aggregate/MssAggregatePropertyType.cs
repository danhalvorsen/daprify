using Irony.Ast;
using Irony.Parsing;
using Mss.Ast.Visitor;

namespace Mss.Ast
{
    public class MssAggregatePropertyTypeNode : MssNode
    {
        public string Type { get; set; } = "";
        public bool IsList { get; set; } = false;

        public override void Accept(IMssAstVisitor visitor)
        {
            visitor.Visit(this);
        }

        public override void Init(AstContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);

            const int EXPECTED_CHILD_COUNT = 1;
            if (Children.Count == 0)
            {
                const int EXPECTED_TREENODE_CHILD_COUNT = 1;
                if (treeNode.ChildNodes.Count == EXPECTED_TREENODE_CHILD_COUNT)
                {
                    Token = treeNode.ChildNodes[0].Token;
                    Type = Token.Text;
                }
                else
                {
                    throw new InvalidChildCountException(EXPECTED_TREENODE_CHILD_COUNT, treeNode.ChildNodes.Count);
                }
            }
            else if (Children.Count == EXPECTED_CHILD_COUNT)
            {
                if (Children[0] is MssIdentifierNode identifier)
                {
                    Token = identifier.Token;
                    Type = identifier.Identifier;
                }
                else if (Children[0] is MssListTypeNode list)
                {
                    Type = list.ContainedType;
                    IsList = true;
                }
                else
                {
                    throw new InvalidChildTypeException();
                }
            }
            else
            {
                throw new InvalidChildCountException(EXPECTED_CHILD_COUNT, Children.Count);
            }
            AsString = "AggregatePropertyTypeNode: " + Type;
        }
    }
}