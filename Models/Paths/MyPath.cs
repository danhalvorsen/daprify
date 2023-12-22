namespace CLI.Models
{
    public interface IPath
    {
        public IPath GetPath();
        public void SetPath(string path);
        public string ToString();
    }

    public class MyPath : IPath
    {
        private string path = string.Empty;

        public MyPath() { }
        public MyPath(string path)
        {
            SetPath(path);
        }

        public static IEnumerable<MyPath> FromStringList(List<string> paths)
        {
            return paths.Select(p => new MyPath(p));
        }

        public void SetPath(string path)
        {
            this.path = path ?? throw new ArgumentNullException(nameof(path));
        }
        public IPath GetPath() => this;

        public override string ToString() => this.path;
    }
}