namespace CLI.Templates
{
    public class MiddlewareTemplate : TemplateBase
    {
        protected override string TemplateString =>
@"httpPipeline: # for incoming http calls
    handlers:
        - name: oauth2
            type: middleware.http.oauth2
        - name: uppercase
            type: middleware.http.uppercase
        appHttpPipeline: # for outgoing http calls
        handlers:
        - name: oauth2
            type: middleware.http.oauth2
        - name: uppercase
            type: middleware.http.uppercase
";
    }
}