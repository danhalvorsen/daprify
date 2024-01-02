using CLI.Models;

namespace CLI.Templates
{
  public class ComposeServiceTemplate(TemplateFactory templateFactory) : TemplateBase
  {
    private readonly TemplateFactory _templateFactory = templateFactory;
    protected override string TemplateString =>
@"###############################################
# {{{service}}}
############################################### 
  {{{service}}}:
    container_name: {{{service}}}
    build:
      context: ../../
      dockerfile: Dapr/Docker/{{{dockerfile}}}.Dockerfile
    ports:
      - ""{{{port}}}:{{{port}}}""
    expose:
      - {{{port}}}
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      {{{https}}}
    depends_on:
      {{#each dependencies}}
      - {{{this}}}
      {{/each}}


";

    public string Render(OptionDictionary options, IProject project, string servicePort)
    {
      OptionValues settings = options.GetAllPairValues(new("settings"));
      var data = new
      {
        service = project.GetName().ToString().ToLower(),
        dockerfile = project.GetName(),
        port = servicePort,
        https = GetHttps(settings, servicePort),
        dependencies = GetDependsOn(options)
      };

      string result = _template(data);
      result = PlaceholderRegex().Replace(result, string.Empty);

      return result;
    }

    private static IEnumerable<string> GetDependsOn(OptionDictionary options)
    {
      OptionValues components = options.GetAllPairValues(new("components"));
      foreach (Value component in components.GetValues())
      {
        yield return component.ToString().ToLower();
      }
    }

    private string GetHttps(OptionValues settings, string servicePort)
    {
      if (settings.GetStringEnumerable().Contains("https"))
      {
        HttpsServiceTemplate httpsTemplate = _templateFactory.GetTemplateService<HttpsServiceTemplate>();
        return httpsTemplate.Render(servicePort);
      }
      return "{{}}";
    }
  }
}