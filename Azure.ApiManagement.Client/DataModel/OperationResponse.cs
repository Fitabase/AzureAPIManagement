using Newtonsoft.Json;
using System.Collections.Generic;

namespace Fitabase.Azure.ApiManagement.Model
{

    /// <summary>
    /// Contain operation response
    /// </summary>
    public class OperationResponse
    {

        public OperationResponse(int statusCode, string description)
        {
            this.StatusCode = statusCode;
            this.Description = description;
        }


        [JsonProperty("statusCode")]
        public int StatusCode { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("representations")]
        public List<Representation> Representations { get; set; }

        [JsonProperty("headers")]
        public List<RequestHeader> Headers { get; set; }

    }

}
