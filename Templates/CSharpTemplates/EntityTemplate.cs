namespace CLI.Templates
{
    public class EntityTemplate() : HandlebarTemplate(TEMPLATE_STR)
    {
        private const string TEMPLATE_STR =
@"namespace {{Namespace}}
{
    public class {{ClassName}}
    {
{{{Properties}}}
    }
}";

        protected override string TemplateString => TEMPLATE_STR;

        public string Render(string namespaceName, string entityName, string properties)
        {
            var data = new
            {
                Namespace = namespaceName,
                ClassName = entityName,
                Properties = properties
            };
            return _template(data);
        }
    }
}