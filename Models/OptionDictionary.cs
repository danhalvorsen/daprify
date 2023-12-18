using System.Collections;
using System.Text.Json;

namespace CLI.Models
{
    public class OptionDictionary() : IEnumerable<KeyValuePair<string, List<string>>>
    {
        private readonly Dictionary<string, List<string>> _optionDictionary = [];

        public void Add(string optionName, List<string> optionValues)
        {
            _optionDictionary[optionName] = optionValues;
        }

        public void Remove(string key, string value)
        {
            _ = _optionDictionary[key].Remove(value);
        }

        public List<string> GetAllValues()
        {
            List<string> allValues = [];
            foreach (var pair in _optionDictionary)
            {
                allValues.AddRange(pair.Value);
            }
            return allValues;
        }

        public List<string> GetAllPairValues(string optionName)
        {
            if (_optionDictionary.TryGetValue(optionName, out List<string>? value))
            {
                return value;
            }
            return [];
        }

        public static OptionDictionary PopulateFromJson(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"The config file at {filePath} was not found");
            }

            string json = File.ReadAllText(filePath);
            Dictionary<string, List<string>> optionsDict = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(json) ??
                throw new InvalidOperationException("Failed to deserialize the config file");

            OptionDictionary options = [];
            foreach (var pair in optionsDict)
            {
                options.Add(pair.Key, pair.Value);
            }

            return options;
        }

        public IEnumerator<KeyValuePair<string, List<string>>> GetEnumerator()
        {
            return _optionDictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
