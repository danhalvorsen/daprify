using System.CommandLine.Parsing;
using CLI.Settings;

namespace CLI.Validation
{
    public interface IValidator
    {
        public void Validate(CommandResult commandResult, ISettings settings);
    }
}