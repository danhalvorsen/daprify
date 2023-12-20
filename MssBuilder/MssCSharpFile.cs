namespace MssBuilder
{
    public class MssCSharpFile(string filename, string content)
    {
        public readonly string Filename = filename;
        public readonly string Content = content;

        public void Write(string path)
        {
            string fullPath = Path.Combine(path, Filename);
            string pathOnly = Path.GetDirectoryName(fullPath)!;
            if (!Path.Exists(pathOnly))
            {
                _ = Directory.CreateDirectory(pathOnly);
            }
            File.WriteAllText(fullPath, Content);
        }
    }
}