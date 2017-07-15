using Newtonsoft.Json;
using System.Collections.Generic;

namespace Fitabase.Azure.ApiManagement.Model
{

    /// <summary>
    /// Operation response representation
    /// </summary>
    public class Representation
    {
        [JsonProperty("contentType")]
        public string ContenType { get; set; }

        [JsonProperty("sample")]
        public string Sample { get; set; }

        [JsonProperty("schemaId")]
        public string SchemaId { get; set; }

        [JsonProperty("typeName")]
        public string TypeName { get; set; }

        public List<FormParameter> FormParameters { get; set; }
    }
}
