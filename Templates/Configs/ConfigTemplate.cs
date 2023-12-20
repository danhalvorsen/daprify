namespace CLI.Templates
{
  public class ConfigTemplate : TemplateBase
  {
    protected override string TemplateString =>
@"apiVersion: dapr.io/v1alpha1
kind: Configuration
metadata:
  name: daprsystem
spec:
  {{logging}}
  {{metric}}
  {{middleware}}
  {{mtls}}
  {{tracing}}

# For more information: https://docs.dapr.io/operations/configuration/configuration-overview/
";
  }
}