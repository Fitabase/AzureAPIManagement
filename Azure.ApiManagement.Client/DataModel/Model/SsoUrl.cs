using Newtonsoft.Json;

namespace Fitabase.Azure.ApiManagement.Model
{
    /// <summary>
    /// Wrapper class for URL containing an authentication token for signing a given user into the developer portal.
    /// </summary>
    public class SsoUrl
    {
        [JsonProperty("value")]
        public string Url;
    }
}
