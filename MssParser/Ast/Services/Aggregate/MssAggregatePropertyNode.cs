using Irony.Ast;
using Irony.Parsing;
using Mss.Ast.Visitor;

namespace Mss.Ast
{
    public class MssAggregatePropertyNode : MssNode
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

            const int EXPECTED_CHILD_COUNT = 1;
            if (Children.Count == EXPECTED_CHILD_COUNT)
            {
                if (Children[0] is MssAggregatePropertyAutoNode auto)
                {
                    Identifier = auto.Identifier;
                    Type = auto.Type;
                    IsList = auto.IsList;
                    Access = auto.Access;
                }
                else if (Children[0] is MssAggregatePropertyMappedNode mapped)
                {
                    Identifier = mapped.Identifier;
                    Type = mapped.Type;
                    IsList = mapped.IsList;
                    Access = mapped.Access;
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