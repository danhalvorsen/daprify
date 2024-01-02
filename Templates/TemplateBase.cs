using HandlebarsDotNet;
using System.Text.RegularExpressions;

namespace CLI.Templates
{

    public interface ITemplateBase
    {
        string GetTemplate();
    }


    public abstract partial class TemplateBase : ITemplateBase
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

        [GeneratedRegex(@"^\s*.*\{\{\s*\}\}.*(\r?\n|\r)?", RegexOptions.Multiline | RegexOptions.Compiled)]
        public static partial Regex PlaceholderRegex();
    }
}
