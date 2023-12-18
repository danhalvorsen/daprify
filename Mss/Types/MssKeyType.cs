namespace Mss.Types
{
    public class MssKeyType(string identifier) : MssType(identifier)
    {
        public override bool IsSameType(MssType type)
        {
            if (type is MssKeyType key)
            {
                return ToString() == type.ToString();
            }

            return false;
        }
    }
}