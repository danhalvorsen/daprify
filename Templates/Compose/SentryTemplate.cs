namespace CLI.Templates
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
      ""-config"", ""/dapr/config/config.yaml"",
      ""-log-level"", ""debug"",
    ]
    volumes:
      - ""./Dapr/Certs/:/dapr/certs""
      - ""./Dapr/Components/:/dapr/components""
      - ""./Dapr/Config/:/dapr/config""
    ports:
      - ""8082:8080""
      
";
  }
}