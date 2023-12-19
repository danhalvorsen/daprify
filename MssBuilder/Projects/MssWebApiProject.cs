using CLI.Templates;

namespace MssBuilder.Projects
{
    public class MssWebApiProject(string name, string path) : MssCSharpProject(name, path)
    {
        // Todo: use a webapi template
        private readonly ClassLibraryProjectTemplate _template = new();

        protected override string GetCsProjContent()
        {
            return _template.Render();
        }
    }
}