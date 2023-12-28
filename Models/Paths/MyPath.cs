namespace CLI.Models
{
    public interface IPath
    {
        IPath GetPath();
        void SetPath(string path);
        string ToString();
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

        public static MyPath GetFullPath(IPath path)
        {
            if (path != null)
            {
                return new(Path.GetFullPath(path.ToString()));
            }

            throw new ArgumentNullException(nameof(path));
        }

        public override string ToString() => _path;

        public static RelativePath GetRelativePath(IPath basePath, IPath target)
        {
            if (basePath != null && target != null)
            {
                return new(Path.GetRelativePath(basePath.ToString(), target.ToString()));
            }

            throw new ArgumentNullException(nameof(target));
        }

        public static MyPath Combine(IPath basePath, IPath target)
        {
            if (basePath != null && target != null)
            {
                return new(Path.Combine(basePath.ToString(), target.ToString()));
            }

            throw new ArgumentNullException(nameof(target));
        }
    }
}