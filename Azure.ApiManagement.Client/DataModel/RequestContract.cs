using Newtonsoft.Json;
using System.Collections.Generic;

namespace Fitabase.Azure.ApiManagement.Model
{

    /// <summary>
    /// This class containing request details
    /// </summary>
    public class RequestContract
    {
      
        public static RequestContract Create(string description = null, 
                                            ParameterContract[] queryParameters = null, 
                                            ParameterContract[] headers = null, 
                                            RepresentationContract[] represenations = null)
        {
            RequestContract e = new RequestContract();
            e.Description = description;
            e.QueryParameters = queryParameters;
            e.Headers = headers;
            e.Representations = represenations;
            return e;
        }


        [JsonProperty("description")]
        public string Description { get; set; }         // Operation request description.

        [JsonProperty("queryParameters")]
        public ParameterContract[] QueryParameters { get; set; }   // Collection of operation request query parameters.

        [JsonProperty("headers")]
        public ParameterContract[] Headers { get; set; }            // Collection of operation request headers

        [JsonProperty("representations")]
        public RepresentationContract[] Representations { get; set; }   // Collection of operation request representations.
    }

}
