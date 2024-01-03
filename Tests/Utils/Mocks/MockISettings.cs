using Daprify.Settings;
using Moq;

namespace ValidatorTest
{
    public class MockISettings : Mock<ISettings>
    {
        public MockISettings()
        {
            Setup(s => s.Options).Returns(
                [
                    new("--pp", () => []),
                    new("--sp", () => [])
                ]);
        }
    }
}
