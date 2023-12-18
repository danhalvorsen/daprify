namespace Mss.Types
{
    public class MssExternType(string identifier, string projectName, string[] namespaceAccess) : MssType(identifier)
    {
        private readonly string _projectName = projectName;
        private readonly string[] _namespaceAccess = namespaceAccess;

        public string Name { get => _identifier; }
        public string Project { get => _projectName; }
        public IEnumerable<string> NamespaceAccess { get => _namespaceAccess; }

        public override bool IsSameType(MssType type)
        {
            if (type is MssExternType ext)
            {
                if (_identifier == ext.Name && _projectName == ext.Project &&
                    _namespaceAccess.Length == ext.NamespaceAccess.Count())
                {
                    var extNamespace = ext.NamespaceAccess.ToArray();
                    for (int i = 0; i < _namespaceAccess.Length; i++)
                    {
                        if (_namespaceAccess[i] != extNamespace[i])
                        {
                            return false;
                        }
                    }
                    return true;
                }
            }

            return false;
        }
    }
}