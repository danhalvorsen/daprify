
using Daprify.Templates;
using Moq;

namespace DaprifyTests.Mocks
{
    public class MockServiceProvider : Mock<IServiceProvider>
    {
        private readonly BindingsTemplate bindingsTemplate = new();
        private readonly ConfigStoreTemplate configStoreTemplate = new();
        private readonly ConfigTemplate configTemplate;
        private readonly ComposeDaprTemplate composeTemplate;
        private readonly ComposeComponentTemplate composeComponentTemplate;
        private readonly ComposeServiceTemplate composeServiceTemplate;
        private readonly ComposeStartTemplate composeStartTemplate = new();
        private readonly CryptoTemplate cryptoTemplate = new();
        private readonly DaprDashboardTemplate daprDashboardTemplate = new();
        private readonly DockerfileTemplate dockerfileTemplate = new();
        private readonly EnvTemplate envTemplate = new();
        private readonly HttpsDaprTemplate httpsDaprTemplate = new();
        private readonly HttpsServiceTemplate httpsServiceTemplate = new();
        private readonly LockTemplate lockTemplate = new();
        private readonly LoggingTemplate loggingTemplate = new();
        private readonly MetricTemplate metricTemplate = new();
        private readonly MiddlewareTemplate middlewareTemplate = new();
        private readonly MtlsCompTemplate mTlsCompTemplate = new();
        private readonly MTlsTemplate mTlsTemplate = new();
        private readonly PlacementTemplate placementTemplate = new();
        private readonly PubSubTemplate pubSubTemplate = new();
        private readonly RabbitMqTemplate rabbitMqTemplate = new();
        private readonly RedisTemplate redisTemplate = new();
        private readonly SecretStoreTemplate secretStoreTemplate = new();
        private readonly SentryTemplate sentryTemplate = new();
        private readonly StateStoreTemplate stateStoreTemplate = new();
        private readonly TracingTemplate tracingTemplate = new();
        private readonly ZipkinTemplate zipkinTemplate = new();

        public MockServiceProvider()
        {
            TemplateFactory templateFactory = new(this.Object);
            composeTemplate = new(templateFactory);
            composeComponentTemplate = new(templateFactory);
            composeServiceTemplate = new(templateFactory);
            configTemplate = new(templateFactory);

            Setup(x => x.GetService(typeof(BindingsTemplate))).Returns(bindingsTemplate);
            Setup(x => x.GetService(typeof(ConfigStoreTemplate))).Returns(configStoreTemplate);
            Setup(x => x.GetService(typeof(ComposeComponentTemplate))).Returns(composeComponentTemplate);
            Setup(x => x.GetService(typeof(ComposeDaprTemplate))).Returns(composeTemplate);
            Setup(x => x.GetService(typeof(ComposeServiceTemplate))).Returns(composeServiceTemplate);
            Setup(x => x.GetService(typeof(ComposeStartTemplate))).Returns(composeStartTemplate);
            Setup(x => x.GetService(typeof(ConfigTemplate))).Returns(configTemplate);
            Setup(x => x.GetService(typeof(CryptoTemplate))).Returns(cryptoTemplate);
            Setup(x => x.GetService(typeof(DaprDashboardTemplate))).Returns(daprDashboardTemplate);
            Setup(x => x.GetService(typeof(DockerfileTemplate))).Returns(dockerfileTemplate);
            Setup(x => x.GetService(typeof(EnvTemplate))).Returns(envTemplate);
            Setup(x => x.GetService(typeof(HttpsDaprTemplate))).Returns(httpsDaprTemplate);
            Setup(x => x.GetService(typeof(HttpsServiceTemplate))).Returns(httpsServiceTemplate);
            Setup(x => x.GetService(typeof(LockTemplate))).Returns(lockTemplate);
            Setup(x => x.GetService(typeof(LoggingTemplate))).Returns(loggingTemplate);
            Setup(x => x.GetService(typeof(MetricTemplate))).Returns(metricTemplate);
            Setup(x => x.GetService(typeof(MiddlewareTemplate))).Returns(middlewareTemplate);
            Setup(x => x.GetService(typeof(MtlsCompTemplate))).Returns(mTlsCompTemplate);
            Setup(x => x.GetService(typeof(MTlsTemplate))).Returns(mTlsTemplate);
            Setup(x => x.GetService(typeof(PlacementTemplate))).Returns(placementTemplate);
            Setup(x => x.GetService(typeof(PubSubTemplate))).Returns(pubSubTemplate);
            Setup(x => x.GetService(typeof(RabbitMqTemplate))).Returns(rabbitMqTemplate);
            Setup(x => x.GetService(typeof(RedisTemplate))).Returns(redisTemplate);
            Setup(x => x.GetService(typeof(SecretStoreTemplate))).Returns(secretStoreTemplate);
            Setup(x => x.GetService(typeof(SentryTemplate))).Returns(sentryTemplate);
            Setup(x => x.GetService(typeof(StateStoreTemplate))).Returns(stateStoreTemplate);
            Setup(x => x.GetService(typeof(TracingTemplate))).Returns(tracingTemplate);
            Setup(x => x.GetService(typeof(ZipkinTemplate))).Returns(zipkinTemplate);
        }
    }
}