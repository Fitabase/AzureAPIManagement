using Newtonsoft.Json;
using System.Collections.Generic;

namespace Fitabase.Azure.ApiManagement.Swagger.Models
{

    /// <summary>
    /// An object to hold responses to be reused across operations.
    /// </summary>
    public class Responses
    {
        [JsonProperty("Response")]
        public List<Response> Response;
    }
}
