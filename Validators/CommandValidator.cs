using System.CommandLine.Parsing;
using CLI.Settings;

namespace CLI.Validation
{
    public abstract class CommandValidator() : IValidator
    {
        public void Validate(CommandResult commandResult, ISettings settings) { }
    }
}