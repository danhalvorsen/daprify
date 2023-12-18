using Mss.Ast.Visitor;

namespace Mss.Ast
{
    public class MssSchemaNode : MssNode
    {
        public override void Accept(IMssAstVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}