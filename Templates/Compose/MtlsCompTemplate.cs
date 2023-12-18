namespace CLI.Templates
{
    public class MtlsCompTemplate : TemplateBase
    {
        protected override string TemplateString =>
@"""-enable-mtls"", # If you want your sidecars to use mTLS communication. 
      ""-sentry-address"", ""sentry:50001"", # The address of the sentry service, default is 50001. Needed for mTLS";
    }
}