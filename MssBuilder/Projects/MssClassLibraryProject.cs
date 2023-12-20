using CLI.Templates;

namespace MssBuilder.Projects
{
    public class MssClassLibraryProject(string name, string path) : MssCSharpProject(name, path)
    {
        private readonly ClassLibraryProjectTemplate _template = new();

        protected override string GetCsProjContent()
        {
            return _template.Render();
        }
    }
}