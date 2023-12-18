
using CLI.Templates;
using Moq;

namespace CLITests
{
    public class MockServiceProvider : Mock<IServiceProvider>
    {
        private readonly PubSubTemplate pubSubTemplate = new();
        private readonly StateStoreTemplate stateStoreTemplate = new();
        private readonly ConfigTemplate configTemplate = new();
        private readonly MTlsTemplate mTlsTemplate = new();
        private readonly TracingTemplate tracingTemplate = new();
        private readonly ComposeTemplate composeTemplate = new();
        private readonly ComposeStartTemplate composeStartTemplate = new();
        private readonly RabbitMqTemplate rabbitMqTemplate = new();
        private readonly RedisTemplate redisTemplate = new();

        public MockServiceProvider()
        {
            Setup(x => x.GetService(typeof(PubSubTemplate))).Returns(pubSubTemplate);
            Setup(x => x.GetService(typeof(StateStoreTemplate))).Returns(stateStoreTemplate);
            Setup(x => x.GetService(typeof(ConfigTemplate))).Returns(configTemplate);
            Setup(x => x.GetService(typeof(MTlsTemplate))).Returns(mTlsTemplate);
            Setup(x => x.GetService(typeof(TracingTemplate))).Returns(tracingTemplate);
            Setup(x => x.GetService(typeof(ComposeTemplate))).Returns(composeTemplate);
            Setup(x => x.GetService(typeof(ComposeStartTemplate))).Returns(composeStartTemplate);
            Setup(x => x.GetService(typeof(RabbitMqTemplate))).Returns(rabbitMqTemplate);
            Setup(x => x.GetService(typeof(RedisTemplate))).Returns(redisTemplate);
        }
    }
}