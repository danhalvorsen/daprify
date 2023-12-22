using CLI.Models;
using FluentAssertions;

namespace CLITests.Assert
{
    public class Asserts
    {
        public static void VerifyNotNull<T>(T item)
        {
            item.Should().NotBeNull();
        }

        public static void VerifyItemCount<T>(IEnumerable<T> items, int expectedCount)
        {
            items.Should().NotBeNull().And.HaveCount(expectedCount);
        }

        public static void VerifyString(IPath actual, IPath expected)
        {
            actual.Should().Be(expected);
        }

        public static void VerifyEnumerableString(IEnumerable<IPath> actual, IEnumerable<IPath> expected)
        {
            actual.Should().BeEquivalentTo(expected);
        }

        public static void VerifyException<T>(Action act) where T : Exception
        {
            act.Should().ThrowExactly<T>();
        }
    }
}