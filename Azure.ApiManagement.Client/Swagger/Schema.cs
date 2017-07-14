using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fitabase.Azure.ApiManagement.Swagger
{


    /// <summary>
    /// The Schema Object allows the definition of input and output data types. 
    /// These types can be objects, but also primitives and arrays. This object 
    /// is based on the JSON Schema Specification Draft 4 and uses a predefined 
    /// subset of it. On top of this subset, there are extensions provided by 
    /// this specification to allow for more complete documentation.
    /// </summary>
    public class Schema
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("discriminator")]
        public string Discriminator { get; set; }       // 	Adds support for polymorphism

        [JsonProperty("readOnly")]
        public bool ReadOnly { get; set; }              // Relevant only for Schema "properties" definitions.

        [JsonProperty("externalDocs")]
        public ExternalDocs ExternalDocs { get; set; }  // Additional external documentation for this schema.

        [JsonProperty("xml")]
        public XML Xml { get; set; }                    // This MAY be used only on properties schemas.
    }

}
