namespace Daprify.Templates
{
    public class HttpsDaprTemplate : TemplateBase
    {
        protected override string TemplateString =>
@"""-app-protocol"", ""https"", # Communication protocol between application and sidecar. Equal to the application's protocol.";
    }
}


