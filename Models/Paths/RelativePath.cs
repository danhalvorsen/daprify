namespace CLI.Models
{
    public class RelativePath : MyPath
    {
        public IPath _targetPath;

        public RelativePath(string path) : base(path) { }

        public RelativePath(IPath basePath, IPath targetPath) : base(GetRelativePath(basePath, targetPath).ToString())
        {
            _targetPath = targetPath ?? throw new ArgumentNullException(nameof(targetPath));
        }

        public IPath GetTargetPath() => _targetPath;
    }
}