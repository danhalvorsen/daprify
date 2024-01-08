namespace Daprify.Models
{
    public class OptionValues
    {
        private IEnumerable<Value> _values;
        private Key _key;

        public OptionValues(Key key)
        {
            _key = key;
        }

        public OptionValues(Key key, List<string> stringValues)
        {
            _key = key;
            _values = stringValues.Select(v => new Value(v)) ?? throw new ArgumentNullException(nameof(stringValues), "The option values cannot be null!");
        }

        public void SetKey(Key key)
        {
            _key = key;
        }

        public IEnumerable<Value> GetValues()
        {
            return _values;
        }

        public Key GetKey()
        {
            return _key;
        }

        public int Count()
        {
            return _values.Count();
        }

        public void RemoveValue(Value value)
        {
            _values = _values.Where(v => !v.Equals(value)).ToList();
        }

        public IEnumerable<string> GetStringEnumerable()
        {
            return _values.Select(v => v.ToString());
        }
    }
}