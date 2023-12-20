namespace CLI.Templates
{
  public class ComposeServiceTemplate : TemplateBase
  {
    protected override string TemplateString =>
@"###############################################
# {service}
############################################### 
  {service}:
    container_name: {service}
    build:
      context: ./
      dockerfile: {Service}.Dockerfile
    ports:
      - ""{port}:{port}""
    expose:
      - {port}
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      {https}
    {depends_on}
";
  }
}