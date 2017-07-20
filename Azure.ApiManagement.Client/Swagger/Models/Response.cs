using Newtonsoft.Json;
using System.Collections.Generic;

namespace Fitabase.Azure.ApiManagement.Swagger.Models
{

    /// <summary>
    /// Describes a single response from an API Operation.
    /// </summary>
    public class Response
    {
        [JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)]
        public string Description { get; set; }

        [JsonProperty("schema", NullValueHandling = NullValueHandling.Ignore)]
        public Schema Schema { get; set; }

        [JsonProperty("headers", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, Header> Headers { get; set; }         // A list of headers that are sent with the response.
        

        [JsonProperty("examples", NullValueHandling = NullValueHandling.Ignore)]
        public Example Examples { get; set; }
    }
}
