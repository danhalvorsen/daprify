namespace CLI.Templates
{
  public class ComposeDaprTemplate : TemplateBase
  {
    protected override string TemplateString =>
@"###############################################
# {service}-dapr
############################################### 
  {service}-dapr:
    container_name: {service}-dapr
    image: ""daprio/daprd:latest""
    command: [""./daprd"",
      ""-app-id"", ""{service}"",
      ""-app-port"", ""{port}"",
      ""-placement-host-address"", ""placement:50005"",
      ""-components-path"", ""/dapr/components"",
      ""-config"", ""/dapr/config/config.yaml"",
      ""-log-level"", ""debug"",
      {dapr-https}
      {mtls}
      ]
    volumes:
      - ""./Dapr/Components/:/dapr/components""
      - ""./Dapr/Config/:/dapr/config""
    {env_file}     
    depends_on:
      - {service}
    network_mode: ""service:{service}""


";
  }
}