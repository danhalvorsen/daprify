namespace Mss.Types
{
    public class MssBuiltInType(string identifier) : MssType(identifier)
    {
        public override bool IsSameType(MssType type)
        {
            if (type is MssBuiltInType builtin)
            {
                return ToString() == builtin.ToString();
            }

            return false;
        }
    }

}