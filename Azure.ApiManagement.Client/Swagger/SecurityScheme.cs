using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitabase.Azure.ApiManagement.Swagger
{
    /// <summary>
    /// Allows the definition of a security scheme that can be 
    /// used by the operations. Supported schemes are basic 
    /// authentication, an API key (either as a header or as a 
    /// query parameter) and OAuth2's common flows (implicit, 
    /// password, application and access code).
    /// </summary>
    public class SecurityScheme
    {
        [JsonProperty("type")]
        public string Type { get; set; }                // The type of the security scheme. Valid values are "basic", "apiKey" or "oauth2".

        [JsonProperty("description")]
        public string Description { get; set; }         // A short description for security scheme.

        [JsonProperty("name")]
        public string Name { get; set; }                // The name of the header or query parameter to be used.

        [JsonProperty("in")]
        public string In { get; set; }                  // The location of the API key. Valid values are "query" or "header".

        [JsonProperty("flow")]
        public string Flow { get; set; }                // The flow used by the OAuth2 security scheme.

        [JsonProperty("authorizationUrl")]
        public string AuthorizationUrl { get; set; }    // The authorization URL to be used for this flow.

        [JsonProperty("tokenUrl")]
        public string TokenUrl { get; set; }            // The token URL to be used for this flow.

        [JsonProperty("Scopes")]
        public Scope Scopes { get; set; }               // The available scopes for the OAuth2 security scheme.
    }
}
