using Newtonsoft.Json;
using System.Collections.Generic;

namespace Fitabase.Azure.ApiManagement.Swagger.Models
{

    /// <summary>
    /// Security Scheme definitions that can be used across the specification
    /// </summary>
    public class SecurityDefinitions
    {
        [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
        public string Type { get; set; }                // The type of the security scheme. Valid values are "basic", "apiKey" or "oauth2".

        [JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)]
        public string Description { get; set; }         // A short description for security scheme.

        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }                // The name of the header or query parameter to be used.

        [JsonProperty("in", NullValueHandling = NullValueHandling.Ignore)]
        public string In { get; set; }                  // The location of the API key. Valid values are "query" or "header".

        [JsonProperty("flow", NullValueHandling = NullValueHandling.Ignore)]
        public string Flow { get; set; }                // The flow used by the OAuth2 security scheme.

        [JsonProperty("authorizationUrl", NullValueHandling = NullValueHandling.Ignore)]
        public string AuthorizationUrl { get; set; }    // The authorization URL to be used for this flow.

        [JsonProperty("tokenUrl", NullValueHandling = NullValueHandling.Ignore)]
        public string TokenUrl { get; set; }            // The token URL to be used for this flow.
        
        [JsonProperty("scopes", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, string> Scopes { get; set; }              // The available scopes for the OAuth2 security scheme.
    }


}
