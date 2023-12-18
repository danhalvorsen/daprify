using Irony.Ast;
using Irony.Parsing;
using Mss.Ast.Visitor;

namespace Mss.Ast
{
    public class MssAggregatePropertyAutoNode : MssNode
    {
        public string Identifier { get; set; } = "";
        public string Type { get; set; } = "";
        public bool IsList { get; set; } = false;
        public List<string> Access { get; set; } = [];

        public override void Accept(IMssAstVisitor visitor)
        {
            visitor.Visit(this);
        }

        public override void Init(AstContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);

            const int EXPECTED_CHILD_COUNT = 2;
            if (Children.Count == EXPECTED_CHILD_COUNT)
            {
                if (Children[0] is MssIdentifierNode identifier)
                {
                    Identifier = identifier.Identifier;
                }
                else
                {
                    throw new InvalidChildTypeException();
                }

                if (Children[1] is MssAggregatePropertyTypeNode type)
                {
                    Type = type.Type;
                    IsList = type.IsList;
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

            Access = ["root", Identifier];
        }
    }
}