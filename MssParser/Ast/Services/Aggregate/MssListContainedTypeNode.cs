using Irony.Ast;
using Irony.Parsing;
using Mss.Ast.Visitor;

namespace Mss.Ast
{
    public class MssListContainedTypeNode : MssNode
    {
        public string ContainedType { get; set; } = "";

        public override void Accept(IMssAstVisitor visitor)
        {
            visitor.Visit(this);
        }

        public override void Init(AstContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);

            int EXPECTED_CHILD_COUNT = 1;
            if (Children.Count == 0)
            {
                Token = treeNode.ChildNodes[0].Token;
                ContainedType = Token.Text;
            }
            else if (Children.Count == EXPECTED_CHILD_COUNT)
            {
                if (Children[0] is MssIdentifierNode identifier)
                {
                    ContainedType = identifier.Identifier;
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
        }
    }
}