using FluentAssertions;

namespace DaprifyTests.Assert
{
    public static class Asserts
    {

        public static void VerifyNotNull<T>(T sut)
        {
            sut.Should().NotBeNull();
        }

        public static void VerifyItemCount<T>(IEnumerable<T> sut, int expectedCount)
        {
            sut.Should().NotBeNull().And.HaveCount(expectedCount);
        }

        public static void VerifyShouldBe<T>(T sut, T expected)
        {
            sut.Should().Be(expected);
        }

        public static void VerifyType<T>(object sut)
        {
            sut.Should().BeOfType<T>();
        }

        public static void VerifyEnumerableEquivalent<T>(IEnumerable<T> sut, IEnumerable<T> expected)
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