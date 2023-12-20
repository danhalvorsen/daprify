namespace CLI.Templates
{
  public class DaprDashboardTemplate : TemplateBase
  {
    protected override string TemplateString =>
@"###############################################
# Dapr-dashboard
###############################################   
  dashboard:
    container_name: dashboard
    image: ""daprio/dashboard:latest""
    command: [ ""--docker-compose=true"", 
      ""--components-path=/home/nonroot/components"", 
      ""--config-path=/home/nonroot/components"", 
      ""--docker-compose-path=/home/nonroot/docker-compose.yml"" ]
    ports:
      - ""9999:8080"" # port used for dapr dashboard
    volumes:
      - ""./components/:/home/nonroot/components""
      - ./docker-compose.yml:/home/nonroot/docker-compose.yml
    depends_on:
      - placement
      
";
  }
}