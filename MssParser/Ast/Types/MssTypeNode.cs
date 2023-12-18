using Mss.Ast.Visitor;

namespace Mss.Ast
{
    public class MssTypeNode : MssNode
    {
        public override void Accept(IMssAstVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}