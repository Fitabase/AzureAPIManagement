using Newtonsoft.Json;

namespace Fitabase.Azure.ApiManagement.Swagger
{

    /// <summary>
    /// An object to hold responses that can be used across operations. 
    /// This property does not define global responses for all operations.
    /// </summary>
    public class Response
    {
        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("schema")]
        public Schema Schema { get; set; }

        [JsonProperty("headers")]
        public Header Headers { get; set; }

        [JsonProperty("examples")]
        public Example Examples { get; set; }
    }
}
