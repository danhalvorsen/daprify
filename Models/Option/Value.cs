
namespace Daprify.Models
{
    public class Value
    {
        private string _value;

        public Value(string value)
        {
            SetValue(value);
        }

        private void SetValue(string value)
        {
            _value = value ?? throw new ArgumentNullException(nameof(value), "The option value cannot be null!");
        }

        public override string ToString()
        {
            return _value;
        }

        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        public override bool Equals(object? obj)
        {
            return obj is Value other && _value == other._value;
        }
    }
}