using FluentAssertions;
using Irony.Parsing;
using Mss.Parsing;

namespace MssParsingTest
{
    [TestClass]
    public class TryParse
    {
        private readonly MssGrammar _grammar;
        private readonly Parser _sut;

        public TryParse()
        {
            _grammar = new MssGrammar();
            _sut = new Parser(_grammar);
        }

        [TestMethod]
        public void Should_Not_Have_Language_Errors()
        {

            var language = new LanguageData(_grammar);
            language.Errors.Count.Should().Be(0);
        }

        [TestMethod]
        public void Empty_Should_Have_Error()
        {
            var parseTree = _sut.Parse("", "test");
            parseTree.HasErrors().Should().BeTrue();
        }

        [TestMethod]
        public void Empty_Class_Should_Have_Error()
        {
            var parseTree = _sut.Parse(MssSamples.EmptyClass, "test");
            parseTree.HasErrors().Should().BeTrue();
        }

        [TestMethod]
        public void Basic_Class_Should_Not_Have_Error()
        {
            var parseTree = _sut.Parse(MssSamples.BasicClass, "test");
            parseTree.HasErrors().Should().BeFalse();
        }

        [TestMethod]
        public void Dual_Property_Class_Should_Have_Error()
        {
            var parseTree = _sut.Parse(MssSamples.DualPropertyClass, "test");
            parseTree.HasErrors().Should().BeTrue();
        }

        [TestMethod]
        public void Class_With_Class_Property_Type_Should_Have_Error()
        {
            var parseTree = _sut.Parse(MssSamples.ClassWithClassPropertyType, "test");
            parseTree.HasErrors().Should().BeTrue();
        }

        [TestMethod]
        public void Extern_With_Class_Only_Should_Have_Error()
        {
            var parseTree = _sut.Parse(MssSamples.ExternWithClassOnly, "test");
            parseTree.HasErrors().Should().BeTrue();
        }

        [TestMethod]
        public void Extern_With_Only_Project_Should_Have_Error()
        {
            var parseTree = _sut.Parse(MssSamples.ExternWithoutNamespace, "test");
            parseTree.HasErrors().Should().BeTrue();
        }

        [TestMethod]
        public void Extern_With_Single_Namespace_Should_Not_Have_Error()
        {
            var parseTree = _sut.Parse(MssSamples.ExternWithSingleNamespace, "test");
            parseTree.HasErrors().Should().BeFalse();
        }

        [TestMethod]
        public void Extern_With_Multiple_Namespaces_Should_Not_Have_Error()
        {
            var parseTree = _sut.Parse(MssSamples.ExterwWithMultipleNamespaces, "test");
            parseTree.HasErrors().Should().BeFalse();
        }

        [TestMethod]
        public void Empty_Service_Should_Have_Error()
        {
            var parseTree = _sut.Parse(MssSamples.EmptyService, "test");
            parseTree.HasErrors().Should().BeTrue();
        }

        [TestMethod]
        public void Basic_Service_Should_Not_Have_Error()
        {
            var parseTree = _sut.Parse(MssSamples.BasicService, "test");
            parseTree.HasErrors().Should().BeFalse();
        }

        [TestMethod]
        public void Basic_Service_Without_Types_Should_Not_Have_Error()
        {
            var parseTree = _sut.Parse(MssSamples.BasicServiceWithoutTypes, "test");
            parseTree.HasErrors().Should().BeFalse();
        }
    }
}