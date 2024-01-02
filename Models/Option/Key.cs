
namespace CLI.Models
{
    public class Key(string key)
    {
        private readonly string _key = key;

        public override string ToString()
        {
            return _key;
        }

        public override int GetHashCode()
        {
            return _key.GetHashCode();
        }

        public override bool Equals(object? obj)
        {
            return obj is Key other && _key == other._key;
        }
    }
}