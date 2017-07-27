using Newtonsoft.Json;

namespace Fitabase.Azure.ApiManagement.Model
{

    public class UserIdentity
    {
        [JsonProperty("provider", NullValueHandling = NullValueHandling.Ignore)]
        public string Provider { get; set; }
        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public string Id { get; set; }
    }
}
