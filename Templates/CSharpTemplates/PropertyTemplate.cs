namespace CLI.Templates
{
    public class PropertyTemplate() : HandlebarTemplate(TEMPLATE_STR)
    {
        private const string TEMPLATE_STR = "        public {{{type}}} {{name}} { get; set; }";

        protected override string TemplateString => TEMPLATE_STR;

        public string Render(string propertyName, string typeName)
        {
            var data = new
            {
                type = typeName,
                name = propertyName
            };
            return _template(data);
        }
    }
}