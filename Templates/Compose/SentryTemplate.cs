namespace Daprify.Templates
{
  public class SentryTemplate : TemplateBase
  {
    protected override string TemplateString =>
@"###############################################
# Sentry
###############################################     
  sentry:
    image: ""daprio/sentry""
    container_name: sentry
    command: [""./sentry"",
      ""-issuer-credentials"", ""/dapr/certs"",
      ""-trust-domain"", ""cluster.local"", # issuer name of the certificates
      ""-config"", ""/dapr/config/config.yml"",
      ""-log-level"", ""debug"",
    ]
    volumes:
      - ""../Certs/:/dapr/certs""
      - ""../Components/:/dapr/components""
      - ""../Config/:/dapr/config""
    ports:
      - ""8082:8080""
      
";
  }
}