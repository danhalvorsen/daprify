using Irony.Ast;
using Irony.Parsing;
using Mss.Ast.Visitor;

namespace Mss.Ast
{
    public class MssRootNode : MssNode
    {
        public List<MssEntityPropertyNode> Properties = [];

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
                if (Children[0] is MssEntityPropertyListNode list)
                {
                    Properties = list.Properties;
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
            AsString = "Root";
        }
    }
}