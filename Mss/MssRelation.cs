namespace Mss
{
    public class MssRelation(MssEntity fromEntity, MssField fromField,
                             MssRelationKind fromRelation, MssRelationKind toRelation,
                             MssEntity toEntity, MssField toField)
    {
        public MssEntity FromEntity { get; set; } = fromEntity;
        public MssField FromField { get; set; } = fromField;
        public MssRelationKind FromRelation { get; set; } = fromRelation;
        public MssRelationKind ToRelation { get; set; } = toRelation;
        public MssEntity ToEntity { get; set; } = toEntity;
        public MssField ToField { get; set; } = toField;
    }
}