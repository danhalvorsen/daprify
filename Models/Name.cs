namespace CLI.Models
{
    public class Name
    {
        private string name = string.Empty;

        public Name(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            SetName(name);
        }

        public Name() { }

        public void SetName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            this.name = name;
        }

        public Name GetName() => this;

        public override string ToString() => this.name;
    }
}