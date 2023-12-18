using Irony.Ast;
using Irony.Parsing;
using Mss.Ast.Visitor;

namespace Mss.Ast
{
    public class MssIdentifierNode : MssNode
    {
        public string Identifier { get; set; } = "";

        public override void Init(AstContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);
            Identifier = Token.Text;
            AsString = "Identifier: " + Identifier;
        }

        public override void Accept(IMssAstVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}