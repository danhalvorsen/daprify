using Mss.Ast.Visitor;

namespace Mss.Ast
{
    public class MssSpecListNode : MssNode
    {
        public override void Accept(IMssAstVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}