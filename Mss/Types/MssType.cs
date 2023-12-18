namespace Mss.Types
{
    public abstract class MssType(string identifier)
    {
        protected readonly string _identifier = identifier;
        public virtual bool IsValid => true;

        public override string ToString() => _identifier;
        public virtual bool IsSameType(MssType type) => ToString() == type.ToString();
    }

}