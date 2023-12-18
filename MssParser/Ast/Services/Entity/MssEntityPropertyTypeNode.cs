using Irony.Ast;
using Irony.Parsing;
using Mss.Ast.Visitor;

namespace Mss.Ast
{
    public class MssEntityPropertyTypeNode : MssNode
    {
        public string Type { get; set; } = "";

        public override void Init(AstContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);
            if (Children.Count > 0)
            {
                const int EXPECTED_CHILD_COUNT = 1;
                if (Children.Count == EXPECTED_CHILD_COUNT)
                {
                    if (Children[0] is MssIdentifierNode identifier)
                    {
                        Type = identifier.Identifier;
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
            else
            {
                Token = treeNode.ChildNodes[0].Token;
                Type = Token.Text;
                AsString = "EntityPropertyType: " + Token.Text;
            }
        }

        public override void Accept(IMssAstVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}