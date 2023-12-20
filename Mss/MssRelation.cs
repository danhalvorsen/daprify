using System.Diagnostics;

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

        public bool ContainsEntity(MssEntity entity)
        {
            if (FromEntity == entity || ToEntity == entity)
            {
                return true;
            }
            return false;
        }

        public bool ContainsField(MssField field)
        {
            if (FromField == field || ToField == field)
            {
                return true;
            }
            return false;
        }

        public MssField? GetOppositeField(MssField field)
        {
            if (FromField == field)
            {
                return ToField;
            }
            else if (ToField == field)
            {
                return FromField;
            }
            else
            {
                return null;
            }
        }

        public MssEntity? GetOppositeEntity(MssEntity entity)
        {
            if (FromEntity == entity)
            {
                return ToEntity;
            }
            else if (ToEntity == entity)
            {
                return FromEntity;
            }
            else
            {
                return null;
            }
        }

        public MssRelationKind? GetRelation(MssField field)
        {
            if (field == FromField)
            {
                return FromRelation;
            }
            else if (field == ToField)
            {
                return ToRelation;
            }
            else
            {
                return null;
            }
        }

    }
}