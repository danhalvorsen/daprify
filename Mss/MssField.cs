using Mss.Types;

namespace Mss
{
    public class MssField(string name, MssType type)
    {
        public string Name { get; } = name;
        public MssType Type { get; } = type;
    }
}