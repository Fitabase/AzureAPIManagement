using Newtonsoft.Json;

namespace Fitabase.Azure.ApiManagement.Swagger
{

    /// <summary>
    /// Describes the operations available on a single path. A Path Item 
    /// may be empty, due to ACL constraints. The path itself is still 
    /// exposed to the documentation viewer but they will not know which 
    /// operations and parameters are available.
    /// </summary>
    public class PathData
    {
        [JsonProperty("$ref")]
        public string Ref { get; set; }             // Allows for an external definition of this path item
        [JsonProperty("post")]
        public OperationMethod Post { get; set; }   // A definition of a POST operation on this path.
        [JsonProperty("get")]
        public OperationMethod Get { get; set; }    // A definition of a GET operation on this path.
        [JsonProperty("patch")]
        public OperationMethod Patch { get; set; }  // A definition of a PATCH operation on this path.
        [JsonProperty("put")]
        public OperationMethod Put { get; set; }    // A definition of a PUT operation on this path.
        [JsonProperty("delete")]
        public OperationMethod Delete { get; set; } // A definition of a DELETE operation on this path.
        [JsonProperty("options")]
        public OperationMethod Options { get; set; }// A definition of a OPTIONS operation on this path.
        [JsonProperty("head")]
        public OperationMethod Head { get; set; }   // A definition of a HEAD operation on this path.
        [JsonProperty("parameters")]
        public Parameter[] Parameters { get; set; } // A list of parameters that are applicable for all the operations described under this path
    }

}
