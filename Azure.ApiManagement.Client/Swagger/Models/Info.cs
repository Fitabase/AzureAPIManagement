using Newtonsoft.Json;
namespace Fitabase.Azure.ApiManagement.Swagger.Models
{

    /// <summary>
    /// This class contains metadata about the API. This metadata can be used
    /// by the clients if needed.
    /// </summary>
    public class Info
    {
        [JsonProperty("version")]
        public string Version { get; set; }         // Provides the version of the application API

        [JsonProperty("title")]
        public string Title { get; set; }           // The title of the application.

        [JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)]
        public string Description { get; set; }     // A short description of the application. 

        [JsonProperty("termsOfService", NullValueHandling = NullValueHandling.Ignore)]
        public string TermsOfService { get; set; }  // The Terms of Service for the API.

        [JsonProperty("Contact", NullValueHandling = NullValueHandling.Ignore)]
        public Contact Contact { get; set; }        // The contact information for the exposed API.

        [JsonProperty("License", NullValueHandling = NullValueHandling.Ignore)]
        public License License { get; set; }        // The license information for the exposed API

    }
}
