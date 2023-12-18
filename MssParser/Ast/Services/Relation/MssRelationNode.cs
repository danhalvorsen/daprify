using Irony.Ast;
using Irony.Parsing;
using Mss.Ast.Visitor;

namespace Mss.Ast
{
    public class MssRelationNode : MssNode
    {
        public MssAccessListNode FromAccess { get; set; } = null!;
        public MssRelationKind RelationStart { get; set; } = null!;
        public MssRelationKind RelationEnd { get; set; } = null!;
        public MssAccessListNode ToAccess { get; set; } = null!;

        public override void Init(AstContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);
            const int EXPECTED_CHILD_COUNT = 4;
            if (Children.Count == EXPECTED_CHILD_COUNT)
            {
                if (Children[0] is MssAccessListNode from)
                {
                    FromAccess = from;
                }
                else
                {
                    throw new InvalidChildTypeException();
                }

                if (Children[1] is MssRelationStartNode start)
                {
                    RelationStart = start.Kind;
                }
                else
                {
                    throw new InvalidChildTypeException();
                }

                if (Children[2] is MssRelationEndNode end)
                {
                    RelationEnd = end.Kind;
                }
                else
                {
                    throw new InvalidChildTypeException();
                }

                if (Children[3] is MssAccessListNode to)
                {
                    ToAccess = to;
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
            AsString = "Relation";
        }
        public override void Accept(IMssAstVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}