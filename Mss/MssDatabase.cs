namespace Mss
{
    public class RelationQueryResult(MssEntity entity, MssField field, MssRelationKind kind)
    {
        public MssEntity Entity { get; } = entity;
        public MssField Field { get; } = field;
        public MssRelationKind Kind { get; } = kind;
    }

    public class MssDatabase(MssEntity root, List<MssEntity> entities, List<MssRelation> relations)
    {
        public readonly MssEntity Root = root;
        public readonly List<MssEntity> Entities = entities;
        public readonly List<MssRelation> Relations = relations;

        public MssEntity? GetEntity(string name)
        {
            return Entities.FirstOrDefault(e => e.Name == name);
        }

        public RelationQueryResult? GetFieldRelations(MssField field)
        {
            foreach (var rel in Relations)
            {
                if (rel.FromField == field)
                {
                    return new RelationQueryResult(rel.ToEntity, rel.ToField, rel.ToRelation);
                }
                else if (rel.ToField == field)
                {
                    return new RelationQueryResult(rel.FromEntity, rel.FromField, rel.FromRelation);
                }
            }
            return null;
        }

        public List<MssRelation> GetEntityRelations(MssEntity entity)
        {
            return Relations.Where(r => r.FromEntity == entity || r.ToEntity == entity).ToList();
        }
    }
}