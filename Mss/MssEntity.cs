namespace Mss
{
    public class MssEntity(string name, List<MssField> fields)
    {
        public string Name { get; } = name;
        public readonly List<MssField> Fields = fields;

        public MssField? GetField(string name)
        {
            return Fields.FirstOrDefault(f => f.Name == name);
        }
    }
}