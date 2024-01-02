
using CLI.Templates;
using Moq;

namespace CLITests.Mocks
{
    public class MockServiceProvider : Mock<IServiceProvider>
    {
        private readonly ComposeDaprTemplate composeTemplate = new();
        private readonly ComposeServiceTemplate composeServiceTemplate = new();
        private readonly ComposeStartTemplate composeStartTemplate = new();
        private readonly DockerfileTemplate dockerfileTemplate = new();
        private readonly ConfigTemplate configTemplate = new();
        private readonly EnvTemplate envTemplate = new();
        private readonly MtlsCompTemplate mTlsCompTemplate = new();
        private readonly MTlsTemplate mTlsTemplate = new();
        private readonly PubSubTemplate pubSubTemplate = new();
        private readonly RabbitMqTemplate rabbitMqTemplate = new();
        private readonly RedisTemplate redisTemplate = new();
        private readonly SecretStoreTemplate secretStoreTemplate = new();
        private readonly SentryTemplate sentryTemplate = new();
        private readonly StateStoreTemplate stateStoreTemplate = new();
        private readonly TracingTemplate tracingTemplate = new();

        public MockServiceProvider()
        {
            Setup(x => x.GetService(typeof(ComposeDaprTemplate))).Returns(composeTemplate);
            Setup(x => x.GetService(typeof(ComposeServiceTemplate))).Returns(composeServiceTemplate);
            Setup(x => x.GetService(typeof(ComposeStartTemplate))).Returns(composeStartTemplate);
            Setup(x => x.GetService(typeof(ConfigTemplate))).Returns(configTemplate);
            Setup(x => x.GetService(typeof(DockerfileTemplate))).Returns(dockerfileTemplate);
            Setup(x => x.GetService(typeof(EnvTemplate))).Returns(envTemplate);
            Setup(x => x.GetService(typeof(MtlsCompTemplate))).Returns(mTlsCompTemplate);
            Setup(x => x.GetService(typeof(MTlsTemplate))).Returns(mTlsTemplate);
            Setup(x => x.GetService(typeof(PubSubTemplate))).Returns(pubSubTemplate);
            Setup(x => x.GetService(typeof(RabbitMqTemplate))).Returns(rabbitMqTemplate);
            Setup(x => x.GetService(typeof(RedisTemplate))).Returns(redisTemplate);
            Setup(x => x.GetService(typeof(SecretStoreTemplate))).Returns(secretStoreTemplate);
            Setup(x => x.GetService(typeof(SentryTemplate))).Returns(sentryTemplate);
            Setup(x => x.GetService(typeof(StateStoreTemplate))).Returns(stateStoreTemplate);
            Setup(x => x.GetService(typeof(TracingTemplate))).Returns(tracingTemplate);
        }
    }
}