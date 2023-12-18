using Irony.Ast;
using Irony.Parsing;
using Mss.Ast.Visitor;

namespace Mss.Ast
{
    public class MssAccessListNode : MssNode
    {
        public List<string> Access { get; set; } = [];

        public override void Accept(IMssAstVisitor visitor)
        {
            visitor.Visit(this);
        }

        public override void Init(AstContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);
            foreach (var child in Children)
            {
                if (child is MssIdentifierNode identifier)
                {
                    Access.Add(identifier.Identifier);
                }
                else
                {
                    throw new InvalidChildTypeException();
                }
            }
        }
    }
}