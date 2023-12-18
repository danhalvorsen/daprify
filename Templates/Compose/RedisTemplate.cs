namespace CLI.Templates
{
  public class RedisTemplate : TemplateBase
  {
    protected override string TemplateString =>
@"###############################################
# Redis
###############################################    
  redis:
    container_name: redis
    image: ""redis:latest""
    hostname: redishost # hostname used in the redis.yml component
    ports:
      - ""6379:6379"" # port used in the redis.yml component
      
";
  }
}