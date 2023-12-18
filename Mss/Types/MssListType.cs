namespace Mss.Types
{
    public class MssListType(MssType subType) : MssType("List<" + subType.ToString() + ">")
    {
        private readonly MssType _subType = subType;
        public MssType SubType { get => _subType; }

        public override bool IsValid => _subType.IsValid;

        public override bool IsSameType(MssType type)
        {
            if (type is MssListType list)
            {
                return _subType.IsSameType(list.SubType);
            }
            return false;
        }
    }
}