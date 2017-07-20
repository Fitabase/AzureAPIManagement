using Newtonsoft.Json;
namespace Fitabase.Azure.ApiManagement.Swagger.Models
{

    /// <summary>
    /// License information for the exposed API.
    /// </summary>
    public class License
    {
        [JsonProperty("name")]
        public string Name { get; set; }        // The license name used for the API.

        [JsonProperty("url")]
        public string Url { get; set; }         // string	A URL to the license used for the API
    }



}
