using System.CommandLine;

namespace Daprify.Settings
{
    public class CertificateSettings() : ISettings
    {
        public string CommandName => "gen_certs";
        public string CommandDescription => "Generates certificates needed for the Dapr sidecars and Sentry service.";
        public string CommandExample => "\n\nExamples:\n  dotnet run gen_certs";

        public List<Option<List<string>>> Options { get; set; } = [];
    }
}