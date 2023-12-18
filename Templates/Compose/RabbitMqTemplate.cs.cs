namespace CLI.Templates
{
  public class RabbitMqTemplate : TemplateBase
  {
    protected override string TemplateString =>
@"###############################################
# RabbitMq
###############################################     
  rabbitmq:
    container_name: rabbitmq
    image: ""rabbitmq:management""
    hostname: ""rabbitmq"" # hostname used in the rabbitmq.yml component
    ports:
      - ""5672:5672"" # port used in the rabbitmq.yml component
      - ""15672:15672"" # port used for rabbitmq management
      
";
  }
}