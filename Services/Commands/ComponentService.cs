using CLI.Models;
using CLI.Templates;

namespace CLI.Services
{
    public class ComponentService(TemplateFactory templateFactory) : CommandService(COMPONENT_NAME)
    {
        protected readonly TemplateFactory _templateFactory = templateFactory;
        private const string COMPONENT_NAME = "Components";

        protected override List<string> CreateFiles(OptionDictionary options, IPath workingDir)
        {
            List<string> generatedYamls = [];
            List<string> componentOpt = options.GetAllPairValues(COMPONENT_NAME.ToLower());

            foreach (string argument in componentOpt)
            {
                string yaml = GetArgumentTemplate(argument, null!);
                DirectoryService.WriteFile(workingDir, $"{argument}.yaml", yaml);
                generatedYamls.Add(argument);
            }

            return generatedYamls;
        }

        protected override string GetArgumentTemplate(string argument, string template)
        {
            return argument.ToLower() switch
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