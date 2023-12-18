namespace CLI.Templates
{
  public class TracingTemplate : TemplateBase
  {
    protected override string TemplateString =>
@"tracing:
    samplingRate: ""1""
    otel: # Use this if otel is used as tracing provider
      endpointAddress: ""https://...""
    zipkin: # Use this if zipkin is used as tracing provider
      endpointAddress: ""http://zipkin.default.svc.cluster.local:9411/api/v2/spans""
";
  }
}