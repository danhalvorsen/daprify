namespace CLI.Templates
{
  public class StateStoreTemplate : TemplateBase
  {
    protected override string TemplateString =>
@"apiVersion: dapr.io/v1alpha1
kind: Component
metadata:
  name: redis
spec:
  type: pubsub.redis
  version: v1
  metadata:
    - name: redisHost
      value: redishost:6379 

# For more information: https://docs.dapr.io/reference/components-reference/supported-state-stores/setup-redis/
      ";
  }
}