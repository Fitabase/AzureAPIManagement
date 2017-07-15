using Newtonsoft.Json;
using System.Collections.Generic;

namespace Fitabase.Azure.ApiManagement.Model
{

    /// <summary>
    /// This class containing request details
    /// </summary>
    public class RequestContract
    {
        public RequestContract()
        {

        }


        [JsonProperty("description")]
        public string Description { get; set; }         // Operation request description.

        [JsonProperty("queryParameters")]
        public List<QueryParameter> QueryParameters { get; set; }   // Collection of operation request query parameters.

        [JsonProperty("headers")]
        public List<RequestHeader> Headers { get; set; }
    }

}
