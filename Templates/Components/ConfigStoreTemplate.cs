namespace Daprify.Templates
{
  public class ConfigStoreTemplate : TemplateBase
  {
    protected override string TemplateString =>
@"apiVersion: dapr.io/v1alpha1
kind: Component
metadata:
  name: <NAME>
spec:
  type: configuration.redis
  version: v1
  metadata:
  - name: redisHost
    value: redishost:6379
  - name: redisPassword
    value: **************
  - name: enableTLS
    value: <bool>

# For more information: https://docs.dapr.io/reference/components-reference/supported-configuration-stores/
";
  }
}