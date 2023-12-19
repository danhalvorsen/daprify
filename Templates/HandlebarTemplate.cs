using HandlebarsDotNet;

namespace CLI.Templates
{
    public abstract class HandlebarTemplate(string templateStr) : TemplateBase
    {
        protected HandlebarsTemplate<object, object> _template = Handlebars.Compile(templateStr);
    }
}