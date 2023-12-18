using FluentAssertions;
using Irony.Parsing;
using Mss.Ast;
using Mss.Parsing;
using Mss.Resolver;

namespace MssParsingTest
{
    [TestClass]
    public class TryResolve
    {
        private readonly Parser _parser;
        private readonly MssResolver _sut = new();

        public TryResolve()
        {
            var grammar = new MssGrammar();
            _parser = new Parser(grammar);
        }

        [TestMethod]
        public void Basic_Class()
        {
            var initialTypeCount = _sut.GetSpec().Types.Count();
            var parseTree = _parser.Parse(MssSamples.BasicClass, "test");
            parseTree.HasErrors().Should().BeFalse();
            var root = parseTree.Root.AstNode as MssSpecNode ??
                throw new InvalidCastException("Unable to cast the root node to MssSpecNode");
            root.Accept(_sut);
            var spec = _sut.GetSpec();
            spec.Types.Count().Should().Be(initialTypeCount + 1);
        }

        [TestMethod]
        public void Basic_Extern()
        {
            var initialTypeCount = _sut.GetSpec().Types.Count();
            var parseTree = _parser.Parse(MssSamples.ExterwWithMultipleNamespaces, "test");
            parseTree.HasErrors().Should().BeFalse();
            var root = parseTree.Root.AstNode as MssSpecNode ??
                throw new InvalidCastException("Unable to cast the root node to MssSpecNode");
            root.Accept(_sut);
            var spec = _sut.GetSpec();
            spec.Types.Count().Should().Be(initialTypeCount + 1);
        }

        [TestMethod]
        public void Basic_Service_Should_Not_Have_Error()
        {
            var parseTree = _parser.Parse(MssSamples.BasicService, "test");
            parseTree.HasErrors().Should().BeFalse();
            var root = parseTree.Root.AstNode as MssSpecNode ??
                throw new InvalidCastException("Unable to cast the root node to MssSpecNode");
            root.Accept(_sut);
            _sut.Errors.Count.Should().Be(0);
        }

        [TestMethod]
        public void Basic_Service_Without_Types_Should_Have_Error()
        {
            var parseTree = _parser.Parse(MssSamples.BasicServiceWithoutTypes, "test");
            parseTree.HasErrors().Should().BeFalse();
            var root = parseTree.Root.AstNode as MssSpecNode ??
                throw new InvalidCastException("Unable to cast the root node to MssSpecNode");
            root.Accept(_sut);
            _sut.Errors.Count.Should().BeGreaterThan(0);
        }
    }
}