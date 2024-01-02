using CLI.Models;

namespace CLI.Templates
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
                .Select(PreProcessArgument)
                .Select(GetArgumentTemplate)
                .Where(template => !string.IsNullOrEmpty(template));

            var data = new
            {
                components = componentTemplates
            };

            return _template(data);
        }

        private static Value PreProcessArgument(Value argument)
        {
            return argument.ToString().ToLower() switch
            {
                "pubsub" => new Value("rabbitmq"),
                "statestore" => new Value("redis"),
                _ => argument
            };
        }

        private string GetArgumentTemplate(Value argument)
        {
            return argument.ToString().ToLower() switch
            {
                "dashboard" => _templateFactory.CreateTemplate<DaprDashboardTemplate>(),
                "placement" => _templateFactory.CreateTemplate<PlacementTemplate>(),
                "rabbitmq" => _templateFactory.CreateTemplate<RabbitMqTemplate>(),
                "redis" => _templateFactory.CreateTemplate<RedisTemplate>(),
                "sentry" => _templateFactory.CreateTemplate<SentryTemplate>(),
                "zipkin" => _templateFactory.CreateTemplate<ZipkinTemplate>(),
                _ => string.Empty
            };
        }
    }
}
