namespace CLI.Models
{
    public class AbsolutePath : MyPath
    {
        public AbsolutePath(string target)
        {
            if (target != null)
            {
                string absolutePath = Path.GetFullPath(target);
                this.SetPath(absolutePath);
            }

        }
        public AbsolutePath GetRelativePath(AbsolutePath target)
        {
            if (target != null)
            {
                string relativePath = Path.GetRelativePath(this.ToString(), target.ToString());
                this.SetPath(relativePath);
                return this;
            }

            throw new ArgumentNullException(nameof(target));
        }
    }
}