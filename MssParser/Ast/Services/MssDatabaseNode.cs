using Irony.Ast;
using Irony.Parsing;
using Mss.Ast.Visitor;

namespace Mss.Ast
{
    public class MssDatabaseNode : MssNode
    {
        public MssRootNode Root { get; set; } = null!;
        public List<MssEntityNode> Entities { get; set; } = [];
        public List<MssRelationNode> Relations { get; set; } = [];

        public override void Accept(IMssAstVisitor visitor)
        {
            visitor.Visit(this);
        }

        public override void Init(AstContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);
            const int EXPECTED_CHILD_COUNT = 2;
            if (Children.Count == EXPECTED_CHILD_COUNT)
            {
                if (Children[0] is MssRootNode root)
                {
                    Root = root;
                }
                else
                {
                    throw new InvalidChildTypeException();
                }

                if (Children[1] is MssSchemaListNode schemaList)
                {
                    Entities = schemaList.Entities;
                    Relations = schemaList.Relations;
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
        }
    }
}