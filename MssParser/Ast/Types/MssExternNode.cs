using Irony.Ast;
using Irony.Parsing;
using Mss.Ast.Visitor;

namespace Mss.Ast
{
    public class MssExternNode : MssNode
    {
        public string ClassName { get; set; } = "";
        public string ProjectName { get; set; } = "";
        public List<string> Namespace { get; set; } = [];

        public override void Accept(IMssAstVisitor visitor)
        {
            visitor.Visit(this);
        }

        public override void Init(AstContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);
            const int EXPECTED_CHILD_COUNT = 3;
            if (Children.Count == EXPECTED_CHILD_COUNT)
            {
                if (Children[0] is MssIdentifierNode className)
                {
                    ClassName = className.Identifier;
                }
                else
                {
                    throw new InvalidChildTypeException();
                }

                if (Children[1] is MssIdentifierNode projectName)
                {
                    ProjectName = projectName.Identifier;
                }
                else
                {
                    throw new InvalidChildTypeException();
                }

                if (Children[2] is MssAccessListNode access)
                {
                    Namespace = access.Access;
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
            AsString = "Extern: " + ClassName;
        }
    }
}