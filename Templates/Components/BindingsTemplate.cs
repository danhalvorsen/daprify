namespace Daprify.Templates
{
  public class BindingsTemplate : TemplateBase
  {
    protected override string TemplateString =>
@"apiVersion: dapr.io/v1alpha1
kind: Component
metadata:
  name: <NAME>
  namespace: <NAMESPACE>
spec:
  type: bindings.<NAME>
  version: v1
  metadata:
  - name: <KEY>
    value: <VALUE>
  - name: <KEY>
    value: <VALUE>

# For more information: https://docs.dapr.io/reference/components-reference/supported-bindings/
";
  }
}