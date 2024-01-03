namespace Daprify.Models
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
    }
}