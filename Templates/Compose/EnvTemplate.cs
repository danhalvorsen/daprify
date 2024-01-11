namespace Daprify.Templates
{
    public class EnvTemplate : TemplateBase
    {
        protected override string TemplateString =>
@"env_file: 
    - ../Env/Dapr.env";
    }
}