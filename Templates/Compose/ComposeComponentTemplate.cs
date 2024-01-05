using Daprify.Models;
using Serilog;

namespace Daprify.Templates
{
    public class ComposeComponentTemplate(TemplateFactory templateFactory) : TemplateBase
    {
        private readonly TemplateFactory _templateFactory = templateFactory;
        protected override string TemplateString =>
@"{{#each components}}
{{{this}}}
{{/each}}";

        public string Render(OptionDictionary options)
        {
            Key componentKey = new("components");
            OptionValues componentOpt = options.GetAllPairValues(componentKey);

            var componentTemplates = componentOpt.GetValues()
                .Select(GetArgumentTemplate)
                .Where(template => !string.IsNullOrEmpty(template));

            var data = new
            {
                components = componentTemplates
            };
            Log.Verbose("Adding component templates to docker-compose: {Components}", componentOpt.GetValues());

            return _template(data);
        }

        private string GetArgumentTemplate(Value argument)
        {
            return argument.ToString().ToLower() switch
            {
                "dashboard" => _templateFactory.CreateTemplate<DaprDashboardTemplate>(),
                "placement" => _templateFactory.CreateTemplate<PlacementTemplate>(),
                "rabbitmq" or "pubsub" => _templateFactory.CreateTemplate<RabbitMqTemplate>(),
                "redis" or "statestore" => _templateFactory.CreateTemplate<RedisTemplate>(),
                "sentry" => _templateFactory.CreateTemplate<SentryTemplate>(),
                "zipkin" => _templateFactory.CreateTemplate<ZipkinTemplate>(),
                _ => string.Empty
            };
        }
    }
}
