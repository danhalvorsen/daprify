namespace CLI.Models
{
    public class RelativePath : MyPath
    {
        public IPath TargetPath { get; private set; }

        public RelativePath(string path) : base(path) { }

        public RelativePath(IPath basePath, IPath targetPath) : base(GetRelativePath(basePath, targetPath))
        {
            TargetPath = targetPath ?? throw new ArgumentNullException(nameof(targetPath));
        }

        private static string GetRelativePath(IPath basePath, IPath targetPath)
        {
            if (basePath == null || targetPath == null)
                throw new ArgumentNullException(basePath == null ? nameof(basePath) : nameof(targetPath));

            return Path.GetRelativePath(basePath.ToString(), targetPath.ToString());
        }
    }
}