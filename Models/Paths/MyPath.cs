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
        private string _path = string.Empty;

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
            _path = path ?? throw new ArgumentNullException(nameof(path));
        }
        public IPath GetPath() => this;

        public override string ToString() => _path;
    }
}