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
      value: <HOSTNAME>:<PORT> # HOSTNAME and PORT from the docker container. i.e localhost:6379

# For more information: https://docs.dapr.io/reference/components-reference/supported-state-stores/setup-redis/
      ";
  }
}