using Fitabase.Azure.ApiManagement.Model;
using Newtonsoft.Json;

namespace Azure.ApiManagement.Client.DataModel
{
    public class ResponseBody<T>
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("properties")]
        public T Properties { get; set; } 
    }
}
