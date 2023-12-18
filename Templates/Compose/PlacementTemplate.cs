namespace CLI.Templates
{
  public class PlacementTemplate : TemplateBase
  {
    protected override string TemplateString =>
@"###############################################
# Placement
###############################################     
  placement:
    container_name: placement
    image: ""daprio/placement""
    command: [""./placement"", 
      ""-port"", ""50005"" 
      ]
    ports:
      - ""50005:50005""
    volumes:
      - ""./Dapr/Certs/:/var/run/secrets/dapr.io/tls"" # if using TLS
      
";
  }
}