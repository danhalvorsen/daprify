namespace Mss
{
    public class MssService(string name, MssDatabase database, List<MssAggregateField> aggregateFields)
    {
        public string Name { get; } = name;
        public MssDatabase Database { get; } = database;
        public List<MssAggregateField> AggregateFields { get; } = aggregateFields;
    }
}