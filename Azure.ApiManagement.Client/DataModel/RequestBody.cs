using Newtonsoft.Json;

namespace Azure.ApiManagement.Client.DataModel
{
    public class RequestBody<T>
    {
        [JsonProperty("properties")]
        public T Properties { get; set; }
    }
}
