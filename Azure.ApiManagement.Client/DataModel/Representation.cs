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

        [JsonProperty("schemaId", NullValueHandling =NullValueHandling.Ignore)]
        public string SchemaId { get; set; }

        [JsonProperty("typeName", NullValueHandling = NullValueHandling.Ignore)]
        public string TypeName { get; set; }

        [JsonProperty("formParameters", NullValueHandling = NullValueHandling.Ignore)]
        public List<FormParameter> FormParameters { get; set; }
    }
}
