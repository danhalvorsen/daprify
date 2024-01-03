namespace Daprify.Models
{
    public class OptionValues
    {
        private IEnumerable<Value> _values;

        public OptionValues() { }

        public OptionValues(List<Value> values)
        {
            SetValues(values);
        }

        public OptionValues(List<string> stringValues)
        {
            _values = stringValues.Select(v => new Value(v)) ?? throw new ArgumentNullException(nameof(stringValues), "The option values cannot be null!");
        }

        private void SetValues(IEnumerable<Value> values)
        {
            _values = values;
        }

        public IEnumerable<Value> GetValues()
        {
            return _values;
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

        public bool Contain(Value value)
        {
            return _values.Contains(value);
        }
    }
}