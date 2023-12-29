namespace CLI.Models
{
    public class OptionValues
    {
        public IEnumerable<string> _values;

        public OptionValues(IEnumerable<string> values)
        {
            SetValues(values);
        }

        private void SetValues(IEnumerable<string> values)
        {
            _values = values;
        }

        public IEnumerable<string> GetValues()
        {
            return _values;
        }

        public int Count()
        {
            return _values.Count();
        }
    }
}