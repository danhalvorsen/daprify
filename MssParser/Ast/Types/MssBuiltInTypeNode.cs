using Irony.Ast;
using Irony.Parsing;
using Mss.Ast.Visitor;

namespace Mss.Ast
{
    public class MssBuiltInTypeNode : MssNode
    {
        public string Type { get; set; } = "";
        public override void Init(AstContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);
            Token = treeNode.ChildNodes[0].Token;
            Type = Token.Text;
            AsString = "BuiltInType: " + Type;
        }

        public override void Accept(IMssAstVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

}