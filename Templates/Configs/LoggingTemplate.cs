namespace CLI.Templates
{
    public class LoggingTemplate : TemplateBase
    {
        protected override string TemplateString =>
@"logging:
    apiLogging:
        enabled: false
        obfuscateURLs: false
        omitHealthChecks: false
";
    }
}