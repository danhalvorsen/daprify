using Irony.Ast;
using Irony.Parsing;
using Mss.Ast.Visitor;
using Mss.Resolver;

namespace Mss.Ast
{
    public class MssRelationStartNode : MssNode
    {
        public MssRelationKind Kind { get; set; } = null!;

        public override void Init(AstContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);
            const int EXPECTED_CHILD_COUNT = 1;
            if (treeNode.ChildNodes.Count == EXPECTED_CHILD_COUNT)
            {
                Token = treeNode.ChildNodes[0].Token;
                Kind = Token.Text switch
                {
                    "||-" => new MssRelationKindOne(),
                    "|o-" => new MssRelationKindZeroOrOne(),
                    "}-" => new MssRelationKindMany(),
                    "}o-" => new MssRelationKindZeroOrMany(),
                    _ => throw new InvalidTokenStringException("Invalid relation kind: " + Token.Text, Token.Location)
                };
                AsString = Token.Text;
            }
            else
            {
                throw new InvalidChildCountException(EXPECTED_CHILD_COUNT, treeNode.ChildNodes.Count);
            }
        }

        public override void Accept(IMssAstVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}