namespace CLI.Templates
{
    public class MTlsTemplate : TemplateBase
    {
        protected override string TemplateString =>
@"mtls:
    enabled: true
    allowedClockSkew: 15m
    workloadCertTTL: 24h
";
    }
}