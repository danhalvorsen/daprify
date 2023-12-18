using System.Data;

namespace Mss
{
    public abstract class MssAggregateFieldMapping(List<(MssEntity, MssField)> mapping)
    {
        public List<(MssEntity, MssField)> Mapping { get; set; } = mapping;
    }

    public class MssAggregateFieldMappingDirect(List<(MssEntity, MssField)> mapping)
        : MssAggregateFieldMapping(mapping)
    {
    }

    public class MssAggregateFieldMappingWhere(
        List<(MssEntity, MssField)> mapping,
        MssEntity root, MssField rootField, MssEntity entity, MssField entityField)
        : MssAggregateFieldMapping(mapping)
    {
        public MssEntity Root { get; set; } = root;
        public MssField RootField { get; set; } = rootField;
        public MssEntity Entity { get; set; } = entity;
        public MssField EntityField { get; set; } = entityField;
    }

    public class MssAggregateField(MssField field, MssAggregateFieldMapping mapping)
    {
        public MssField Field { get; set; } = field;
        public MssAggregateFieldMapping Mapping { get; set; } = mapping;
    }
}