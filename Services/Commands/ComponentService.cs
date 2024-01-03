using Daprify.Models;
using Daprify.Templates;

namespace Daprify.Services
{
    public class ComponentService(TemplateFactory templateFactory) : CommandService("Components")
    {
        protected readonly TemplateFactory _templateFactory = templateFactory;
        private readonly Key _componentKey = new("components");

        protected override List<string> CreateFiles(OptionDictionary options, IPath workingDir)
        {
            List<string> generatedYamls = [];
            OptionValues componentOpt = options.GetAllPairValues(_componentKey);

            foreach (Value argument in componentOpt.GetValues())
            {
                string yaml = GetArgumentTemplate(argument, null!);
                DirectoryService.WriteFile(workingDir, $"{argument}.yaml", yaml);
                generatedYamls.Add(argument.ToString());
            }

            return generatedYamls;
        }

        private string GetArgumentTemplate(Value argument, string template)
        {
            return argument.ToString().ToLower() switch
            {
                "bindings" => _templateFactory.CreateTemplate<BindingsTemplate>(),
                "configstore" => _templateFactory.CreateTemplate<ConfigStoreTemplate>(),
                "crypto" => _templateFactory.CreateTemplate<CryptoTemplate>(),
                "lock" => _templateFactory.CreateTemplate<LockTemplate>(),
                "pubsub" => _templateFactory.CreateTemplate<PubSubTemplate>(),
                "secretstore" => _templateFactory.CreateTemplate<SecretStoreTemplate>(),
                "statestore" => _templateFactory.CreateTemplate<StateStoreTemplate>(),
                _ => throw new ArgumentException("Invalid component name: " + argument, nameof(argument))
            };
        }
    }
}