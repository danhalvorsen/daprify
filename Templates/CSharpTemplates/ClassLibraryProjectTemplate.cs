namespace CLI.Templates
{
    public class ClassLibraryProjectTemplate() : HandlebarTemplate(TEMPLATE_STR)
    {
        private const string TEMPLATE_STR =
@"<Project Sdk=""Microsoft.NET.Sdk"">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

</Project>";

        protected override string TemplateString => TEMPLATE_STR;

        public string Render()
        {
            return _template(new { });
        }
    }
}