namespace CLI.Templates
{
  public class ComposeTemplate : TemplateBase
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
      ""-app-port"", <APP_PORT>, # The port your application is exposed to, e.g. 5000
      ""-placement-host-address"", ""placement:50005"",
      ""-components-path"", ""/dapr/components"",
      ""-config"", ""/dapr/config/config.yaml"",
      ""-log-level"", ""debug"",
      {https}
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