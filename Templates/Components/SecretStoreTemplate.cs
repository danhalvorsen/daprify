namespace CLI.Templates
{
    public class SecretStoreTemplate : TemplateBase
    {
        protected override string TemplateString =>
@"apiVersion: dapr.io/v1alpha1
kind: Component
metadata:
  name: secretstore
spec:
  type: secretstores.<NAME>
  version: v1
  metadata:
  - name: <KEY>
    value: <VALUE>
  - name: <KEY>
    value: <VALUE>

    # For more information: https://docs.dapr.io/reference/components-reference/supported-secret-stores/
";
    }
}