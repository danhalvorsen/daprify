namespace MssBuilder
{
    public class MssCSharpFile(string filename, string content)
    {
        public readonly string Filename = filename;
        public readonly string Content = content;

        public void Write(string path)
        {
            File.WriteAllText(Path.Combine(path, Filename), Content);
        }
    }
}