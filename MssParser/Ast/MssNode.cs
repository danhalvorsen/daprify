using Irony.Ast;
using Irony.Interpreter.Ast;
using Irony.Parsing;
using Mss.Ast.Visitor;

namespace Mss.Ast
{
    public abstract class MssNode : AstNode
    {
        public readonly List<MssNode> Children = [];
        public Token Token { get; set; } = null!;

        public abstract void Accept(IMssAstVisitor visitor);

        public override void Init(AstContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);
            Token = treeNode.Token;

            var nodes = treeNode.GetMappedChildNodes().Select(n => n.AstNode);
            foreach (var node in nodes)
            {
                if (node is MssNode mnode)
                {
                    Children.Add(mnode);
                }
            }
        }
    }
}