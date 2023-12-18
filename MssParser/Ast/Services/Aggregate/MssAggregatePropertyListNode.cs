using Irony.Ast;
using Irony.Parsing;
using Mss.Ast.Visitor;

namespace Mss.Ast
{
    public class MssAggregatePropertyListNode : MssNode
    {
        public List<MssAggregatePropertyNode> Properties { get; set; } = [];

        public override void Accept(IMssAstVisitor visitor)
        {
            visitor.Visit(this);
        }

        public override void Init(AstContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);

            foreach (var child in Children)
            {
                if (child is MssAggregatePropertyNode prop)
                {
                    Properties.Add(prop);
                }
                else
                {
                    throw new InvalidChildTypeException();
                }
            }
        }
    }
}