using Fitabase.Azure.ApiManagement.Model.Exceptions;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Fitabase.Azure.ApiManagement.Model
{

    /// <summary>
    /// Represents an API operation response
    /// </summary>
    public class ResponseContract
    {

        
        public static ResponseContract Create(int statusCode, string description, RepresentationContract[] representations)
        {
            if (statusCode == 0)
                throw new InvalidEntityException("status code is required");

            ResponseContract response = new ResponseContract();
            response.StatusCode = statusCode;
            response.Description = description;
            response.Representations = representations;

            return response;
        }


        [JsonProperty("statusCode")]
        public int StatusCode { get; set; }                 // Operation response HTTP status code.

        [JsonProperty("description")]
        public string Description { get; set; }             // Operation response description.

        [JsonProperty("representations")]
        public RepresentationContract[] Representations { get; set; }   // Collection of operation response representations

        [JsonProperty("headers")]
        public ParameterContract[] Headers { get; set; }        // Collection of operation response headers

    }

}
