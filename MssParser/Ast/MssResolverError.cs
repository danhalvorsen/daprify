using Irony.Parsing;

namespace Mss.Resolver
{
    public class MssResolverError(string msg, SourceLocation loc)
    {
        public string Message { get; } = msg;
        public SourceLocation Location { get; set; } = loc;
    }
}