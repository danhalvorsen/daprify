using CLI.Models;

namespace CLI.Templates
{
  public class ConfigTemplate(TemplateFactory templateFactory) : TemplateBase
  {
    private readonly TemplateFactory _templateFactory = templateFactory;
    protected override string TemplateString =>
@"apiVersion: dapr.io/v1alpha1
kind: Configuration
metadata:
  name: daprsystem
spec:
  {{{logging}}}
  {{{metric}}}
  {{{middleware}}}
  {{{mtls}}}
  {{{tracing}}}

# For more information: https://docs.dapr.io/operations/configuration/configuration-overview/
";

    public string Render(OptionValues settings)
    {
      var data = new
      {
        logging = GetSettingTemplate("logging", settings),
        metric = GetSettingTemplate("metric", settings),
        middleware = GetSettingTemplate("middleware", settings),
        mtls = GetSettingTemplate("mtls", settings),
        tracing = GetSettingTemplate("tracing", settings),
      };

      return _template(data);
    }

    private string GetSettingTemplate(string settingName, OptionValues settings)
    {
      if (settings.GetStringEnumerable().Contains(settingName))
      {
        return settingName switch
        {
          "logging" => _templateFactory.CreateTemplate<LoggingTemplate>(),
          "metric" => _templateFactory.CreateTemplate<MetricTemplate>(),
          "middleware" => _templateFactory.CreateTemplate<MiddlewareTemplate>(),
          "mtls" => _templateFactory.CreateTemplate<MTlsTemplate>(),
          "tracing" => _templateFactory.CreateTemplate<TracingTemplate>(),
          _ => throw new ArgumentException("Invalid setting name:" + settingName, nameof(settingName))
        };
      }
      else
      {
        return "{{}}";
      }
    }
  }
}