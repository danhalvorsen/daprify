namespace CLI.Templates
{
    public class CryptoTemplate : TemplateBase
    {
        protected override string TemplateString =>
@"apiVersion: dapr.io/v1alpha1
kind: Component
metadata:
  name: jwks
spec:
  type: crypto.dapr.jwks
  version: v1
  metadata:
    # Example 1: load JWKS from file
    - name: ""jwks""
      value: ""fixtures/crypto/jwks/jwks.json""
    # Example 2: load JWKS from a HTTP(S) URL
    # Only ""jwks"" is required
    - name: ""jwks""
      value: ""https://example.com/.well-known/jwks.json""
    - name: ""requestTimeout""
      value: ""30s""
    - name: ""minRefreshInterval""
      value: ""10m""
    # Option 3: include the actual JWKS
    - name: ""jwks""
      value: |
        {
          ""keys"": [
            {
              ""kty"": ""RSA"",
              ""use"": ""sig"",
              ""kid"": ""…"",
              ""n"": ""…"",
              ""e"": ""…"",
              ""issuer"": ""https://example.com""
            }
          ]
        }        
    # Option 3b: include the JWKS base64-encoded
    - name: ""jwks""
      value: |
                eyJrZXlzIjpbeyJ…

# For more information: https://docs.dapr.io/reference/components-reference/supported-cryptography/
";
    }
}