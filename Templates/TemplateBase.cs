using HandlebarsDotNet;
using System.Text.RegularExpressions;

namespace CLI.Templates
{

    public interface ITemplateBase
    {
        string GetTemplate();
    }


    public abstract class TemplateBase : ITemplateBase
    {
        protected HandlebarsTemplate<object, object> _template;

        protected abstract string TemplateString { get; }

        public TemplateBase()
        {
            _template = Handlebars.Compile(TemplateString);
        }

        public string GetTemplate()
        {
            return TemplateString;
        }
    }
}
