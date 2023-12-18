namespace CLI.Templates
{
    public abstract class TemplateBase
    {
        protected abstract string TemplateString { get; }

        public string GetTemplate()
        {
            return TemplateString;
        }
    }
}
