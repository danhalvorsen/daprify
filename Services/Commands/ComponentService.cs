using Daprify.Models;
using Daprify.Templates;
using Serilog;

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
                Log.Verbose("Starting to create template for component: {component}", argument);
                string yaml = GetArgumentTemplate(argument);
                DirectoryService.WriteFile(workingDir, $"{argument}.yml", yaml);
                generatedYamls.Add(argument.ToString());
            }

            return generatedYamls;
        }

        private string GetArgumentTemplate(Value argument)
        {
            Log.Verbose("Getting template for component: {component}", argument);
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