using Mss.Types;

namespace Mss
{
    public class MssSpec(IEnumerable<MssType> types, IEnumerable<MssService> services)
    {
        public IEnumerable<MssType> Types { get; } = types;
        public IEnumerable<MssService> Services { get; } = services;
    }
}