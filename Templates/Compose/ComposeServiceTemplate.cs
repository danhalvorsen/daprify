using Daprify.Models;
using Daprify.Settings;
using Serilog;

namespace Daprify.Templates
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
      Log.Verbose("Adding service template to docker-compose for service: {service}", project.GetName());

      return _template(data);
    }

    private static IEnumerable<string> GetDependsOn(OptionDictionary options)
    {
      OptionValues components = options.GetAllPairValues(new("components"));
      foreach (string component in components.GetStringEnumerable())
      {
        string componentName = UpdateComponentName(component.ToLower());
        if (!ComposeComponent(componentName))
        {
          continue;
        }

        yield return componentName;
      }
    }

    private static string UpdateComponentName(string component)
    {
      return component switch
      {
        "pubsub" => "rabbitmq",
        "statestore" => "redis",
        _ => component
      };
    }

    private static bool ComposeComponent(string componentName)
    {
      return ComposeSettings.AvailableComponents.Contains(componentName);
    }


    private string GetHttps(OptionValues settings, string servicePort)
    {
      if (settings.GetValues() != null && settings.GetStringEnumerable().Contains("https"))
      {
        HttpsServiceTemplate httpsTemplate = _templateFactory.GetTemplateService<HttpsServiceTemplate>();
        return httpsTemplate.Render(servicePort);
      }
      return "{{}}";
    }
  }
}