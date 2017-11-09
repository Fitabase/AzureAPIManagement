using Newtonsoft.Json;
namespace Fitabase.Azure.ApiManagement.Swagger.Models
{

    /// <summary>
    /// Allows referencing an external resource for extended documentation.
    /// </summary>
    public class ExternalDoc
    {
        [JsonProperty("description")]
        public string Description { get; set; }     // A short description of the target documentation. 

        [JsonProperty("url")]
        public string Url { get; set; }             // The URL for the target documentation.
    }
}
