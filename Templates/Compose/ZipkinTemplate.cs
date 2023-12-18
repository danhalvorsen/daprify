namespace CLI.Templates
{
  public class ZipkinTemplate : TemplateBase
  {
    protected override string TemplateString =>
@"###############################################
# Zipkin
###############################################     
  zipkin:
    image: ghcr.io/openzipkin/zipkin-slim:${TAG:-latest}
    container_name: zipkin
    environment:
      - STORAGE_TYPE=mem
      - MYSQL_HOST=<DATABASE_CONTAINER_NAME> # The name of the database container (If any)
      - SELF_TRACING_ENABLED=true
    ports:
      - 9411:9411
      
";
  }
}