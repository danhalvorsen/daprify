namespace Mss.Ast
{
    public class InvalidChildCountException(int expected, int found)
        : Exception("Expected " + expected + " children, but found " + found)
    { }
}