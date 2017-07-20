using Newtonsoft.Json;
using System.Collections.Generic;

namespace Fitabase.Azure.ApiManagement.Swagger.Models
{

    /// <summary>
    /// Describes a single API operation on a path.
    /// </summary>
    public class OperationMethod
    {
        [JsonProperty("tags")]
        public string[] Tags { get; set; }              // A list of tags for API documentation control.

        [JsonProperty("summary")]
        public string Summary { get; set; }             // A short summary of what the operation does.

        //[JsonProperty("description")]
        //public string Description { get; set; }         // 	A verbose explanation of the operation behavior.

        //[JsonProperty("externalDocs")]
        //public ExternalDocs ExternalDocs { get; set; }  // Additional external documentation for this operation.

        [JsonProperty("operationId")]
        public string OperationId { get; set; }         // Unique string used to identify the operation.

        [JsonProperty("consumes")]
        public string[] Consumes { get; set; }          // A list of MIME types the operation can consume.

        [JsonProperty("produces")]
        public string[] Produces { get; set; }          // A list of MIME types the operation can produce.

        [JsonProperty("parameters")]
        public Parameter[] Parameters { get; set; }     // A list of parameters that are applicable for this operation.

        [JsonProperty("responses")]
        public Dictionary<string, Response> Responses { get; set; }  // The list of possible responses as they are returned from executing this operation.

        [JsonProperty("schemes")]
        public string[] Schemes { get; set; }           // The transfer protocol for the operation. 

        [JsonProperty("deprecated")]
        public bool Deprecated { get; set; }            // 	Declares this operation to be deprecated.

        //[JsonProperty("security")]
        //public Security Security { get; set; }          // A declaration of which security schemes are applied for this operation. 
    }


}
