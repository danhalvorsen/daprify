namespace CLITests.Commands
{
    public interface ICommandLineInterface
    {
        string[] GetCommandLineArgs();
    }

    public class CommandInterface : ICommandLineInterface
    {
        public string[] GetCommandLineArgs()
        {
            return Environment.GetCommandLineArgs();
        }
    }

}