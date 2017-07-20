using Newtonsoft.Json;
namespace Fitabase.Azure.ApiManagement.Swagger.Models
{

    /// <summary>
    /// Describes a single response from an API Operation.
    /// </summary>
    public class Response
    {
        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("schema")]
        public Schema Schema { get; set; }

        [JsonProperty("headers")]
        public Header Headers { get; set; }         // A list of headers that are sent with the response.

        [JsonProperty("examples")]
        public Example Examples { get; set; }
    }
}
