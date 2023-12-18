using Irony.Parsing;
using Mss.Ast;
using Mss.Resolver;

namespace Mss.Parsing
{
    public static class MssParser
    {
        private static void PrintError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("Error: ");
            Console.ResetColor();
            Console.WriteLine(message);
        }

        private static void PrintErrorWithLocation(string filename, SourceLocation location, string message)
        {
            PrintErrorWithLocation(filename, location.Line + 1, location.Column + 1, message);
        }

        private static void PrintErrorWithLocation(string filename, int line, int column, string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("Error: ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(filename);
            Console.ResetColor();
            Console.Write("(");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(line);
            Console.ResetColor();
            Console.Write(",");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(column);
            Console.ResetColor();
            Console.WriteLine("): " + message);
        }

        public static MssSpec? ParseMss(string filename, string mssSource)
        {
            var grammar = new MssGrammar();
            var language = new LanguageData(grammar);
            if (language.Errors.Count > 0)
            {
                foreach (var error in language.Errors.AsEnumerable())
                {
                    PrintError("in grammar: " + error.ToString());
                }
                return null;
            }
            var parser = new Parser(grammar);
            var parseTree = parser.Parse(mssSource, filename);
            if (parseTree.HasErrors())
            {
                foreach (var error in parseTree.ParserMessages)
                {
                    PrintErrorWithLocation(filename, error.Location, error.ToString());
                }
                return null;
            }

            var resolver = new MssResolver();
            var root = parseTree.Root.AstNode as MssSpecNode ??
                throw new InvalidCastException("Unable to cast the root node to MssSpecNode");
            root.Accept(resolver);

            if (resolver.Errors.Count != 0)
            {
                foreach (var e in resolver.Errors)
                {
                    PrintErrorWithLocation(filename, e.Location, e.Message);
                }
                return null;
            }

            return resolver.GetSpec();
        }
    }
}