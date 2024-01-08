using Serilog;
using System.Collections;
using System.Text.Json;

namespace Daprify.Models
{
    public class OptionDictionary() : IEnumerable<KeyValuePair<Key, OptionValues>>
    {
        private readonly Dictionary<Key, OptionValues> _optionDictionary = [];

        public void Add(Key key, OptionValues optionValues)
        {
            optionValues.SetKey(key);
            _optionDictionary[key] = optionValues;
            Log.Verbose("Added option {key} with values {values}", key, optionValues.GetStringEnumerable());
        }

        public void Remove(Key key, Value value)
        {
            _optionDictionary[key].RemoveValue(value);
        }

        public OptionValues GetAllPairValues(Key key)
        {
            if (_optionDictionary.TryGetValue(key, out OptionValues? value))
            {
                return value;
            }
            return new OptionValues(key);
        }

        public static OptionDictionary PopulateFromJson(string filePath)
        {
            VerifyFilepath(filePath);

            string json = File.ReadAllText(filePath);
            Dictionary<string, List<string>> optionsDict = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(json) ??
                throw new InvalidOperationException("Failed to deserialize the config file");

            OptionDictionary options = [];
            foreach (var pair in optionsDict)
            {
                Key key = new(pair.Key);
                OptionValues optionValues = new(key, pair.Value);
                Log.Verbose("Adding option {key} with values {values}", key, optionValues.GetStringEnumerable());
                options.Add(key, optionValues);
            }

            return options;
        }

        private static void VerifyFilepath(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"The config file at {Path.GetFullPath(filePath)} was not found");
                Console.ResetColor();
                Console.WriteLine("Shutting down...");

                Environment.Exit(1);
            }
        }


        public IEnumerator<KeyValuePair<Key, OptionValues>> GetEnumerator()
        {
            return _optionDictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
