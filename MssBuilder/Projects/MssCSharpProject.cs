namespace MssBuilder.Projects
{
    public abstract class MssCSharpProject(string name, string relPath)
    {
        public readonly string Name = name;
        public readonly string RelPath = relPath;

        private readonly List<MssCSharpFile> _files = [];

        public void AddFile(MssCSharpFile file) => _files.Add(file);
        public void AddFiles(IEnumerable<MssCSharpFile> file) => _files.AddRange(file);

        protected abstract string GetCsProjContent();

        public void Write(string path)
        {
            path = Path.Combine(path, RelPath);
            if (!Path.Exists(path))
            {
                _ = Directory.CreateDirectory(path);
            }
            File.WriteAllText(Path.Combine(path, Name + ".csproj"), GetCsProjContent());
            foreach (var file in _files)
            {
                file.Write(path);
            }
        }
    }
}