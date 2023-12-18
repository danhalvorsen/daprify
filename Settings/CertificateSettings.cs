using System.CommandLine;

namespace CLI.Settings
{
    public class CertificateSettings() : ISettings
    {
        public string CommandName => "gen_certs";
        public string CommandDescription => "Generates certificates needed for the Dapr sidecars.";
        public string CommandExample => "\n\nExamples:\n  dotnet run gen_certs";

        public List<Option<List<string>>> Options { get; set; } = [];
    }
}