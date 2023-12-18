using Irony.Parsing;

namespace Mss.Ast
{
    public class InvalidTokenStringException(string message, SourceLocation location)
        : Exception(message + " at " + location.ToString())
    { }
}