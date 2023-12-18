namespace CLI.Templates
{
    public class EnvTemplate : TemplateBase
    {
        protected override string TemplateString =>
@"env_file: 
    - ""./Dapr/Env/Dapr.env""";
    }
}