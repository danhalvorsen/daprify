namespace CLI.Templates
{
  public class HttpsServiceTemplate : TemplateBase
  {
    protected override string TemplateString =>
@"- ASPNETCORE_URLS=https://+:{{port}}
      - ASPNETCORE_Kestrel__Certificates__Default__Password=""<DEV_CRT_PASSWORD>"" # Password for your certificate. Read more at:
      - ASPNETCORE_Kestrel__Certificates__Default__Path=/https/aspnetapp.pfx # https://learn.microsoft.com/en-us/aspnet/core/security/docker-compose-https?view=aspnetcore-8.0
    volumes:
      - ${HOME}/.aspnet/https/aspnetapp.pfx:/https/aspnetapp.pfx";

    public string Render(string servicePort)
    {
      var data = new
      {
        port = servicePort
      };

      return _template(data);
    }
  }
}