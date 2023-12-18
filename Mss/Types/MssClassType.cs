namespace Mss.Types
{
    public class MssClassType(string identifier, MssField field) : MssType(identifier)
    {
        private readonly MssField _field = field;

        public string Name { get => _identifier; }
        public MssField Field { get => _field; }

        public override bool IsSameType(MssType type)
        {
            if (type is MssClassType c && c.Name == _identifier)
            {
                if (!_field.Type.IsSameType(c.Field.Type))
                {
                    return false;
                }
                return true;
            }

            return false;
        }
    }
}