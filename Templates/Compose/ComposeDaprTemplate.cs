using Daprify.Models;

namespace Daprify.Templates
{
  public class ComposeDaprTemplate(TemplateFactory templateFactory) : TemplateBase
  {
    private readonly TemplateFactory _templateFactory = templateFactory;
    protected override string TemplateString =>
@"###############################################
# {{{service}}}-dapr
############################################### 
  {{{service}}}-dapr:
    container_name: {{{service}}}-dapr
    image: ""daprio/daprd:latest""
    command: [""./daprd"",
      ""-app-id"", ""{{{service}}}"",
      ""-app-port"", ""{{{port}}}"",
      ""-placement-host-address"", ""placement:50005"",
      ""-components-path"", ""/dapr/components"",
      ""-config"", ""/dapr/config/config.yaml"",
      ""-log-level"", ""debug"",
      {{{https}}}
      {{{mtls}}}
      ]
    volumes:
      - ""./Dapr/Components/:/dapr/components""
      - ""./Dapr/Config/:/dapr/config""
    {{{env_file}}}    
    depends_on:
      - {{{service}}}
    network_mode: ""service:{{{service}}}""


";

    public string Render(OptionDictionary options, IProject project, string servicePort)
    {
      OptionValues settings = options.GetAllPairValues(new("settings"));
      var mtlsSetting = GetSettingTemplate("mtls", settings);

      var data = new
      {
        service = project.GetName().ToString().ToLower(),
        port = servicePort,
        https = GetSettingTemplate("https", settings),
        mtls = mtlsSetting,
        env_file = mtlsSetting != "{{}}" ? GetSettingTemplate("env", settings) : "{{}}"
      };

      return _template(data);
    }


    private string GetSettingTemplate(string settingName, OptionValues settings)
    {
      if (settings.GetValues() != null && settings.GetStringEnumerable().Contains(settingName) || settingName == "env")
      {
        return settingName switch
        {
          "https" => _templateFactory.CreateTemplate<HttpsDaprTemplate>(),
          "env" => _templateFactory.CreateTemplate<EnvTemplate>(),
          "mtls" => _templateFactory.CreateTemplate<MtlsCompTemplate>(),
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