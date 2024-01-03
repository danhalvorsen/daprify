using System.CommandLine;

namespace Daprify.Settings
{
    public interface ISettings
    {
        public string CommandName { get; }
        public string CommandDescription { get; }
        public string CommandExample { get; }

        public List<Option<List<string>>> Options { get; set; }
    }
}