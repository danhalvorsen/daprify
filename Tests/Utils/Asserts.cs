using Daprify.Models;
using FluentAssertions;

namespace DaprifyTests.Assert
{
    public class Asserts
    {

        public static void VerifyEmpty(IEnumerable<string> sut)
        {
            sut.Should().BeEmpty();
        }

        public static void VerifyBool(bool sut, bool expected)
        {
            sut.Should().Be(expected);
        }
        public static void VerifyNotNull<T>(T sut)
        {
            sut.Should().NotBeNull();
        }

        public static void VerifyItemCount<T>(IEnumerable<T> sut, int expectedCount)
        {
            sut.Should().NotBeNull().And.HaveCount(expectedCount);
        }

        public static void VerifyString(string sut, string expected)
        {
            sut.Should().Be(expected);
        }

        public static void VerifyType<T>(object sut)
        {
            sut.Should().BeOfType<T>();
        }

        public static void VerifyEnumerableString(IEnumerable<string> sut, IEnumerable<string> expected)
        {
            sut.Should().BeEquivalentTo(expected);
        }

        public static void VerifyEnumerableValue(IEnumerable<Value> sut, IEnumerable<Value> expected)
        {
            sut.Should().BeEquivalentTo(expected);
        }

        public static void VerifyException<T>(Action act) where T : Exception
        {
            act.Should().ThrowExactly<T>();
        }

        public static void VerifyExceptionWithMessage<T>(Action act, string message) where T : Exception
        {
            act.Should().ThrowExactly<T>().WithMessage(message);
        }

        public static void VerifyTrue(bool sut)
        {
            sut.Should().BeTrue();
        }

        public static void VerifyFalse(bool sut)
        {
            sut.Should().BeFalse();
        }

        public static void VerifyContainSingle<T>(IEnumerable<T> sut)
        {
            sut.Should().ContainSingle();
        }

        public static void VerifyCount<T>(IEnumerable<T> sut, int expectedCount)
        {
            sut.Should().HaveCount(expectedCount);
        }

        public static void VerifyContain(string sut, string expected)
        {
            sut.Should().Contain(expected);
        }
    }
}