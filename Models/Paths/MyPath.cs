namespace Daprify.Models
{
    public interface IPath
    {
        IPath GetPath();
        void SetPath(string path);
        string ToString();
        void SetDirectoryPath();
        bool HasFileExtension();
    }

    public class MyPath : IPath
    {
        private string _path = string.Empty;

        public MyPath() { }
        public MyPath(string path)
        {
            SetPath(path);
        }

        public MyPath(Value value)
        {
            SetPath(value.ToString());
        }

        public static IEnumerable<MyPath> FromStringList(IEnumerable<Value> paths)
        {
            return paths.Select(p => new MyPath(p.ToString()));
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

        public void SetDirectoryPath()
        {
            _path = Path.GetDirectoryName(_path) ?? throw new ArgumentNullException(nameof(_path));
        }

        public bool HasFileExtension()
        {
            return Path.HasExtension(_path);
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

        public static MyPath Combine(string basePath, string target)
        {
            if (basePath != null && target != null)
            {
                return new(Path.Combine(basePath, target));
            }

            throw new ArgumentNullException(nameof(target));
        }

        public static MyPath Combine(string basePath, string target1, string target2)
        {
            if (basePath != null && target1 != null && target2 != null)
            {
                return new(Path.Combine(basePath, target1, target2));
            }

            throw new ArgumentNullException(nameof(target2));
        }
    }
}